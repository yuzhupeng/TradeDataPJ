using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Threading;

namespace GetTradeHistoryData
{
    public class HuobiliquidationHelper
    {

        //交割https://api.btcgateway.pro/api/v1/contract_liquidation_orders?symbol=BTC&trade_type=0&create_date=7
        //币本位永续https://api.btcgateway.pro/swap-api/v1/swap_liquidation_orders?contract_code=BTC-USD&trade_type=0&create_date=90
        //USDT永续https://api.btcgateway.pro/linear-swap-api/v1/swap_liquidation_orders?contract_code=BTC-USDT&trade_type=0&create_date=90"


        //交易对的问题
        //1.按当前时间和上次对应交易所的最后时间戳 去取数据
        //2.添加到redis中 每小时LQ 集合中， 同时记录每日大单
        //3.获取最后一个的时间 ，记录（更新）到hash中
        //4.
        //5.记录结束时间
        //6.


        //币本位永续url
        private const string HUOBI_DEFAULT_HOST = "https://api.btcgateway.pro";




        //币本位永续获取时间戳key

        /// <summary>
        ///币本位永续获取时间戳key
        /// </summary>
        private const string coinswapkey = "_swap_liquidation_orders";

        //USDT永续获取时间戳key
        /// <summary>
        /// USDT永续获取时间戳key
        /// </summary>
        private const string usdtswapkey = "_usdtswap_liquidation_orders";

        //交割获取时间戳key
        /// <summary>
        /// 交割获取时间戳key
        /// </summary>
        private const string deliverykey = "_delivery_liquidation_orders";




        #region  huobi
        /// <summary>
        /// 币本位永续强平
        /// </summary>
        private void GetLQdata(string symbol, string url, string key)
        {
            //key = CommandEnum.RedisKey.huobi + coinswapkey;

            var times = RedisHelper.GetHash(CommandEnum.RedisKey.LQTimeStamp, key + "_" + symbol);
            //取上次时间戳
            // 1617120000000
            //long timestamp = 1624204800000;
            //if (times != "")
            //{
            //    timestamp = Convert.ToInt64(times);
            //}

            long timestamp = 1624204800000;

            if (times != "")
            {
                timestamp = Convert.ToInt64(times) > 1624204800000 ? Convert.ToInt64(times) : 1624204800000;
            }



            //获取交易对
            string contractCode = symbol;//品种代码
            int tradeType = 0;//交易类型		0:全部,5: 卖出强平,6: 买入强平
            int createDate = 7;//日期		7，90（7天或者90天）
            int? pageIndex = 1;//页码,不填默认第1页	1
            int? pageSize = 50;//不填默认20，不得多于50	20	[1-50]

            //获取初始数据,获取页数数据
            GetLiquidationOrdersResponse result = GetResultData(url, contractCode, tradeType, createDate, pageIndex, pageSize);
            if (result.status == "ok")
            {
                var orderslist = result.data.orders;
                if (orderslist.Count > 0)
                {
                    if (orderslist.Where(p => p.createdAt <= timestamp).Count() > 0)//如果首页存在比上次时间戳小或者等于的值
                    {
                        SaveLQToRedis(orderslist.Where(p => p.createdAt > timestamp).ToList(), key, symbol);
                        return;
                    }

                    var totalPage = result.data.totalPage;
                    if (totalPage > 0)
                    {
                        List<HUOBILQData.Order> data = new List<HUOBILQData.Order>();
                        data.AddRange(orderslist.Where(p => p.createdAt > timestamp).ToList());//添加首页

                        int i = 2;
                        while (true)
                        {
                            var ones = data.FirstOrDefault(p => p.createdAt <= 1616169600000);
                            var TIME = GZipDecompresser.GetTimeFromUnixTimestamps(data.Min(o => o.createdAt).ToString());
                            //Thread.Sleep(1000);
                            Console.WriteLine("huobi--"+TIME);
                            if (i > totalPage || ones != null)
                            {
                                break;
                            }

                            var alldata = GetResultData(url, contractCode, tradeType, createDate, i, pageSize);

                            if (alldata.status == "ok")
                            {
                                if (alldata.data.orders.Where(p => p.createdAt <= timestamp).Count() > 0)//存在比上次时间戳小或者等于的值
                                {
                                    data.AddRange(alldata.data.orders.Where(p => p.createdAt > timestamp).ToList());
                                    break;
                                }
                                else
                                {
                                    data.AddRange(alldata.data.orders);
                                }

                            }
                            i = i + 1;
                        }
                        SaveLQToRedis(data, key, symbol);
                    }
                }
                else
                {
                    Console.WriteLine("爆仓数据数量为0！");
                }
            }
        }


        private GetLiquidationOrdersResponse GetResultData(string urls, string contractCode, int tradeType, int createDate, int? pageIndex, int? pageSize)
        {
            // location
            //string location = $"/linear-swap-api/v1/swap_liquidation_orders?contract_code={contractCode}&&trade_type={tradeType}&&create_date={createDate}";

            string location = urls + $"&&trade_type={tradeType}&&create_date={createDate}";

            GetLiquidationOrdersResponse results = null;

            // option
            string option = null;
            if (pageIndex != null)
            {
                option += $"&&page_index={pageIndex}";
            }
            if (pageSize != null)
            {
                option += $"&&page_size={pageSize}";
            }
            if (option != null)
            {
                location += $"{option}";
            }
            string url = HUOBI_DEFAULT_HOST;
            url += location;

            //var result = HttpRequest.GetAsync<GetLiquidationOrdersResponse>(url);
            //return result;
            //dynamic list;
            //try
            //{
            //    list = ApiHelper.GetExtx(url);
            //}
            //catch (Exception e)
            //{
            //    list = ApiHelper.GetExtx(url);
            //}
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);

                    results = ((object)list).ToString().ToObject<GetLiquidationOrdersResponse>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取火币强平订单数据异常，异常信息：" + e.ToString());
                    Console.WriteLine("再次执行");
                }
                if (results != null)
                {
                    break;
                }

            }





            return results;
        }

        /// <summary>
        /// 保存大单数据
        /// </summary>
        /// <param name="list"></param>
        private void SaveLQToRedis(List<HUOBILQData.Order> list, string key, string symbol)
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
                o.amount = item.amount.ToString();
                o.timestamp = item.createdAt.ToString();
                o.times = GZipDecompresser.GetTimeFromUnixTimestamps(item.createdAt.ToString()).AddHours(8).ToString();

                o.Unit = "";
                o.utctime = GZipDecompresser.GetTimeFromUnixTimestamps(item.createdAt.ToString()).ToString();
                if (item.trade_turnover != null && item.trade_turnover > 0)
                {
                    o.vol = item.trade_turnover.ToString();
                }
                else
                {
                    item.trade_turnover = (item.price * item.amount);
                    o.vol = item.trade_turnover.ToString();

                    //item.trade_turnover = Convert.ToDecimal(o.vol);
                }
                o.side = item.direction;
                o.exchange = CommandEnum.RedisKey.huobi;
                o.pair = item.symbol;
                o.price = item.price.ToString();
                o.qty = item.volume.ToString();
                o.market = item.contractCode;
                o.contractcode= item.symbol;
                if (key == CommandEnum.RedisKey.huobi + coinswapkey)
                {
                    o.kinds = CommandEnum.RedisKey.PERP;
                    o.types = item.contractCode + "--币本位永续";
                }
                else if (key == CommandEnum.RedisKey.huobi + usdtswapkey)
                {
                    o.kinds = CommandEnum.RedisKey.PERP;
                    o.types = item.contractCode + "--USDT永续";
                }
                else
                {
                    o.types = item.contractCode + "--币本位交割";
                    o.kinds = CommandEnum.RedisKey.DELIVERY;
                }
                ////大单统计
                //if (item.trade_turnover >= 100000)
                //{
                //    RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                //}
                //大单统计
                if (item.trade_turnover >= 1000000)
                {
                    Console.WriteLine(o.ToJson());
                    RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                }
                //savelist.Add(o);

                //永续
                //public const string PERP = "PERP";
                ////交割
                //public const string DELIVERY = "DELIVERY";
                ////现货
                //public const string SPOT = "SPOT";

                //        //币本位永续获取时间戳key
                //private const string coinswapkey = "_swap_liquidation_orders";

                ////USDT永续获取时间戳key
                //private const string usdtswapkey = "_usdtswap_liquidation_orders";

                ////交割获取时间戳key
                //private const string deliverykey = "_delivery_liquidation_orders";
                RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));


                RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, o.ToJson());

            }

            //RedisHelper.Pushdata(savelist.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd HH"));
            var maxtimestamp = list.Max(a => a.createdAt).ToString();

            RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.LQTimeStamp, key + "_" + symbol, maxtimestamp);

            Console.WriteLine("结束爆仓数据统计：" + key + "_" + symbol);
            //保存数据到redis
            //更新时间戳值
        }




        public void runall()
        {
            //交割https://api.btcgateway.pro/api/v1/contract_liquidation_orders?symbol=BTC&trade_type=0&create_date=7
            //币本位永续https://api.btcgateway.pro/swap-api/v1/swap_liquidation_orders?contract_code=BTC-USD&trade_type=0&create_date=90
            //USDT永续https://api.btcgateway.pro/linear-swap-api/v1/swap_liquidation_orders?contract_code=BTC-USDT&trade_type=0&create_date=90"

            int a = 0;
            //USDT永续
            new Thread(() =>
            {
                List<string> lqlist = new List<string>();
                List<CommandEnum.USDTModel> usdtlist = new List<CommandEnum.USDTModel>();
                usdtlist = CommandEnum.PerpHUOBI.GetContract_sizeapi();
              
                lqlist = CommandEnum.ExchangeData.LQsymbolList;
                foreach (var item in usdtlist)
                {
                    //var symbol = item + "-USDT";
                    string key = CommandEnum.RedisKey.huobi + usdtswapkey;
                    string urls = $"/linear-swap-api/v1/swap_liquidation_orders?contract_code={item.contract_code}";
                    GetLQdata(item.symbol, urls, key);
                    //Console.WriteLine("a:"+a);
                }
                a = 1;
            }).Start();


            int b = 0;
            //币本位永续
            new Thread(() =>
            {
              
                List<string> lqlist = new List<string>();
                List<CommandEnum.USDTModel> usdlist = new List<CommandEnum.USDTModel>();
                usdlist = CommandEnum.PerpHUOBI.GetUSDContract_sizeapi();
                //lqlist = CommandEnum.ExchangeData.LQsymbolList;
                foreach (var item in usdlist)
                {
                    //var symbol = item + "-USD";
                    string key = CommandEnum.RedisKey.huobi + coinswapkey;
                    string urls = $"/swap-api/v1/swap_liquidation_orders?contract_code={item.contract_code}";
                    GetLQdata(item.symbol, urls, key);
                    //Console.WriteLine("b:" + b);
                }
                b = 1;
            }).Start();


            int c = 0;
            //交割
            new Thread(() =>
            {
               

                List<string> lqlist = new List<string>();
                lqlist = CommandEnum.ExchangeData.LQsymbolList;
                foreach (var item in lqlist)
                {
                    string key = CommandEnum.RedisKey.huobi + deliverykey;
                    string urls = $"/api/v1/contract_liquidation_orders?symbol={item}";
                    GetLQdata(item, urls, key);
                    //Console.WriteLine("c:" + c);
                }
                c = 1;
            }).Start();

            while (true)
            {
                if (a == 1 && b == 1 && c == 1)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }

        }


        //public void runtest()
        //{
        //    List<string> lqlist = new List<string>();
        //    lqlist = CommandEnum.ExchangeData.LQsymbolList;
        //    foreach (var item in lqlist)
        //    {
        //        string key = CommandEnum.RedisKey.huobi + deliverykey;
        //        string urls = $"/api/v1/contract_liquidation_orders?symbol={item}";
        //        GetLQdata(item, urls, key);
        //    }
        //}

        #endregion



        /// <summary>
        /// 获取费率
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="url"></param>
        /// <param name="key"></param>
        private List<HuobiFundingRate> GetFundingRate(string urls)
        {
            string url = HUOBI_DEFAULT_HOST;
            url += urls;

            List<HuobiFundingRate> results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);
                    results = ((object)list.data).ToString().ToList<HuobiFundingRate>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("批量获取火币费率数据异常，异常信息：" + e.ToString());
                    Console.WriteLine("再次执行");
                }
                if (results != null)
                {
                    break;
                }
            }
            return results;
        }


        /// <summary>
        /// get open interest
        /// 参数名称	是否必须	类型	描述	取值范围
        //symbol	true	string	品种代码	支持大小写，"BTC","ETH"...
        //contract_type	true	string	合约类型	当周:"this_week", 次周:"next_week", 当季:"quarter",次季:"next_quarter"
        //period	true	string	时间周期类型	1小时:"60min"，4小时:"4hour"，12小时:"12hour"，1天:"1day"
        //size	false	int	获取数量	默认为：48，取值范围 [1,200]
        //amount_type	true	int	计价单位	1:张，2:币
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        private GetOpenInterest GetOpenInterestAsync(string urls, string contractCode = null)
        {
            // location
            //string location = "/linear-swap-api/v1/swap_open_interest";
            string location = urls;
            // option
            string option = null;

            string period = "60min";
            //string period = "60min";

            string size = "200";

            string amount_type = "1";

            if (contractCode != null)
            {
                option += $"contract_code={contractCode}";
            }
            if (option != null)
            {
                location += $"?{option}";
            }

            string url = HUOBI_DEFAULT_HOST;
            url += location;

            GetOpenInterest results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);

                    results = ((object)list).ToString().ToObject<GetOpenInterest>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取火币持仓数据异常，异常信息：" + e.ToString());
                    Console.WriteLine("再次执行");
                }
                if (results != null)
                {
                    break;
                }

            }

            return results;
        }

        /// <summary>
        /// 根据使用symbol获取费率和持仓
        /// </summary>
        /// <param name="fundingurl">费率</param>
        /// <param name="openinteresturl">持仓</param>
        ///  <param name="isDelivery">是否交割</param>
        public  void GetFundingRateAndOpenInterest(string fundingurl, string openinteresturl, bool isDelivery, string type)
        {
            try
            {
                string typemessage = "";
                List<FundRate> FundRatelist = new List<FundRate>();
                List<OpenInterest> OpenInterestlist = new List<OpenInterest>();
                List<huoticket> tikcets = new List<huoticket>();
                List<string> symbollist = new List<string>();
                List<huobiKline> klist = new List<huobiKline>();

                GetOpenInterest openinserthistorylist = new GetOpenInterest();
                openinserthistorylist = (GetOpenInterestAsync(openinteresturl));


                if (isDelivery)
                {
                    //tikcets = Gethuobiticket(CommandEnum.RedisKey.DELIVERY);
                    symbollist = GetSymbollist(CommandEnum.RedisKey.DELIVERY).Split(",").ToList();
                    typemessage = "交割持仓信息";
                    klist = getprice(openinserthistorylist).Result;
                }
                else if (type == "USD")
                {
                    //symbollist = GetSymbollist("USD").Split(",").ToList();
                    typemessage = "币本位持仓费率信息";
                    tikcets = Gethuobiticket("USD");
                }
                else
                {
                    tikcets = Gethuobiticket("USDT");
                    //symbollist = GetSymbollist("USDT").Split(",").ToList();
                    typemessage = "U本位持仓费率信息";
                }
                //if (symbollist != null && symbollist.Count() > 0)
                //{
                //foreach (var item in symbollist)
                //{
                if (fundingurl != "")
                {
                    List<HuobiFundingRate> fundingratelist = new List<HuobiFundingRate>();
                    fundingratelist = GetFundingRate(fundingurl);
                    foreach (var funs in fundingratelist)
                    {
                        FundRate i = new FundRate();
                        i.market = funs.contract_code;
                        i.exchange = CommandEnum.RedisKey.huobi;
                        i.FundingRate = funs.funding_rate == null ? "" : funs.funding_rate.ToString();
                        i.symbol = funs.contract_code;
                        i.times = DateTime.Now.ToString();
                        i.NextFundingRate = funs.estimated_rate == null ? "" : funs.funding_rate.ToString();
                        FundRatelist.Add(i);
                    }
                }


                if (openinserthistorylist.data.Count > 0)
                {
                    foreach (var opens in openinserthistorylist.data)
                    {
                        if (opens.volume == 0)
                        {
                            continue;
                        }
                        OpenInterest i = new OpenInterest();
                        i.market = opens.contractCode;
                        i.exchange = CommandEnum.RedisKey.huobi;


                        i.symbol = opens.contractCode;
                        i.times = GZipDecompresser.GetTimeFromUnixTimestampthree(openinserthistorylist.ts.ToString()).ToString();
                        if (isDelivery)
                        {
                            //i.SumOpenInterestValue = opens.value;
                            i.amount = opens.volume;
                            i.SumOpenInterest = opens.amount;
                            i.SumOpenInterestValue = opens.avgvalue * opens.volume;
                            i.type = CommandEnum.RedisKey.coin;
                            i.desc = "持仓币，总价值为USDT";
                            i.kind = CommandEnum.RedisKey.DELIVERY;
                            if (klist.FirstOrDefault(p => p.symbol == opens.contractCode) != null)
                            {
                                i.price = klist.FirstOrDefault(p => p.symbol == opens.contractCode).close;
                            }


                        }
                        else if (type == "USD")
                        {
                            i.SumOpenInterestValue = opens.avgvalue * opens.volume;//使用24小时成交额除以总成交张数 * 持仓张数量
                            i.SumOpenInterest = opens.amount;
                            i.type = CommandEnum.RedisKey.coin;
                            i.desc = "持仓币，总价值为USDT";
                            if (tikcets.FirstOrDefault(p => p.contract_code == opens.contractCode) != null)
                            {
                                i.price = tikcets.FirstOrDefault(p => p.contract_code == opens.contractCode).close;
                            }
                        }
                        else
                        {
                            i.SumOpenInterestValue = opens.avgvalue * opens.volume;
                            i.SumOpenInterest = opens.amount;
                            i.type = CommandEnum.RedisKey.coin;
                            i.desc = "持仓币，总价值为USDT";
                            if (tikcets.FirstOrDefault(p => p.contract_code == opens.contractCode) != null)
                            {
                                i.price = tikcets.FirstOrDefault(p => p.contract_code == opens.contractCode).close;
                            }

                        }
                        i.volumeUsd24h = opens.trade_turnover;
                        i.coin = opens.amount;


                        OpenInterestlist.Add(i);
                    }
                }
                else
                {
                    Console.WriteLine("huobi获取持仓信息为空！！！");
                }
                //FundRate fate = new FundRate();
                //var fun = fundingratelist.FirstOrDefault();
                //var open = openinserthistorylist.FirstOrDefault();
                //ObjectUtil.MapTo(fun, fate);
                //ObjectUtil.MapTo(open, fate);
                //fate.exchange = CommandEnum.RedisKey.huobi;
                //list.Add(fate);
                //}
                //}
                //else
                //{
                //    Console.WriteLine("huobi 交易对不存在！！无法获取！");
                //}
                if (fundingurl != "")
                {
                    RedisHelper.Pushdata(FundRatelist.ToJson(), "11", CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd"));
                }
                RedisHelper.Pushdata(OpenInterestlist.ToJson(), "11", CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd"));
                Console.WriteLine("huobi 结束持仓和费率数据获取：" + CommandEnum.RedisKey.huobi + DateTime.Now.ToString() + typemessage);
            }
            catch (Exception e)
            {
                Console.WriteLine("获取持仓费率时出错：" + e.Message.ToString());
            }
        }

        public void runFundingRateAndOpenInterest()
        {
            //USDT永续
            Task.Run(() =>
            {

                string fundingurl = "/linear-swap-api/v1/swap_batch_funding_rate";
                string openinteresturl = $"/linear-swap-api/v1/swap_open_interest";
                GetFundingRateAndOpenInterest(fundingurl, openinteresturl, false, "USDT");
                //}
            });

            //币本位永续
            Task.Run(() =>
            {
                string fundingurl = "/swap-api/v1/swap_batch_funding_rate";
                string openinteresturl = "/swap-api/v1/swap_open_interest";
                GetFundingRateAndOpenInterest(fundingurl, openinteresturl, false, "USD");

            });

            //交割
            Task.Run(() =>
            {
                string fundingurl = "";
                string openinteresturl = $"/api/v1/contract_open_interest";
                GetFundingRateAndOpenInterest(fundingurl, openinteresturl, true, "USDT");
            });
        }

        public void runtest()
        {
            //string fundingurl = "/linear-swap-api/v1/swap_batch_funding_rate";
            //string openinteresturl = $"/linear-swap-api/v1/swap_open_interest";
            //GetFundingRateAndOpenInterest(fundingurl, openinteresturl, false, "USDT");


            //string fundingurl = "/linear-swap-api/v1/swap_batch_funding_rate";
            //string openinteresturl = $"/linear-swap-api/v1/swap_open_interest";
            //GetFundingRateAndOpenInterest(fundingurl, openinteresturl, false, "USDT");

            //string fundingurls = "/swap-api/v1/swap_batch_funding_rate";
            //string openinteresturls = "/swap-api/v1/swap_open_interest";
            //GetFundingRateAndOpenInterest(fundingurls, openinteresturls, false, "USD");



            string fundingurlq = "";
            string openinteresturlq = $"/api/v1/contract_open_interest";
            GetFundingRateAndOpenInterest(fundingurlq, openinteresturlq, true, "USDT");




        }


        /// <summary>
        /// 获取交易对
        /// </summary>
        /// //TYPE: USD/USDT
        /// <returns></returns>
        public static string GetSymbollist(string type)
        {
            string list = null;
            if (type == CommandEnum.RedisKey.DELIVERY)
            {
                list = GetDelivery();
            }
            else if (type == "USD")
            {
                var lists = CommandEnum.PerpHUOBI.GetUSDContract_sizeapi();
                list = string.Join(",", lists.Select(p => p.contract_code).ToList());
            }
            else
            {
                var lists = CommandEnum.PerpHUOBI.GetContract_sizeapi();
                list = string.Join(",", lists.Select(p => p.contract_code).ToList());
            }

            return list;

        }

        /// <summary>
        /// 获取交割交易对
        /// </summary>
        /// <returns></returns>
        private static string GetDelivery()
        {
            string symbol = "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV,ADA,FIL,ETC";

            string[] arr1 = symbol.Split(',');

            List<string> result = new List<string>();
            string results = string.Empty;


            foreach (var item in arr1)
            {
                var cw = $"{item}_CW";
                var nw = $"{item}_NW";
                var cq = $"{item}_CQ";
                var nq = $"{item}_NQ";
                result.Add(cw);
                result.Add(nw);
                result.Add(cq);
                result.Add(nq);
            }

            results = string.Join(",", result.Select(p => p).ToList());
            return results;
        }


        /// <summary>
        /// 批量获取行情信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<huoticket> Gethuobiticket(string type)
        {
            string deliver = "/market/detail/batch_merged";
            string usd = "/swap-ex/market/detail/batch_merged";
            string usdt = "/linear-swap-ex/market/detail/batch_merged";
            string url = HUOBI_DEFAULT_HOST;
            if (type == CommandEnum.RedisKey.DELIVERY)
            {
                url = url + deliver;
            }
            else
            {
                if (type == "USD")
                {
                    url = url + usd;
                }
                else
                {
                    url = url + usdt;
                }

            
            }          
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list.ticks).ToString().ToList<huoticket>();
                return results;
            
        }


        /// <summary>
        /// 获取K线单价
        /// </summary>
        /// <param name="symbollist"></param>
        /// <returns></returns>
        public async  Task<List<huobiKline>> getprice(GetOpenInterest symbollist)
        {
            List<huobiKline> klist = new List<huobiKline>();
            string url = HUOBI_DEFAULT_HOST;

            foreach (var item in symbollist.data)
            {
                string urls = $"/market/history/kline?period=1min&size=5&symbol={item.contractCode}";

                var list = await ApiHelper.GetAsync(url+ urls);
                var results = ((object)list.data).ToString().ToList<huobiKline>().FirstOrDefault();
                results.symbol = item.contractCode;
                klist.Add(results);
            }       
            return klist;
        }

    }
}
