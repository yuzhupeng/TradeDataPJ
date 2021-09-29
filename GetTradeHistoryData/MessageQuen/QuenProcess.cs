using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetTradeHistoryData
{
   public class QuenProcess
    {
        ///每小时,4小时，24小时 的成交量,爆仓，对应的交易对，区分永续和交割
        //
        public void CalcPere()
        {
            List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
            List<string> s = new List<string>();
            while (true)
            {
                while (true)
                {
                    //Console.WriteLine("开始时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                    list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.PERPQueueList, 100000).ToList<UPermanentFuturesModel>());
                    //s.Add(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.PERPQueueList, 1000));
                    //Console.WriteLine("结束时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                    Console.WriteLine(s.Count);
                    if (list.Count > 50000)
                    {
                        break;
                    }
                }


                var exchagelist = CommandEnum.ExchangeData.ExchangeList;

                var end = Convert.ToDateTime(list.Max(p => p.times));
                var start=Convert.ToDateTime( list.Min(p => p.times));



                foreach (var item in exchagelist)//循环每一个交易所
                {

                    var exchage = list.Where(p => p.exchange == item).ToList();
                    if (exchage != null && exchage.Count() > 0)
                    {
                        //ProcessOneExchagePerp(exchage,item);
                    }

                }
            }

        }


        /// <summary>
        /// 循环处理每一个交易所
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
        /// <param name="dtlist"></param>
        /// <param name="exchange"></param>
        public void ProcessOneExchagePerp(List<UPermanentFuturesModel> data, string exchange)
        {
            List<PermanentFutures> savelist = new List<PermanentFutures>();

            List<string> grouplist = new List<string>();


            if (data != null && data.Count > 0)
            {
                //foreach (var dt in dtlist)//每一分钟
                //{
                //SaveListDelivery(savelist);
                var resultslist = data.Where(p => Convert.ToDateTime(p.SYS_CreateDate).ToString("yyyy-MM-dd HH") == DateTime.Now.ToString("yyyy-MM-dd HH")).ToList();

                grouplist = resultslist.GroupBy(p => p.pair).Select(a => a.Key).ToList();

                if (resultslist != null && resultslist.Count > 0)
                {
                    foreach (var sy in grouplist)//bizhong
                    {
                        var Modellist = resultslist.Where(p => p.pair == sy).OrderBy(p => p.SYS_CreateDate).ToList();
                        if (Modellist.Count() > 0)
                        {
                            PermanentFutures df = new PermanentFutures();
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
                            df.pair = Modellist.FirstOrDefault().pair;
                            df.count = Modellist.Count();
                            df.count_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Count();
                            df.count_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Count();
                            df.exchange = exchange;
                            df.liquidation = 0;
                            df.liquidation_buy = 0;
                            df.liquidation_sell = 0;
                            df.basis = "0";
                            df.market = Modellist.FirstOrDefault().market;
                            df.timestamp = Modellist.FirstOrDefault().timestamp;
                            df.types = Modellist.FirstOrDefault().types;
                            df.utime = Modellist.FirstOrDefault().utctime;

                            df.Times = Convert.ToDateTime(Modellist.Max(a=>a.times));


                            savelist.Add(df);
                        }
                    }
                }
                //}
            }


            //if (savelist.Count() > 0)
            //{
            //    SavePermanentFuturesData(savelist);
            //}


        }


        /// <summary>
        /// 循环币对处理全网
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
        /// <param name="dtlist"></param>
        /// <param name="exchange"></param>
        public void ProcessAllPerp(List<UPermanentFuturesModel> data, string exchange)
        {
            List<PermanentFutures> savelist = new List<PermanentFutures>();

            List<string> grouplist = new List<string>();


            if (data != null && data.Count > 0)
            {
                //foreach (var dt in dtlist)//每一分钟
                //{
                //SaveListDelivery(savelist);
                var resultslist = data.Where(p => Convert.ToDateTime(p.SYS_CreateDate).ToString("yyyy-MM-dd HH") == DateTime.Now.ToString("yyyy-MM-dd HH")).ToList();

                grouplist = resultslist.GroupBy(p => p.pair).Select(a => a.Key).ToList();

                if (resultslist != null && resultslist.Count > 0)
                {
                    foreach (var sy in grouplist)//bizhong
                    {
                        var Modellist = resultslist.Where(p => p.pair == sy).OrderBy(p => p.SYS_CreateDate).ToList();
                        if (Modellist.Count() > 0)
                        {
                            PermanentFutures df = new PermanentFutures();
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
                            df.pair = Modellist.FirstOrDefault().pair;
                            df.count = Modellist.Count();
                            df.count_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Count();
                            df.count_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Count();
                            df.exchange = exchange;
                            df.liquidation = 0;
                            df.liquidation_buy = 0;
                            df.liquidation_sell = 0;
                            df.basis = "0";
                            df.market = Modellist.FirstOrDefault().market;
                            df.timestamp = Modellist.FirstOrDefault().timestamp;
                            df.types = Modellist.FirstOrDefault().types;
                            df.utime = Modellist.FirstOrDefault().utctime;

                            df.Times = Convert.ToDateTime(Modellist.Max(a => a.times));


                            savelist.Add(df);
                        }
                    }
                }
        
            }


    


        }




    }
}
