using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetTradeHistoryData
{
    public class OkexLQHelpernew
    {
        //币本位永续url
        private const string OKEX_DEFAULT_HOST = "https://www.okex.com/";

        private const string liquidation_orders = "api/v5/public/liquidation-orders?";


        /// <summary>
        /// USDT永续获取时间戳key
        /// </summary>
        private const string usdtswapkey = "_usdtswap_liquidation_orders";

        //交割获取时间戳key
        /// <summary>
        /// 交割获取时间戳key
        /// </summary>
        private const string deliverykey = "_delivery_liquidation_orders";


        /// <summary>
        ///  永续/交割 强平
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        private async void GetLQdata(string symbol, string key, string type, string ccy, string alias, string instId, instruments item)
        {
            var times = RedisHelper.GetHash(CommandEnum.RedisKey.LQTimeStamp, key + "_" + instId);
            //取上次时间戳1616601600000
            //1617120000000
            long timestamp = 1617379200000;

            if (times != "")
            {
                timestamp = Convert.ToInt64(times);
            }

            //获取交易对
            string contractCode = symbol;//品种代码

            //获取初始数据,获取页数数据
            OkexLq result = await GetResultData(type, contractCode, timestamp, alias);
            List<details> data = new List<details>();
            if (result.details.Count() > 0 && result.details.Count == 100)
            {
                var orderslist = result.details;

                if (orderslist.Count > 0)
                {
                    if (orderslist.Where(p => p.ts > timestamp).Count() > 0)//是否存在大于最后一次的时间戳
                    {

                        data.AddRange(orderslist.Where(p => p.ts > timestamp).ToList());//添加首页
                        long maxtime = data.Max(p => p.ts);
                        while (true)
                        {
                            var results = await GetResultData(type, contractCode, maxtime, alias);

                            if (results.details.Count == 0)
                            {
                                break;
                            }
                            data.AddRange(results.details);//添加首页

                            maxtime = results.details.Max(p => p.ts);
                            Console.WriteLine(symbol + GZipDecompresser.GetTimeFromUnixTimestampthree(maxtime.ToString()).ToString());

                        }
                        SaveLQToRedis(data, key, symbol, type, ccy, result.instId, instId, item.ctValCcy);
                    }
                    else//不存在比上次时间戳大的数据
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("爆仓数据数量为0！");
                }
            }
            else
            {
                data.AddRange(result.details.Where(p => p.ts > timestamp).ToList());//添加首页
                SaveLQToRedis(data, key, symbol, type, ccy, result.instId, instId, item.ctValCcy);
            }

        }
        //https://www.okex.com/api/v5/public/liquidation-orders?instType=SWAP&&uly=BTC-USDT&&state=filled

        private async Task<OkexLq> GetResultData(string type, string contractCode, long before, string alias)
        {
            string location = "";
            if (alias == "")
            {
                location = liquidation_orders + $"&&instType={type}&&uly={contractCode}&&before={before}&&state=filled&&limit=100";
            }
            else
            {
                location = liquidation_orders + $"&&instType={type}&&uly={contractCode}&&before={before}&&alias={alias}&&state=filled&&limit=100";
            }
            OkexLq results = null;

            string url = OKEX_DEFAULT_HOST;
            url += location;

            while (true)
            {
                try
                {
                    //var list = await ApiHelper.GetAsync(url);
                    var list = await HttpRequests.GetAsync<dynamic>(url);

                    results = ((object)list.data).ToString().ToList<OkexLq>().FirstOrDefault();

                }
                catch (Exception e)
                {
                    //Console.WriteLine("获取OKex强平订单数据异常，异常信息：" + e.ToString());
                    //Console.WriteLine("再次执行");
                }
                if (results != null)
                {
                    break;
                }

            }
            return results;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="list"></param>
        private void SaveLQToRedis(List<details> list, string key, string symbol, string type, string ccy, string market, string instId, string ctValCcy)
        {
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("key:" + symbol + key + "--首页爆仓数据数量为0！" + DateTime.Now.ToString());
                return;
            }
            else
            {
                Console.WriteLine("key：" + symbol + key + "--爆仓数量：" + list.Count() + "--------" + DateTime.Now.ToString());
            }
            //List<Liquidation> savelist = new List<Liquidation>();
            foreach (var item in list)
            {
                Liquidation o = new Liquidation();
                o.amount = item.sz.ToString();
                o.timestamp = item.ts.ToString();
                o.times = GZipDecompresser.GetTimeFromUnixTimestampthree(item.ts.ToString()).ToString();
                o.Unit = "张";
                o.utctime = GZipDecompresser.GetTimeFromUnixTimestampthree(item.ts.ToString()).ToString();


                if (ctValCcy.ToUpper() == "USD")
                {
                    o.vol = ((item.sz) * (Convert.ToDecimal(ccy))).ToString();
                }
                else
                {
                    o.vol = ((item.sz) * (Convert.ToDecimal(ccy)) * (Convert.ToDecimal(item.bkPx))).ToString();
                }



                o.side = item.side;
                o.exchange = CommandEnum.RedisKey.okex;
                o.pair = symbol;
                o.price = item.bkPx.ToString();
                o.qty = item.sz.ToString();
                o.market = market;
                o.contractcode = symbol;

                if (type == "SWAP")
                {
                    o.kinds = CommandEnum.RedisKey.PERP;
                    o.types = symbol + "--OK 永续";
                }
                else
                {
                    o.types = symbol + "-- OK 交割";
                    o.kinds = CommandEnum.RedisKey.DELIVERY;
                }

                //大单统计
                if (Convert.ToDecimal(o.vol) >= 1000000)
                {
                    Console.WriteLine(o.ToJson());
                    RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                }
                //savelist.Add(o);

                RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));
                RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, o.ToJson());
            }


            var maxtimestamp = list.Max(a => a.ts).ToString();

            RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.LQTimeStamp, key + "_" + instId, maxtimestamp);

            Console.WriteLine("结束爆仓数据统计：" + key + "_" + instId);
            //保存数据到redis
            //更新时间戳值
        }


        /// <summary>
        /// 获取爆仓
        /// </summary>
        public void runall()
        {


            //var instruments = GetCommonData.GetSwapContract_size("SWAP");
            //foreach (var item in instruments)
            //{
            //    var symbol = item.uly;
            //    string key = CommandEnum.RedisKey.okex + usdtswapkey;
            //    GetLQdata(item.uly, key, "SWAP", item.ctVal, "", item.instId, item);
            //}

            //var instrumentsDeliver = GetCommonData.GetSwapContract_size("FUTURES");
            //foreach (var item in instrumentsDeliver)
            //{
            //    string key = CommandEnum.RedisKey.okex + deliverykey;
            //    GetLQdata(item.uly, key, "FUTURES", item.ctVal, item.alias, item.instId, item);
            //}



            //永续
            int b = 0;
            new Thread(() =>
            {
                var instruments = GetCommonData.GetSwapContract_size("SWAP");
                foreach (var item in instruments)
                {

                    string key = CommandEnum.RedisKey.okex + usdtswapkey;
                    GetLQdata(item.uly, key, "SWAP", item.ctVal, "", item.instId, item);
                }
                b = 1;
            }).Start();



            int a = 0;

            new Thread(() =>
            {
                ////交割
                var instrumentsDeliver = GetCommonData.GetSwapContract_size("FUTURES");
                foreach (var item in instrumentsDeliver)
                {
                    string key = CommandEnum.RedisKey.okex + deliverykey;
                    GetLQdata(item.uly, key, "FUTURES", item.ctVal, item.alias, item.instId, item);
                }
                a = 1;
            }).Start();

            while (true)
            {
                if (a == 1 && b == 1)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            Console.WriteLine("CLOSE!");
        }

        public void runlqdata()
        {
            while (true)
            {
                var instruments = GetCommonData.GetSwapContract_size("SWAP");
                foreach (var item in instruments)
                {

                    string key = CommandEnum.RedisKey.okex + usdtswapkey;
                    GetLQdata(item.uly, key, "SWAP", item.ctVal, "", item.instId, item);
                    Console.WriteLine(item.instId + "结束了！");
                }
                Thread.Sleep(20000);
            }

        }


        public void runopenfundingrate()
        {

            GetFundingRateAndOpenInterest("SWAP");
            //GetFundingRateAndOpenInterest("FUTURES");
        }

        /// <summary>
        /// 根据使用symbol获取费率和持仓
        /// </summary>
        private void GetFundingRateAndOpenInterest(string type)
        {
            List<FundRate> FundRatelist = new List<FundRate>();
            List<OpenInterest> OpenInterestlist = new List<OpenInterest>();
            var symbollist = GetCommonData.GetSwapContract_size(type);
            var openlist = GetCommonData.Getopeninterest(type);
            var markpricelist = GetCommonData.GetMarkPrice(type);

            foreach (var item in openlist)
            {
                OpenInterest o = new OpenInterest();
                var sy = symbollist.FirstOrDefault(p => p.instId == item.instId);
                var mp = markpricelist.FirstOrDefault(p => p.instId == item.instId);
                //if (sy.ctValCcy.ToUpper() == "USD")
                //{
                //    o.SumOpenInterestValue = ((item.oi) * (Convert.ToDecimal(sy.ctValCcy)));
                //}
                //else
                //{
                //    o.SumOpenInterestValue = ((item.oi) * (Convert.ToDecimal(sy.ctValCcy)) * (Convert.ToDecimal(mp.markPx)));
                //}
                if (sy != null && mp != null)
                {
                    o.price = mp.markPx;
                    o.SumOpenInterest = item.oiCcy;
                    o.SumOpenInterestValue = item.oiCcy * mp.markPx;//币乘以当前价格
                    o.market = item.instId;
                    o.exchange = CommandEnum.RedisKey.okex;
                    o.SumOpenInterest = Convert.ToDecimal(item.oi);
                    o.symbol = sy.uly;
                    //o.times = DateTime.Now.ToString();
                    o.times = GZipDecompresser.GetTimeFromUnixTimestampthree(item.ts).ToString();
                    o.type = CommandEnum.RedisKey.coin;
                    o.desc = "币";
                    o.kind = type == "SWAP" ? CommandEnum.RedisKey.PERP : CommandEnum.RedisKey.DELIVERY;
                    OpenInterestlist.Add(o);
                }
            }

            if (symbollist != null && type == "SWAP")
            {
                foreach (var item in symbollist)
                {
                    okexFundingRate funs = new okexFundingRate();

                    funs = GetCommonData.GetFundingRate(item.instId).FirstOrDefault();
                    FundRate i = new FundRate();
                    i.market = item.uly;
                    i.exchange = CommandEnum.RedisKey.okex;
                    i.FundingRate = funs.nextFundingRate.ToString();
                    i.symbol = item.uly;
                    i.times = DateTime.Now.ToString();
                    i.NextFundingRate = funs.nextFundingRate.ToString();
                    i.NextFundingRateTime = GZipDecompresser.GetTimeFromUnixTimestampthree(funs.fundingTime).ToString();
                    //i.kind = type == "SWAP" ? CommandEnum.RedisKey.PERP : CommandEnum.RedisKey.DELIVERY;
                    FundRatelist.Add(i);
                }
            }
            else
            {
                Console.WriteLine("OKEX 获取持仓费率时，获取交易对信息失败！！");
            }

            RedisHelper.Pushdata(FundRatelist.ToJson(), "11", CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd"));
            RedisHelper.Pushdata(OpenInterestlist.ToJson(), "11", CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd"));
            Console.WriteLine("okex 结束持仓和费率数据获取：" + CommandEnum.RedisKey.bitmex + DateTime.Now.ToString());
        }
    }
}
