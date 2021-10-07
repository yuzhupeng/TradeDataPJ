using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GetTradeHistoryData
{
    public class BybitLQhelper
    {
        //curl https://api.bybit.com/v2/public/liq-records?symbol=BTCUSD
        // private const string BYBIT_DEFAULT_HOST = "https://api.bybit.com/v2/public/liq-records?symbol=";
        //币本位永续url
        private const string BYBIT_DEFAULT_HOST = "https://api.bytick.com/v2/public/liq-records?symbol=";


        private const string coinkey = "Bybit_liquidation_orders";


        private void GetLQdata(string symbol, string key)
        {
            var times = RedisHelper.GetHash(CommandEnum.RedisKey.LQTimeStamp, key + "_" + symbol);
            //取上次时间戳

            long timestamp = 1624204800000;

            if (times != "")
            {
                timestamp = Convert.ToInt64(times) > 1624204800000 ? Convert.ToInt64(times) : 1624204800000;
            }

            // from    false   integer 起始ID. 默认: 返回最新数据
            int limit = 1000;
            long start_time = 0;
            long end_time = 0;


            start_time = timestamp;
          
            end_time = GZipDecompresser.GetUnixTimestamp(DateTime.Now);

 

            //获取初始数据,获取页数数据
            BybitLQ result = GetResultData(symbol, limit, start_time, end_time);


            if (result.ret_msg == "OK")
            {
                var orderslist = result.result;

   

                if (orderslist.Count > 0)
                {
                    if (orderslist.Where(p => p.time <= start_time).Count() > 0)//如果首页存在比上次时间戳小或者等于的值
                    {
                        SaveLQToRedisBybit(orderslist.Where(p => p.time > timestamp).ToList(), key, symbol);
                        return;
                    }

                    //var totalPage = result.data.totalPage;
                    long mintimestamp = orderslist.Min(p => p.time);


                  
                    List<BybitLQData> data = new List<BybitLQData>();
                    data.AddRange(orderslist.Where(p => p.time > start_time).ToList());//添加首页
                    BybitLQ count = new BybitLQ();
               
                    long nextmin = 0;
                    long timess = 0;
                    while (true)
                    {
                        

                        if (count.result != null&&(mintimestamp <= start_time|| count.result.Count<1000))
                        {
                            break;
                        }

                        count = GetResultData(symbol, limit, start_time, mintimestamp);


                        DateTime dtmin = GZipDecompresser.GetTimeFromUnixTimestamps(mintimestamp.ToString()).AddHours(8);

                        Console.WriteLine("bybit-时间戳："+dtmin);

                        // nextmin = count.result.Min(p => p.time);
                        //long maxs = count.result.Max(p => p.time);
                        // DateTime dtsart = GZipDecompresser.GetTimeFromUnixTimestamps(start_time.ToString()).AddHours(8);
                        // DateTime dtmin = GZipDecompresser.GetTimeFromUnixTimestamps(nextmin.ToString()).AddHours(8);
                        // DateTime dtmax = GZipDecompresser.GetTimeFromUnixTimestamps(maxs.ToString()).AddHours(8);
                        // Console.WriteLine(dtmin);
                        // Console.WriteLine(dtmax);

                        if (count.ret_msg == "OK")
                        {
                            //data.AddRange(count.result);
                            data.AddRange(count.result.Where(p => p.time > start_time).ToList());
                        }
                        if (count.result.Count > 0)
                        {
                            mintimestamp = count.result.Min(p => p.time);
                        }

                        //if (alldata.status == "ok")
                        //{
                        //    if (alldata.data.orders.Where(p => p.createdAt <= timestamp).Count() > 0)//存在比上次时间戳小或者等于的值
                        //    {
                        //        data.AddRange(alldata.data.orders.Where(p => p.createdAt > timestamp).ToList());
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        data.AddRange(alldata.data.orders);
                        //    }

                        //}
                        //i = i + 1;
                    }
                    SaveLQToRedisBybit(data, key, symbol);
                    //}
                }
                else
                {
                    Console.WriteLine("爆仓数据数量为0！");
                }
            }
        }



        private BybitLQ GetResultData(string symbol, int limit, long start_time, long end_time)
        {
            // location
            //string location = $"/linear-swap-api/v1/swap_liquidation_orders?contract_code={contractCode}&&trade_type={tradeType}&&create_date={createDate}";

            string location = symbol + $"&&limit={limit}&&start_time={start_time}&&end_time={end_time}";

            BybitLQ results = null;


            string url = BYBIT_DEFAULT_HOST;
            url += location;

            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);

                    results = ((object)list).ToString().ToObject<BybitLQ>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取bybit强平订单数据异常，异常信息：" + e.ToString());
                    Console.WriteLine("再次执行");
                }
                if (results != null)
                {
                    break;
                }

            }
            return results;
        }


        public void runtest()
        {
            int a = 0;
            new Thread(() =>
            {
                List<string> lqlist = new List<string>();
                lqlist = CommandEnum.ExchangeData.BybitAlllist.Split(",").ToList();
                foreach (var item in lqlist)
                {
                    string key = coinkey;
                    string urls = "";
                    GetLQdata(item, key);
                }
                a = 1;
            }).Start();

            while (true)
            {
                if (a == 1)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }

        }

        /// <summary>
        /// 保存大单数据
        /// </summary>
        /// <param name="list"></param>
        private void SaveLQToRedisBybit(List<BybitLQData> list, string key, string symbol)
        {
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("key:"+ symbol + key + "--bybit首页爆仓数据数量为0！" + DateTime.Now.ToString());
                return;
            }
            else
            {
                Console.WriteLine("key：" +symbol + key + "--bybit爆仓数量：" + list.Count() +"--------"+ DateTime.Now.ToString());
            }

            foreach (var item in list)
            {
                Liquidation o = new Liquidation();
                o.amount = item.qty.ToString();
                o.timestamp = item.time.ToString();
                o.times = GZipDecompresser.GetTimeFromUnixTimestamps(item.time.ToString()).AddHours(8).ToString();

                o.Unit = "";
                o.utctime = GZipDecompresser.GetTimeFromUnixTimestamps(item.time.ToString()).ToString();


                if (item.symbol.ToUpper().Contains("USDT"))
                {
                    o.vol = (item.price * item.qty).ToString();
                   // o.vol = item.trade_turnover.ToString();
                }
                else
                {
                  
                    o.vol = item.qty.ToString();
                    //item.trade_turnover = Convert.ToDecimal(o.vol);
                }
                o.side = item.side;
                o.exchange = CommandEnum.RedisKey.bybit;
                o.pair = item.symbol;
                o.price = item.price.ToString();
                o.qty = item.qty.ToString();
                o.market = item.symbol;
                if (item.symbol.ToUpper().Contains("USDT"))
                {
                    o.kinds = CommandEnum.RedisKey.PERP;
                    o.types = item.symbol + "--币本位USDT永续";
                    o.Unit = "币";
                    o.contractcode= item.symbol.Replace("USDT", "");

                }
                else 
                {
                    o.kinds = CommandEnum.RedisKey.PERP;
                    o.types = item.symbol + "--反向USD永续";
                    o.Unit = "USDT";
                    o.contractcode = item.symbol.Replace("USD", "");
                }
           
                ////大单统计
                //if (Convert.ToDecimal(o.vol )>= 100000)
                //{
                //    RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                //}
                //大单统计
                if (Convert.ToDecimal(o.vol) >= 1000000)
                {
                    Console.WriteLine(o.ToJson());
                    RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                }

                RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));


                RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, o.ToJson());

            }


            var maxtimestamp = list.Max(a => a.time).ToString();

            RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.LQTimeStamp, key + "_" + symbol, maxtimestamp);

            Console.WriteLine("bybit结束爆仓数据统计：" + key + "_" + symbol);
            //保存数据到redis
            //更新时间戳值



        }
    }
}
