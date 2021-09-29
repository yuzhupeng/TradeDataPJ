using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration  
{
   public class BBODataProcess
    {
        public void CalcBooData()
        {
            while (true)
            {
                var count = RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.AllBBoQueueList);
                if (count < 10000)
                {
                    Console.WriteLine("挂单数量不足1W，暂不汇总计算！---" + DateTime.Now.ToString());
                    Thread.Sleep(60000);
                    continue;
                }
                List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
                List<HuobiQuatelMarkData> datalist = new List<HuobiQuatelMarkData>();


                while (true)
                {
                    //获取挂单数据
                    datalist.Add(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.AllBBoQueueList, 0).ToObject<HuobiQuatelMarkData>());

                    ////获取成交数据
                    //list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.DeliverQueueList, 0).ToList<UPermanentFuturesModel>());

                    if (datalist.Count > 10000)
                    {
                        break;
                    }
                }

                ProcessBigData(datalist);



                //var end = Convert.ToDateTime(list.Max(p => p.times));
                //var start = Convert.ToDateTime(list.Min(p => p.times));
                //var timelist = TimeCore.GetStartAndEndTimeContent(start, end);
                //if (timelist != null && timelist.Count > 0)
                //{
                //    if (list != null && list.Count() > 0)
                //    {
                //        //ProcessAllPerpHourAndDate(list, timelist, CommandEnum.RedisKey.DeliverRealDeal);
                //        //ProcessALLCoinAllExchageEveryHour(list, "Deliver", timelist);
                //    }
                //}



            }
        }


        /// <summary>
        /// 处理大单数据
        /// </summary>
        /// <param name="list"></param>
        public void ProcessBigData(List<HuobiQuatelMarkData> list)
        {
            List<MarketData> collectlist = new List<MarketData>();
            if (list != null && list.Count > 0  )
            {
               var biglist= list.Where(p => p.sellcount > 5000 || p.buycount > 5000).ToList();
                if (biglist.Count > 0)
                {
                    foreach (var item in biglist)
                    {
                        if (collectlist.FirstOrDefault(a => a.id == item.id) != null)
                        {
                            continue;
                        }


                       var maxlist= list.Where(p => p.id == item.id).ToList();
                       var maxtimes= maxlist.Max(p => p.ts);
                       var maxone=  maxlist.FirstOrDefault(p => p.ts == maxtimes);//最后成交id

                        MarketData o = new MarketData();
                        o.id = maxone.id;
                        o.symbol = maxone.ch.Split('.')[1].ToString(); ;
                        o.sellprice = maxone.sellprice;
                        o.buyprice = maxone.buyprice;
                        o.sellcount = item.sellcount;//start
                        o.buycount = item.buycount;
                        o.sdie = item.sellcount >= 5000 ? "sell" : "buy";
                        o.endsellcount = maxone.sellcount;
                        o.endbuycount = maxone.buycount;
                        o.endtime = maxone.times;
                        o.starttime = item.times;
                        if (o.sdie == "buy")
                        {
                            o.vol = ((Convert.ToInt64(o.buycount - o.endbuycount) * 100));
                            o.vols = ((Convert.ToInt64(o.buycount - o.endbuycount) * 100)).ToString();
                            o.totalcount = o.buycount - o.endbuycount;

                        }
                        else
                        {
                            o.vol = ((Convert.ToInt64(o.sellcount - o.endsellcount) * 100));
                            o.vols = ((Convert.ToInt64(o.sellcount - o.endsellcount) * 100)).ToString();
                            o.totalcount = o.sellcount - o.endsellcount;
                        }
                        collectlist.Add(o);
                        RedisHelper.Pushdata(o.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd") + CommandEnum.RedisKey.BBoBigDataList);
                    }
                }

                //if (list.Count > 0)
                //{
                //    foreach (var item in list)
                //    {
                //        if (collectlist.FirstOrDefault(a => a.id == item.id) != null)
                //        {
                //            continue;
                //        }


                //        var maxlist = list.Where(p => p.id == item.id).ToList();
                //        var maxtimes = maxlist.Max(p => p.ts);
                //        var maxone = maxlist.FirstOrDefault(p => p.ts == maxtimes);//最后成交id

                //        MarketData o = new MarketData();
                //        o.id = maxone.id;
                //        o.symbol = maxone.ch.Split('.')[1].ToString(); ;
                //        o.sellprice = maxone.sellprice;
                //        o.buyprice = maxone.buyprice;
                //        o.sellcount = item.sellcount;//start
                //        o.buycount = item.buycount;
                //        o.sdie = item.sellcount >= item.sellcount ? "sell" : "buy";
                //        o.endsellcount = maxone.sellcount;
                //        o.endbuycount = maxone.buycount;
                //        o.endtime = maxone.times;
                //        o.starttime = item.times;
                //        if (o.sdie == "buy")
                //        {
                //            o.vol = ((Convert.ToInt64(o.buycount - o.endbuycount) * 100));
                //            o.vols = ((Convert.ToInt64(o.buycount - o.endbuycount) * 100)).ToString();
                //            o.totalcount = o.buycount - o.endbuycount;
                //        }
                //        else
                //        {
                //            o.vol = ((Convert.ToInt64(o.sellcount - o.endsellcount) * 100));
                //            o.vols = ((Convert.ToInt64(o.sellcount - o.endsellcount) * 100)).ToString();
                //            o.totalcount = o.sellcount - o.endsellcount;
                //        }
                //        collectlist.Add(o);
                //        RedisHelper.Pushdata(o.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd") +"ALL"+CommandEnum.RedisKey.BBoBigDataList);
                //    }
                //}
            }

        }


        public void CheckCount()
        {
            string key = DateTime.Now.ToString("yyyy-MM-dd") + CommandEnum.RedisKey.BBoBigDataList;
            List<MarketData> list = new List<MarketData>();


            var results = RedisHelper.GetSetSScanObjectT<MarketData>(key, "DB0");
            if (results != null && results.Count > 0)
            {
                list = results;
            }
            var dealmaxlist = list.Where(p => p.totalcount >= 5000).ToList();
            var buy = dealmaxlist.Where(p => p.sdie == "buy").OrderBy(p => p.id);
            foreach (var item in buy )
            {
                Console.WriteLine("id:"+item.id+"张数："+item.totalcount+"|价格："+item.buyprice + "|时间：" + item.endtime);
            }        
                


            Console.WriteLine("买总：" + buy.Sum(p => p.vol));
            var sell = dealmaxlist.Where(p => p.sdie == "sell").OrderBy(p => p.id);
            foreach (var item in sell)
            {
                Console.WriteLine("id:" + item.id + "张数：" + item.totalcount + "|价格：" + item.sellprice + "|时间：" + item.endtime);
            }

            Console.WriteLine("卖总："+sell.Sum(p => p.vol));

            Console.ReadKey();
            //var biglist = list.Where(p => p.sellcount >= 10000 || p.buycount >= 10000).ToList();

        }

    }

     
}
