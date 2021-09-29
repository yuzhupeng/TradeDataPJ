using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// 2020-03-26
    /// </summary>
    public class QuenProcessregion
    {
        ///每小时,4小时，24小时 的成交量,爆仓，对应的交易对，区分永续和交割
        //
        public void CalcPere()
        {


            while (true)
            {
                var count = RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.PERPQueueList);
                if (count < 50000)
                {
                    Console.WriteLine("永续数量不足1W，暂不汇总计算！=====" + DateTime.Now.ToString());
                    Thread.Sleep(60000);
                    continue;
                }

                List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
                while (true)
                {

                    //Console.WriteLine("开始时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                    list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.PERPQueueList, 1000000).ToList<UPermanentFuturesModel>());


                    //s.Add(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.PERPQueueList, 1000));
                    //Console.WriteLine("结束时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                    //Console.WriteLine(s.Count);
                    if (list.Count > 50000)
                    {
                        break;
                    }
                }


                //var exchagelist = CommandEnum.ExchangeData.ExchangeList;

                var end = Convert.ToDateTime(list.Max(p => p.times));
                var start = Convert.ToDateTime(list.Min(p => p.times));

                var timelist = TimeCore.GetStartAndEndTimeContent(start, end);
                if (timelist != null && timelist.Count > 0)
                {
                    if (list != null && list.Count() > 0)
                    {
                        //ProcessAllPerp(list,timelist);//全网币种小时汇总 
                        //ProcessAll(list, CommandEnum.RedisKey.PERPRealDeal + DateTime.Now.ToString("yyyy-MM-dd"));//全网币种天汇总 
                        ProcessAllPerpHourAndDate(list, timelist, CommandEnum.RedisKey.PERPRealDeal);
                        ProcessALLCoinAllExchageEveryHour(list, "PERP", timelist);//全网逐一交易所逐一币种每小时
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
                    Console.WriteLine("交割数量不足1W，暂不汇总计算！---" + DateTime.Now.ToString());
                    Thread.Sleep(60000);
                    continue;
                }
                List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
                List<Liquidation> lqlist = new List<Liquidation>();


                while (true)
                {
                    //Console.WriteLine("开始时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                    list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.DeliverQueueList, 1000000).ToList<UPermanentFuturesModel>());

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
                        //ProcessAllDeliver(list, timelist);
                        //ProcessAll(list, CommandEnum.RedisKey.DeliverRealDeal + DateTime.Now.ToString("yyyy-MM-dd"));

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
                if (count < 50000)
                {
                    Console.WriteLine("现货数量不足1W，暂不汇总计算！---" + DateTime.Now.ToString());
                    Thread.Sleep(60000);
                    continue;
                }
                List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();



                while (true)
                {
                    //Console.WriteLine("开始时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                    list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.SpotQueueList, 1000000).ToList<UPermanentFuturesModel>());

                    if (list.Count > 500000)
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
                        //ProcessAllSpot(list, timelist);
                        //ProcessAll(list, CommandEnum.RedisKey.SPOTRealDeal + DateTime.Now.ToString("yyyy-MM-dd"));

                        ProcessAllPerpHourAndDate(list, timelist, CommandEnum.RedisKey.SPOTRealDeal);

                        ProcessALLCoinAllExchageEveryHour(list, "SPOT", timelist);
                    }
                }
            }
        }



        /// <summary>
        ///未使用-- 交割循环币对处理全网
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
        /// <param name="dtlist"></param>
        /// <param name="exchange"></param>
        public void ProcessAllDeliver(List<UPermanentFuturesModel> data, List<DateTime> times)
        {
            List<DeliveryFutures> savelist = new List<DeliveryFutures>();
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
                            var Modellist = resultslist.Where(p => p.contractcode == sy).OrderBy(p => p.times).ToList();
                            if (Modellist.Count() > 0)
                            {
                                DeliveryFutures df = new DeliveryFutures();
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
            foreach (var item in savelist)
            {
                //取
                var tradedata = RedisHelper.GetHash(CommandEnum.RedisKey.DeliverRealDeal, item.timecode + "_" + item.contractcode);
                if (tradedata != null && tradedata != "")
                {
                    var o = tradedata.ToObject<DeliveryFutures>();
                    o.Times = item.Times;
                    o.close = item.close;
                    o.count = o.count + item.count;
                    o.count_buy = o.count_buy + item.count_buy;
                    o.count_sell = o.count_sell + item.count_sell;
                    o.exchange = item.exchange;

                    o.qty = o.qty + item.qty;
                    o.qty_buy = o.qty_buy + item.qty_buy;
                    o.qty_sell = o.qty_sell + item.qty_sell;

                    o.vol = o.vol + item.vol;
                    o.vol_buy = o.vol_buy + item.vol_buy;
                    o.vol_sell = o.vol_sell + item.vol_sell;

                    o.price = item.price;
                    o.open = item.open;
                    o.close = item.close;
                    o.low = item.low < o.low ? item.low : o.low;
                    o.high = item.high > o.high ? item.high : o.high;



                    //更新
                    RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.DeliverRealDeal, item.timecode + "_" + item.contractcode, o.ToJson());
                }
                else
                {
                    RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.DeliverRealDeal, item.timecode + "_" + item.contractcode, item.ToJson());
                }
            }
        }
        /// <summary>
        /// 未使用--现货循环币对处理全网
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
        /// <param name="dtlist"></param>
        /// <param name="exchange"></param>
        public void ProcessAllSpot(List<UPermanentFuturesModel> data, List<DateTime> times)
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
                                df.types = Modellist.FirstOrDefault().types;
                                df.utime = Modellist.FirstOrDefault().utctime;
                                df.Times = Convert.ToDateTime(Modellist.Max(a => a.times));
                                df.timecode = dt.ToString("yyyy-MM-dd HH");

                                savelist.Add(df);
                            }
                        }
                    }

                }

            }
            foreach (var item in savelist)
            {
                //取
                var tradedata = RedisHelper.GetHash(CommandEnum.RedisKey.SPOTRealDeal, item.timecode + "_" + item.contractcode);
                if (tradedata != null && tradedata != "")
                {
                    var o = tradedata.ToObject<PermanentFutures>();
                    o.Times = item.Times;
                    o.close = item.close;
                    o.count = o.count + item.count;
                    o.count_buy = o.count_buy + item.count_buy;
                    o.count_sell = o.count_sell + item.count_sell;
                    o.exchange = item.exchange;

                    o.qty = o.qty + item.qty;
                    o.qty_buy = o.qty_buy + item.qty_buy;
                    o.qty_sell = o.qty_sell + item.qty_sell;

                    o.vol = o.vol + item.vol;
                    o.vol_buy = o.vol_buy + item.vol_buy;
                    o.vol_sell = o.vol_sell + item.vol_sell;

                    o.price = item.price;
                    o.open = item.open;
                    o.close = item.close;
                    o.low = item.low < o.low ? item.low : o.low;
                    o.high = item.high > o.high ? item.high : o.high;



                    //更新
                    RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.SPOTRealDeal, item.timecode + "_" + item.contractcode, o.ToJson());
                }
                else
                {
                    RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.SPOTRealDeal, item.timecode + "_" + item.contractcode, item.ToJson());
                }
            }



        }
        /// <summary>
        /// 未使用--循环处理 整天
        /// </summary>
        /// <param name="data">数据 交割 永续 现货</param>
        /// <param name="key">存储的key</param>
        public void ProcessAll(List<UPermanentFuturesModel> data, string key)
        {
            List<PermanentFutures> savelist = new List<PermanentFutures>();

            List<string> grouplist = new List<string>();


            if (data != null && data.Count > 0)
            {
                var resultslist = data.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd")).ToList();

                grouplist = resultslist.GroupBy(p => p.contractcode).Select(a => a.Key).ToList();

                if (resultslist != null && resultslist.Count > 0)
                {
                    foreach (var sy in grouplist)//bizhong
                    {
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
                            df.pair = Modellist.FirstOrDefault().contractcode;
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

                            df.timecode = DateTime.Now.ToString("yyyy-MM-dd");

                            savelist.Add(df);
                        }
                    }
                }



            }
            foreach (var item in savelist)
            {
                //取
                var tradedata = RedisHelper.GetHash(key, item.timecode + "_" + item.contractcode);
                if (tradedata != null && tradedata != "")
                {
                    var o = tradedata.ToObject<PermanentFutures>();
                    o.Times = item.Times;
                    o.close = item.close;
                    o.count = o.count + item.count;
                    o.count_buy = o.count_buy + item.count_buy;
                    o.count_sell = o.count_sell + item.count_sell;
                    o.exchange = item.exchange;

                    o.qty = o.qty + item.qty;
                    o.qty_buy = o.qty_buy + item.qty_buy;
                    o.qty_sell = o.qty_sell + item.qty_sell;

                    o.vol = o.vol + item.vol;
                    o.vol_buy = o.vol_buy + item.vol_buy;
                    o.vol_sell = o.vol_sell + item.vol_sell;

                    o.price = item.price;
                    o.open = item.open;
                    o.close = item.close;
                    o.low = item.low < o.low ? item.low : o.low;
                    o.high = item.high > o.high ? item.high : o.high;



                    //更新
                    RedisHelper.SetOrUpdateHash(key, item.timecode + "_" + item.contractcode, o.ToJson());
                }
                else
                {
                    RedisHelper.SetOrUpdateHash(key, item.timecode + "_" + item.contractcode, item.ToJson());
                }
            }



        }
        /// <summary>
        /// 未使用--永续 全网 币种 小时汇总
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
        /// <param name="dtlist"></param>
        /// <param name="exchange"></param>
        public void ProcessAllPerp(List<UPermanentFuturesModel> data, List<DateTime> times)
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
            foreach (var item in savelist)
            {
                //取
                var tradedata = RedisHelper.GetHash(CommandEnum.RedisKey.PERPRealDeal, item.timecode + "_" + item.contractcode);
                if (tradedata != null && tradedata != "")
                {
                    var o = tradedata.ToObject<PermanentFutures>();
                    o.Times = item.Times;
                    o.close = item.close;
                    o.count = o.count + item.count;
                    o.count_buy = o.count_buy + item.count_buy;
                    o.count_sell = o.count_sell + item.count_sell;
                    o.exchange = item.exchange;

                    o.qty = o.qty + item.qty;
                    o.qty_buy = o.qty_buy + item.qty_buy;
                    o.qty_sell = o.qty_sell + item.qty_sell;

                    o.vol = o.vol + item.vol;
                    o.vol_buy = o.vol_buy + item.vol_buy;
                    o.vol_sell = o.vol_sell + item.vol_sell;

                    o.price = item.price;
                    o.open = item.open;
                    o.close = item.close;
                    o.low = item.low < o.low ? item.low : o.low;
                    o.high = item.high > o.high ? item.high : o.high;



                    //更新
                    RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.PERPRealDeal, item.timecode + "_" + item.contractcode, o.ToJson());
                }
                else
                {
                    RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.PERPRealDeal, item.timecode + "_" + item.contractcode, item.ToJson());
                }
            }



        }





        /// <summary>
        /// 永续 交割 全网 币种 小时and 天 汇总
        /// </summary>
        /// <param name="data"></param>
        /// <param name="times"></param>
        /// <param name="key">永续or交割</param>CommandEnum.RedisKey.DeliverRealDeal or    CommandEnum.RedisKey.PERPRealDeal
        public void ProcessAllPerpHourAndDate(List<UPermanentFuturesModel> data, List<DateTime> times, string key)
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


            //var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key);
            //var daytradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key + times.FirstOrDefault().ToString("yyyy-MM-dd"));

            foreach (var item in savelist)
            {

                var hourdata = RedisHelper.GetHash(key, item.timecode + "_" + item.contractcode);
                saveandupdate(item, hourdata, key, item.timecode + "_" + item.contractcode);


                var datedata = RedisHelper.GetHash(key + item.Times.ToString("yyyy-MM-dd"), item.Times.ToString("yyyy-MM-dd") + "_" + item.contractcode);
                saveandupdate(item, datedata, key + item.Times.ToString("yyyy-MM-dd"), item.Times.ToString("yyyy-MM-dd") + "_" + item.contractcode);

            }



        }
        private static void saveandupdate(PermanentFutures item, string tradedata, string key, string code)
        {
            if (tradedata != null && tradedata != "")
            {
                var o = tradedata.ToObject<PermanentFutures>();
                o.Times = item.Times;
                o.close = item.close;
                o.count = o.count + item.count;
                o.count_buy = o.count_buy + item.count_buy;
                o.count_sell = o.count_sell + item.count_sell;
                o.exchange = item.exchange;

                o.qty = o.qty + item.qty;
                o.qty_buy = o.qty_buy + item.qty_buy;
                o.qty_sell = o.qty_sell + item.qty_sell;

                o.vol = o.vol + item.vol;
                o.vol_buy = o.vol_buy + item.vol_buy;
                o.vol_sell = o.vol_sell + item.vol_sell;

                o.price = item.price;
                //o.open = item.open;
                o.close = item.close;
                o.low = item.low < o.low ? item.low : o.low;
                o.high = item.high > o.high ? item.high : o.high;

                o.pair = item.pair;
                o.market = item.market;
                //更新
                RedisHelper.SetOrUpdateHash(key, code, o.ToJson());
            }
            else
            {
                RedisHelper.SetOrUpdateHash(key, code, item.ToJson());
            }
        }


        /// <summary>
        /// ALL Coin ALL Exchange Every Hour
        /// </summary>
        /// <param name="data"></param>
        public void ProcessALLCoinAllExchageEveryHour(List<UPermanentFuturesModel> data, string type, List<DateTime> times)
        {
            string key = type + "ALLCoinALLExchange" + DateTime.Now.ToString("yyyy-MM-dd");
            var exchagelist = CommandEnum.ExchangeData.ExchangeList;
            List<PermanentFutures> savelist = new List<PermanentFutures>();

            List<string> grouplist = new List<string>();

            foreach (var exchange in exchagelist)
            {
                var calcdata = data.Where(p => p.exchange == exchange).ToList();
                if (calcdata != null && calcdata.Count > 0)
                {
                    foreach (var dt in times)//每一分钟
                    {
                        var resultslist = calcdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();
                        grouplist = resultslist.GroupBy(p => p.market).Select(a => a.Key).ToList();


                        if (resultslist != null && resultslist.Count > 0)
                        {
                            foreach (var sy in grouplist)//bizhong
                            {

                                List<UPermanentFuturesModel> Modellist = new List<UPermanentFuturesModel>();
                                Modellist = resultslist.Where(p => p.market == sy).OrderBy(p => p.times).ToList();
                                saveupermanent(savelist, sy, Modellist, dt);
                            }
                        }
                    }


                }
                foreach (var item in savelist)
                {

                    var hourdata = RedisHelper.GetHash(key, item.exchange + "_" + item.timecode + "_" + item.market);
                    saveandupdate(item, hourdata, key, item.exchange + "_" + item.timecode + "_" + item.market);

                    var datedata = RedisHelper.GetHash(key, item.exchange + "_" + item.Times.ToString("yyyy-MM-dd") + "_" + item.market);
                    saveandupdate(item, datedata, key, item.exchange + "_" + item.Times.ToString("yyyy-MM-dd") + "_" + item.market);

                }
            }


        }
        private static void saveupermanent(List<PermanentFutures> savelist, string sy, List<UPermanentFuturesModel> Modellist, DateTime dt)
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

                var buyslist = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY);


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



//public void ProcessALLCoinAllExchageEveryHour(List<UPermanentFuturesModel> data, string type, List<DateTime> times)
//{
//    string key = type + "ALLCoinALLExchange" + DateTime.Now.ToString("yyyy-MM-dd");
//    var exchagelist = CommandEnum.ExchangeData.ExchangeList;
//    List<PermanentFutures> savelist = new List<PermanentFutures>();

//    List<string> grouplist = new List<string>();

//    foreach (var exchange in exchagelist)
//    {
//        var calcdata = data.Where(p => p.exchange == exchange).ToList();
//        if (calcdata != null && calcdata.Count > 0)
//        {
//            foreach (var dt in times)//每一分钟
//            {
//                var resultslist = calcdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();
//                grouplist = resultslist.GroupBy(p => p.market).Select(a => a.Key).ToList();


//                if (resultslist != null && resultslist.Count > 0)
//                {
//                    foreach (var sy in grouplist)//bizhong
//                    {

//                        List<UPermanentFuturesModel> Modellist = new List<UPermanentFuturesModel>();
//                        Modellist = resultslist.Where(p => p.market == sy).OrderBy(p => p.times).ToList();
//                        saveupermanent(savelist, sy, Modellist, dt);
//                    }
//                }
//            }


//        }
//        foreach (var item in savelist)
//        {
//            ////取        
//            var tradedata = RedisHelper.GetHash(key, item.exchange + "_" + item.timecode + "_" + item.market);
//            if (tradedata != null && tradedata != "")
//            {
//                var o = tradedata.ToObject<PermanentFutures>();
//                o.Times = item.Times;
//                o.close = item.close;
//                o.count = o.count + item.count;
//                o.count_buy = o.count_buy + item.count_buy;
//                o.count_sell = o.count_sell + item.count_sell;
//                o.exchange = item.exchange;

//                o.qty = o.qty + item.qty;
//                o.qty_buy = o.qty_buy + item.qty_buy;
//                o.qty_sell = o.qty_sell + item.qty_sell;

//                o.vol = o.vol + item.vol;
//                o.vol_buy = o.vol_buy + item.vol_buy;
//                o.vol_sell = o.vol_sell + item.vol_sell;

//                o.price = item.price;
//                o.open = item.open;
//                o.close = item.close;
//                o.low = item.low < o.low ? item.low : o.low;
//                o.high = item.high > o.high ? item.high : o.high;

//                //更新
//                RedisHelper.SetOrUpdateHash(key, item.exchange + "_" + item.timecode + "_" + item.market, o.ToJson());
//            }
//            else
//            {
//                RedisHelper.SetOrUpdateHash(key, item.exchange + "_" + item.timecode + "_" + item.market, item.ToJson());
//            }
//        }
//    }


//}