using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CoinWin.DataGeneration
{
   public class QuenProcess
    {
        ///每小时,4小时，24小时 的成交量,爆仓，对应的交易对，区分永续和交割
        //
        public void CalcPere()
        {
            
       
            while (true)
            {
              var count=  RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.PERPQueueList);
                if(count< 50000)
                {
                    Console.WriteLine("永续数量不足1W，暂不汇总计算！====="+DateTime.Now.ToString());
                    Thread.Sleep(60000);
                    continue;
                }

                List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
                while (true)
                {
                 
                        //Console.WriteLine("开始时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                        list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.PERPQueueList, 0).ToList<UPermanentFuturesModel>());
 
                    if (list.Count > 50000)
                    {
                        break;
                    }
                }



                var end = Convert.ToDateTime(list.Max(p => p.times));
                var start = Convert.ToDateTime(list.Min(p => p.times));
                var timelist= TimeCore.GetStartAndEndTimeContent(start, end);
                if (timelist != null && timelist.Count > 0)
                {                 
                        if (list!= null && list.Count() > 0)
                        {
 
                        ProcessAllPerpHourAndDate(list, timelist, CommandEnum.RedisKey.PERPRealDeal);//全网币种小时汇总 全网币种天汇总 
                        ProcessALLCoinAllExchageEveryHour(list,"PERP", timelist);//全网逐一交易所逐一币种每小时
                        }

                }
            }
        }

        public void CalcDeliver()
        {
            while (true)
            {
                var count = RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.DeliverQueueList);
                if (count < 50000)
                {
                    Console.WriteLine("交割数量不足1W，暂不汇总计算！---"+DateTime.Now.ToString());
                    Thread.Sleep(60000);
                    continue;
                }
                List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
       
                while (true)
                {

                    list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.DeliverQueueList, 0).ToList<UPermanentFuturesModel>());
        
                    if (list.Count > 50000)
                    {
                        break;
                    }
                }

                var end = Convert.ToDateTime(list.Max(p => p.times));
                var start = Convert.ToDateTime(list.Min(p => p.times));
                var timelist = TimeCore.GetStartAndEndTimeContent(start, end);
                if (timelist != null && timelist.Count > 0)
                {           
                    if (list != null && list.Count() > 0)
                    {
                        ProcessAllPerpHourAndDate(list, timelist, CommandEnum.RedisKey.DeliverRealDeal);
                        ProcessALLCoinAllExchageEveryHour(list, "Deliver", timelist);
                    }
                }             
            }
        }

        public void CalcSpot()
        {
            while (true)
            {
                var count = RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.SpotQueueList);
                if (count < 10000)
                {
                    Console.WriteLine("现货数量不足1W，暂不汇总计算！---" + DateTime.Now.ToString());
                    Thread.Sleep(60000);
                    continue;
                }
                List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
                 


                while (true)
                {
                    list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.SpotQueueList, 0).ToList<UPermanentFuturesModel>());
 
                    if (list.Count > 50000)
                    {
                        break;
                    }
                }

                var end = Convert.ToDateTime(list.Max(p => p.times));
                var start = Convert.ToDateTime(list.Min(p => p.times));
                var timelist = TimeCore.GetStartAndEndTimeContent(start, end);
                if (timelist != null && timelist.Count > 0)
                {
                    if (list != null && list.Count() > 0)
                    {
                        ProcessAllPerpHourAndDate(list, timelist, CommandEnum.RedisKey.SPOTRealDeal);

                        ProcessALLCoinAllExchageEveryHour(list, "SPOT", timelist);
                    }
                }
            }
        }



        /// <summary>
        /// 永续 交割 全网 币种 小时and 天 汇总
        /// </summary>
        /// <param name="data"></param>
        /// <param name="times"></param>
        /// <param name="key">永续or交割</param>CommandEnum.RedisKey.DeliverRealDeal or    CommandEnum.RedisKey.PERPRealDeal
        public void ProcessAllPerpHourAndDate(List<UPermanentFuturesModel> data, List<DateTime> times,string key)
        {
            List<PermanentFutures> savelist = new List<PermanentFutures>();
          
            List<string> grouplist = new List<string>();


            if (data != null && data.Count > 0)
            {
                foreach (var dt in times)//每一分钟
                {

                    var resultslist = data.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();

                    grouplist = resultslist.GroupBy(p => p.contractcode).Select(a => a.Key).ToList();

                    if (resultslist != null && resultslist.Count > 0)
                    {
                        foreach (var sy in grouplist)//bizhong
                        {
                            //if (sy == "BTC")
                            //{ 
                            
                            //}

                            var Modellist = resultslist.Where(p => p.contractcode == sy).OrderBy(p => p.times).ToList();
                            if (Modellist.Count() > 0)
                            {
                                PermanentFutures df = new PermanentFutures();
                                df.contractcode = sy;
                                df.price = Modellist.Average(p => p.price);
                                df.open = Modellist.FirstOrDefault().price;
                                df.close = Modellist.Last().price;
                                df.low = Modellist.Min(p => p.price);
                                df.high = Modellist.Max(p => p.price);


                          
                                df.vol = Modellist.Sum(p => p.vol);
                                df.vol_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.vol);
                                df.vol_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.vol);

                                df.qty = Modellist.Sum(p => p.qty);
                                df.qty_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.qty);
                                df.qty_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.qty);
                                df.Unit = Modellist.FirstOrDefault().Unit == null ? "" : Modellist.FirstOrDefault().Unit;
                                df.pair = string.Join(",", Modellist.GroupBy(p => p.pair).Select(a => a.Key).ToList());
                                df.count = Modellist.Count();
                                df.count_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Count();
                                df.count_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Count();
                                df.exchange = string.Join(",", Modellist.GroupBy(p => p.exchange).Select(a => a.Key).ToList());


                                df.liquidation = 0;
                                df.liquidation_buy = 0;
                                df.liquidation_sell = 0;
                                df.basis = "0";
                                df.market = string.Join(",", Modellist.GroupBy(p => p.market).Select(a => a.Key).ToList());
                                df.timestamp = Modellist.FirstOrDefault().timestamp;
                                df.types = string.Join(",", Modellist.GroupBy(p => p.types).Select(a => a.Key).ToList());
                                df.utime = Modellist.FirstOrDefault().utctime;
                                df.Times = Convert.ToDateTime(Modellist.Max(a => a.times));
                                df.timecode = dt.ToString("yyyy-MM-dd HH");

                                savelist.Add(df);
                            }
                        }
                    }

                }

            }


            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key);
            var daytradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key + times.FirstOrDefault().ToString("yyyy-MM-dd"));
          
            foreach (var item in savelist)
            {
                saveandupdatenew(item, key, item.timecode + "_" + item.contractcode, hourtradeMasterlist);
                saveandupdatenew(item, key + item.Times.ToString("yyyy-MM-dd"), item.Times.ToString("yyyy-MM-dd") + "_" + item.contractcode, daytradeMasterlist);
            }



        }

        private  void saveandupdatenew(PermanentFutures item,  string key,string code, Dictionary<string, PermanentFutures> dts)
        {
            PermanentFutures values = null;

            bool flag = dts.TryGetValue(code, out values);


            if (values!=null)
            {
                //var values = tradedata.ToObject<PermanentFutures>();
                values.Times = item.Times;
                values.close = item.close;
                values.count = values.count + item.count;
                values.count_buy = values.count_buy + item.count_buy;
                values.count_sell = values.count_sell + item.count_sell;
                values.exchange = item.exchange;

                values.qty = values.qty + item.qty;
                values.qty_buy = values.qty_buy + item.qty_buy;
                values.qty_sell = values.qty_sell + item.qty_sell;

                values.vol = values.vol + item.vol;
          
                values.vol_buy = values.vol_buy + item.vol_buy;
                values.vol_sell = values.vol_sell + item.vol_sell;

                values.price = item.price;
                //o.open = item.open;
                values.close = item.close;
                values.low = item.low < values.low ? item.low : values.low;
                values.high = item.high > values.high ? item.high : values.high;

                values.pair = item.pair;
                values.market = item.market;
                //更新
                RedisHelper.SetOrUpdateHash(key, code, values.ToJson());
            }
            else
            {
                RedisHelper.SetOrUpdateHash(key, code, item.ToJson());
            }
        }


        /// <summary>
        /// ALL Coin ALL Exchange Every Hour and holddate
        /// </summary>
        /// <param name="data"></param>
        public void ProcessALLCoinAllExchageEveryHour(List<UPermanentFuturesModel> data,string type,List<DateTime>times)
        {
            //string key = type+"ALLCoinALLExchange" +DateTime.Now.ToString("yyyy-MM-dd");
            string key=type+ "ALLCoinALLExchange"+times.FirstOrDefault().ToString("yyyy-MM-dd");
            var exchagelist = CommandEnum.ExchangeData.ExchangeList;
            List<PermanentFutures> savelist = new List<PermanentFutures>();

            List<string> grouplist = new List<string>();

            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key);
            //var daytradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key + times.FirstOrDefault().ToString("yyyy-MM-dd"));



            foreach (var exchange in exchagelist)
            {
                var calcdata = data.Where(p => p.exchange == exchange).ToList();
                if (calcdata != null && calcdata.Count > 0)
                {
                    foreach (var dt in times)//每一分钟
                    {
                        key= type + "ALLCoinALLExchange" + dt.ToString("yyyy-MM-dd");

                        var resultslist = calcdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();
                        grouplist = resultslist.GroupBy(p => p.market).Select(a => a.Key).ToList();


                        if (resultslist != null && resultslist.Count > 0)
                        {
                            foreach (var sy in grouplist)//bizhong
                            {

                                List<UPermanentFuturesModel> Modellist = new List<UPermanentFuturesModel>();
                                Modellist = resultslist.Where(p => p.market == sy).OrderBy(p => p.times).ToList();
                                saveupermanent(savelist, sy, Modellist,dt);
                            }
                        }
                    }


                }
                foreach (var item in savelist)
                {
                    saveandupdatenew(item, key, item.exchange + "_" + item.timecode + "_" + item.market, hourtradeMasterlist);
                    saveandupdatenew(item, key, item.exchange + "_" + item.Times.ToString("yyyy-MM-dd") + "_" + item.market, hourtradeMasterlist);
                }

                savelist = new List<PermanentFutures>();
            }

             
        }
        private static void saveupermanent(List<PermanentFutures> savelist, string sy, List<UPermanentFuturesModel> Modellist,DateTime dt)
        {
            if (Modellist.Count() > 0)
            {
                PermanentFutures df = new PermanentFutures();
                df.contractcode = Modellist.FirstOrDefault().contractcode; ;
                df.price = Modellist.Average(p => p.price);
                df.open = Modellist.FirstOrDefault().price;
                df.close = Modellist.Last().price;
                df.low = Modellist.Min(p => p.price);
                df.high = Modellist.Max(p => p.price);

                //var buyslist = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY);


                df.vol = Modellist.Sum(p => p.vol);
                df.vol_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.vol);
                df.vol_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.vol);

                df.qty = Modellist.Sum(p => p.qty);
                df.qty_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.qty);
                df.qty_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.qty);
                df.Unit = Modellist.FirstOrDefault().Unit == null ? "" : Modellist.FirstOrDefault().Unit;
                df.pair = string.Join(",", Modellist.GroupBy(p => p.pair).Select(a => a.Key).ToList());
                df.count = Modellist.Count();
                df.count_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Count();
                df.count_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Count();
                df.exchange = string.Join(",", Modellist.GroupBy(p => p.exchange).Select(a => a.Key).ToList());


                df.liquidation = 0;
                df.liquidation_buy = 0;
                df.liquidation_sell = 0;
                df.basis = "0";
                df.market = string.Join(",", Modellist.GroupBy(p => p.market).Select(a => a.Key).ToList());
                df.timestamp = Modellist.FirstOrDefault().timestamp;
                df.types = string.Join(",", Modellist.GroupBy(p => p.types).Select(a => a.Key).ToList());
                df.utime = Modellist.FirstOrDefault().utctime;
                df.Times = Convert.ToDateTime(Modellist.Max(a => a.times));

                df.timecode = dt.ToString("yyyy-MM-dd HH");

                savelist.Add(df);
            }
        }


    }
}

 