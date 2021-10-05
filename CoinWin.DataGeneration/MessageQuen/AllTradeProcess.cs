using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration 
{
   public class AllTradeProcess
    {

        /// <summary>
        /// 跨天无法处理
        /// </summary>
        public void CalcPere()
        {
        peagain:
            try
            {
                FundingRateAndOpenInterest fo = new FundingRateAndOpenInterest();

                while (true)
                {
                    var count = RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.PERPQueueList);
                    if (count < 50000)
                    {
                        Console.WriteLine("永续数量不足1W，暂不汇总计算！=====" + DateTime.Now.ToString());
                        Thread.Sleep(60000);
                        continue;
                    }
                    List<LiquidationModel> lqdata = new List<LiquidationModel>();//获取bc数据
                    List<OpenInterest> opinterestdata = new List<OpenInterest>();//获取cc数据
                    List<FundRate> frdata = new List<FundRate>();

                    //new Thread(() =>
                    //{
                    //    lqdata = fo.GetLQdata();
                    //}).Start();

                    new Thread(() =>
                    {
                        opinterestdata = fo.GetOpenInterests(CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd")).Where(a => a.market != null && a.symbol != null).ToList();
                    }).Start();
                    new Thread(() =>
                    {
                        frdata = fo.GetFundRateData(CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd")).Where(a => a.market != null && a.symbol != null).ToList();
                    }).Start();



                    var lqcount = RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.liquidationList);
                    if (lqcount > 0)
                    {
                        while (true)
                        {
                            var lq = RedisMsgQueueHelper.DeQueue(CommandEnum.RedisKey.liquidationList);

                            if (lq.Contains("["))
                            {
                                var res = lq.ToList<LiquidationModel>();
                                lqdata.AddRange(res);
                            }
                            else
                            {
                                var res = lq.ToObject<LiquidationModel>();
                                lqdata.Add(res);
                            }
                            if (lqdata.Count == lqcount)
                            {
                                break;
                            }
                        }
                    }


                    //lqdata = fo.GetLQdata(DateTime.Now);
                    //opinterestdata = fo.GetOpenInterests(CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd")).Where(a=>a.market!=null&&a.symbol!=null).ToList();
                    //frdata = fo.GetFundRateData(CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd")).Where(a => a.market != null && a.symbol != null).ToList();

                    List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
                    while (true)
                    {
                        //Console.WriteLine("开始时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH mm:ss:fff"));
                        list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.PERPQueueList, 0).ToList<UPermanentFuturesModel>());
                        if (list.Count > 100000)
                        {
                            break;
                        }
                    }

                    var end = Convert.ToDateTime(list.Max(p => Convert.ToDateTime(p.times)));
                    var start = Convert.ToDateTime(list.Min(p => Convert.ToDateTime(p.times)));

                    //ms: 发生在获取 队列信息是出现，不在这一时间段的数据，导致循环无限执行
                    if (start < DateTime.Now.AddDays(-1))
                    {
                        Console.WriteLine("发生错误时间：" + start.ToString("yyyy-MM-dd HH"));
                        LogHelper.WriteLog(typeof(DownExchangeData), "发生错误时间：" + start.ToString("yyyy-MM-dd HH"));
                        var errorjson = list.FirstOrDefault(p => Convert.ToDateTime(p.times) == start);
                        if (errorjson != null)
                        {
                            Console.WriteLine("发生错误时间的数据：" + errorjson.ToJson().ToString()) ;
                            LogHelper.WriteLog(typeof(DownExchangeData), ("发生错误时间的数据：" + errorjson.ToJson().ToString()));
                        }



                        start = DateTime.Now.AddHours(-3);
                    }
                   

                    var timelist = TimeCore.GetStartAndEndTimeContent(start, end);
                    if (timelist != null && timelist.Count > 0)
                    {
                        if (list != null && list.Count() > 0)
                        {

                            ProcessAllPerpHourAndDate(list, timelist.OrderBy(p => p).ToList(), CommandEnum.RedisKey.PERPRealDeal, lqdata, opinterestdata, frdata, CommandEnum.RedisKey.PERP);
                            ////全网币种小时汇总 全网币种天汇总 

                            ProcessALLCoinAllExchageEveryHour(list, "PERP", timelist.OrderBy(p => p).ToList(), lqdata, opinterestdata, frdata, CommandEnum.RedisKey.PERP);//全网逐一交易所逐一币种每小时

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("处理永续数据异常，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "处理永续数据异常，错误信息：" + e.Message.ToString());
                goto peagain;
            }
        }

        public void CalcDeliver()
        {
            deaggin:
            try
            {
                FundingRateAndOpenInterest fo = new FundingRateAndOpenInterest();
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

                    List<LiquidationModel> lqdata = new List<LiquidationModel>();//获取bc数据
                    List<OpenInterest> opinterestdata = new List<OpenInterest>();//获取cc数据
                    List<FundRate> frdata = new List<FundRate>();

                    new Thread(() =>
                    {
                        opinterestdata = fo.GetOpenInterests(CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd")).Where(a => a.market != null && a.symbol != null).ToList();
                    }).Start();


                    while (true)
                    {

                        list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.DeliverQueueList, 0).ToList<UPermanentFuturesModel>());

                        if (list.Count > 50000)
                        {
                            break;
                        }
                    }




                    //lqdata = fo.GetLQdata(DateTime.Now);

                    //opinterestdata = fo.GetOpenInterests(CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd")).Where(a => a.market != null && a.symbol != null).ToList();
                    //frdata = fo.GetFundRateData(CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd")).Where(a => a.market != null && a.symbol != null).ToList();


                    var end = Convert.ToDateTime(list.Max(p => Convert.ToDateTime(p.times)));
                    var start = Convert.ToDateTime(list.Min(p => Convert.ToDateTime(p.times)));

                    var timelist = TimeCore.GetStartAndEndTimeContent(start, end);
                    if (timelist != null && timelist.Count > 0)
                    {
                        if (list != null && list.Count() > 0)
                        {
                            ProcessAllPerpHourAndDate(list, timelist.OrderBy(p => p).ToList(), CommandEnum.RedisKey.DeliverRealDeal, lqdata, opinterestdata, frdata, CommandEnum.RedisKey.DELIVERY);
                            ////全网币种小时汇总 全网币种天汇总 

                            ProcessALLCoinAllExchageEveryHour(list, "Deliver", timelist.OrderBy(p => p).ToList(), lqdata, opinterestdata, frdata, CommandEnum.RedisKey.DELIVERY);//全网逐一交易所逐一币种每小时
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("处理交割数据异常，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "处理交割数据异常，错误信息："+e.Message.ToString());
                goto deaggin;
            }
        }

        public void CalcSpot()
        {
        spotagain:
            try
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
                        list.AddRange(RedisMsgQueueHelper.DeQueueBlock(CommandEnum.RedisKey.SpotQueueList, 0).ToList<UPermanentFuturesModel>());

                        if (list.Count > 50000)
                        {
                            break;
                        }
                    }
                    List<LiquidationModel> lqdata = new List<LiquidationModel>();//获取bc数据
                    List<OpenInterest> opinterestdata = new List<OpenInterest>();//获取cc数据
                    List<FundRate> frdata = new List<FundRate>();


                    var end = Convert.ToDateTime(list.Max(p => Convert.ToDateTime(p.times)));
                    var start = Convert.ToDateTime(list.Min(p => Convert.ToDateTime(p.times)));


                    var timelist = TimeCore.GetStartAndEndTimeContent(start, end);
                    if (timelist != null && timelist.Count > 0)
                    {
                        if (list != null && list.Count() > 0)
                        {

                            ProcessAllPerpHourAndDate(list, timelist.OrderBy(p => p).ToList(), CommandEnum.RedisKey.SPOTRealDeal, lqdata, opinterestdata, frdata, CommandEnum.RedisKey.SPOT);
                            ////全网币种小时汇总 全网币种天汇总 
                            ProcessALLCoinAllExchageEveryHour(list, "SPOT", timelist.OrderBy(p => p).ToList(), lqdata, opinterestdata, frdata, CommandEnum.RedisKey.SPOT);//全网逐一交易所逐一币种每小时
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("处理现货数据异常，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "处理现货数据异常，错误信息：" + e.Message.ToString());
                goto spotagain;
            }
        }





        /// <summary>
        /// 永续 交割 全网 币种 小时and 天 汇总
        /// </summary>
        /// <param name="data"></param>
        /// <param name="times"></param>
        /// <param name="key">永续or交割</param>CommandEnum.RedisKey.DeliverRealDeal or    CommandEnum.RedisKey.PERPRealDeal
        public void ProcessAllPerpHourAndDate(List<UPermanentFuturesModel> data, List<DateTime> times, string key, List<LiquidationModel>lq, List<OpenInterest> opdata, List<FundRate>frdata,string types)
        {
            List<TradeMater> savelist = new List<TradeMater>();
            string resultkey = key +"Hour"+times.FirstOrDefault().ToString("yyyy-MM-dd");

            ProcessSaveList(data, times, lq, opdata, frdata, types, savelist);

            //var hourtradeMasterlist = RedisHelper.GetAllHash<TradeMater>(key+"Date");
            var hourtradeMasterlist = RedisHelper.GetAllHash<TradeMater>(resultkey);
            var daytradeMasterlist = RedisHelper.GetAllHash<TradeMater>(key + times.FirstOrDefault().ToString("yyyy-MM-dd"));
   

            foreach (var item in savelist.OrderBy(p=>p.Times))
            {
                UpdateTradeList(item, key + "Hour" + item.Times.ToString("yyyy-MM-dd"), item.timecode + "_" + item.contractcode, hourtradeMasterlist);
                UpdateTradeList(item, key + item.Times.ToString("yyyy-MM-dd"), item.Times.ToString("yyyy-MM-dd") + "_" + item.contractcode, daytradeMasterlist);
            }
        }




        /// <summary>
        /// 天小时 汇总处理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="times"></param>
        /// <param name="lq"></param>
        /// <param name="opdata"></param>
        /// <param name="frdata"></param>
        /// <param name="types"></param>
        /// <param name="savelist"></param>
        private void ProcessSaveList(List<UPermanentFuturesModel> data, List<DateTime> times, List<LiquidationModel> lq, List<OpenInterest> opdata, List<FundRate> frdata, string types, List<TradeMater> savelist)
        {
            List<string> grouplist = new List<string>();
            if (data != null && data.Count > 0)
            {
                foreach (var dt in times)//每一分钟
                {
                    //tradadata
                    var resultslist = data.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();
                    //lqdata
                    var resultlqdata = lq.Where(p => p.times.ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();
                    var resultlqdataTF = lq.Where(p => p.times.ToString("yyyy-MM-dd") == dt.ToString("yyyy-MM-dd")).ToList();
                    //opendata
                    var resultopendata = opdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH") && p.kind == types).ToList();
                    var resultfrdata = frdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();

                    //var ones = opdata.GroupBy(p => p.kind).ToList();


                    grouplist = resultslist.GroupBy(p => p.contractcode).Select(a => a.Key).ToList();
                    if (resultslist != null && resultslist.Count > 0)
                    {
                        foreach (var sy in grouplist)//bizhong
                        {

                            var Modellist = resultslist.Where(p => p.contractcode == sy).OrderBy(p => p.times).ToList();

                            var lqdatas = resultlqdata.Where(p => p.pair.Contains(sy)).ToList();

                            var lqdatastf = resultlqdataTF.Where(p => p.pair.Contains(sy)).ToList();

                            var opendatas = resultopendata.Where(p => p.symbol.Contains(sy)).OrderByDescending(p => Convert.ToDateTime(p.times)).ToList();

                            opendatas = opendatas.Distinct(new OpenInterestComparer()).ToList();

                            var frdatas = resultfrdata.Where(p => p.symbol.Contains(sy)).OrderByDescending(p => Convert.ToDateTime(p.times)).ToList();

                            frdatas = frdatas.Distinct(new FundRateComparer()).ToList();

                            CreateTradeMater(types, savelist, dt, sy, Modellist, lqdatas, lqdatastf, opendatas, frdatas);

                        }
                    }
                }
            }
        }


        /// <summary>
        /// 处理实时数据生成savelist
        /// </summary>
        /// <param name="types"></param>
        /// <param name="savelist"></param>
        /// <param name="dt"></param>
        /// <param name="sy"></param>
        /// <param name="Modellist"></param>
        /// <param name="lqdatas"></param>
        /// <param name="lqdatastf"></param>
        /// <param name="opendatas"></param>
        /// <param name="frdatas"></param>
        private void CreateTradeMater(string types, List<TradeMater> savelist, DateTime dt, string sy, List<UPermanentFuturesModel> Modellist, List<LiquidationModel> lqdatas, List<LiquidationModel> lqdatastf, List<OpenInterest> opendatas, List<FundRate> frdatas)
        {
            if (Modellist.Count() > 0)
            {
                TradeMater df = new TradeMater();
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

                decimal lqbuy = 0;
                decimal lqsell = 0;
                df.liquidation = lqdatas.Sum(p => p.vol);
                var lqnow = GetLQ("BUY", lqdatas);
                lqnow.TryGetValue("BUY", out lqbuy);
                lqnow.TryGetValue("SELL", out lqsell);

                df.liquidation_buy = lqbuy;//多bao==buy
                df.liquidation_sell = lqsell;//空bao==sell

                decimal lqbuyTF = 0;
                decimal lqsellTF = 0;

                var lqtf = GetLQ("BUY", lqdatastf);
                lqtf.TryGetValue("BUY", out lqbuyTF);
                lqtf.TryGetValue("SELL", out lqsellTF);

                df.liquidationTFH = lqdatastf.Sum(p => p.vol);
                df.liquidation_TFHbuy = lqbuyTF;
                df.liquidation_TFHsell = lqsellTF;

                df.SumOpenInterestValueStart = opendatas.Sum(p => p.SumOpenInterestValue);
                df.SumOpenInterestValueNow = opendatas.Sum(p => p.SumOpenInterestValue);
                //df.FundingRateNow = frdatas.Select(p=>(p.market+p.exchange+p.FundingRate).ToString()).ToString();
                df.nowcoin = opendatas.Sum(p => p.coin);
                df.startcoin = opendatas.Sum(p => p.coin);
                df.volumeUsd24h = opendatas.Sum(p => p.volumeUsd24h);


                string frstring = "";
                string openstring = "";
                if (frdatas.Count > 0)
                {
                    frstring = frdatas.ToJson();
                }
                //if (opendatas.Count > 0)
                //{
                //    openstring = opendatas.ToJson();
                //}

                df.FundingRateNow = frstring;
                df.FundingRateStart = frstring;
                //df.description = frstring + openstring;
                df.description = "";

                df.basis = "0";
                df.market = string.Join(",", Modellist.GroupBy(p => p.market).Select(a => a.Key).ToList());
                df.timestamp = Modellist.FirstOrDefault().timestamp;
                df.types = string.Join(",", Modellist.GroupBy(p => p.types).Select(a => a.Key).ToList());
                df.utime = Modellist.FirstOrDefault().utctime;
                df.Times = Convert.ToDateTime(Modellist.Max(a => a.times));
                df.timecode = dt.ToString("yyyy-MM-dd HH");
         

                df.kind = types;
                savelist.Add(df);
            }
        }

        /// <summary>
        /// 查询KEy根据code更新数据到redis 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <param name="dts"></param>
        private static void UpdateTradeList(TradeMater item, string key, string code,Dictionary<string, TradeMater>dts)
        {
            TradeMater values = null;
            
            bool flag = dts.TryGetValue(code, out values);

            if (values != null)
            {
                //var values = tradedata.ToObject<TradeMater>();
                values.Times = item.Times;
                values.close = item.close;
                values.count = values.count + item.count;
                values.count_buy = values.count_buy + item.count_buy;
                values.count_sell = values.count_sell + item.count_sell;
                values.exchange = item.exchange;
                values.types = item.types;
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

                 
                values.SumOpenInterestValueNow = item.SumOpenInterestValueNow;
                values.SumOpenInterestValueStart = values.SumOpenInterestValueStart == 0 ? item.SumOpenInterestValueStart : values.SumOpenInterestValueStart;

                values.liquidation = values.liquidation+item.liquidation;
                values.liquidation_buy = values.liquidation_buy+ item.liquidation_buy;
                values.liquidation_sell = values.liquidation_sell+ item.liquidation_sell;

                values.liquidationTFH = values.liquidationTFH+ item.liquidationTFH;
                values.liquidation_TFHbuy = values.liquidation_TFHbuy+ item.liquidation_TFHbuy;
                values.liquidation_TFHsell = values.liquidation_TFHsell+item.liquidation_TFHsell;


                values.contractcode = item.contractcode;
                //values.startcoin = item.startcoin;
                values.startcoin = values.startcoin == 0 ? item.startcoin : values.startcoin;
                values.nowcoin = item.nowcoin;
                values.volumeUsd24h = item.volumeUsd24h;
                values.FundingRateNow = item.FundingRateNow;
                //values.description = item.description;

                //values.FundingRateNow = "";
                values.description ="";

                values.timecode = item.timecode;
                values.timestamp = item.timestamp;
          
                //更新
                RedisHelper.SetOrUpdateHash(key, code, values.ToJson());
            }
            else
            {
                RedisHelper.SetOrUpdateHash(key, code, item.ToJson());
            }
        }

 
         /// <summary>
         /// 
         /// </summary>
         /// <param name="data"></param>
         /// <param name="keys"></param>
         /// <param name="times"></param>
         /// <param name="lq"></param>
         /// <param name="opdata"></param>
         /// <param name="frdata"></param>
         /// <param name="types"></param>
        public void ProcessALLCoinAllExchageEveryHour(List<UPermanentFuturesModel> data,  string keys, List<DateTime> times, List<LiquidationModel> lq, List<OpenInterest> opdata, List<FundRate> frdata, string types)
        {
            //string key = type+"ALLCoinALLExchange" +DateTime.Now.ToString("yyyy-MM-dd");
            string key = keys + "ALLCoinALLExchange" + times.FirstOrDefault().ToString("yyyy-MM-dd");
            var exchagelist = CommandEnum.ExchangeData.ExchangeList;
            List<TradeMater> savelist = new List<TradeMater>();
            List<string> grouplist = new List<string>();
            var hourtradeMasterlist = RedisHelper.GetAllHash<TradeMater>(key);    
            foreach (var exchange in exchagelist)
            {
                var calcdata = data.Where(p => p.exchange == exchange).ToList();
                if (calcdata != null && calcdata.Count > 0)
                {
                    foreach (var dt in times)//每一分钟
                    {
                        key = keys + "ALLCoinALLExchange" + dt.ToString("yyyy-MM-dd");

                        var resultslist = calcdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();
                        grouplist = resultslist.GroupBy(p => p.market).Select(a => a.Key).ToList();
                       
                        //lqdata
                        var resultlqdata = lq.Where(p => p.times.ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")&&p.exchange== exchange).ToList();
                        var resultlqdataTF = lq.Where(p => p.times.ToString("yyyy-MM-dd") == dt.ToString("yyyy-MM-dd") && p.exchange == exchange).ToList();
                        //opendata
                        var resultopendata = opdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH") && p.kind == types && p.exchange == exchange).ToList();
                        var resultfrdata = frdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH") && p.exchange == exchange).ToList();


                        if (resultslist != null && resultslist.Count > 0)
                        {
                            foreach (var sy in grouplist)//bizhong
                            {

                                var lqdatas = resultlqdata.Where(p => p.market == (sy)).ToList();

                                var lqdatastf = resultlqdataTF.Where(p => p.market == (sy)).ToList();


                                var opendatas = resultopendata.Where(p => p.market==(sy)).OrderByDescending(p => Convert.ToDateTime(p.times)).ToList();
                                opendatas = opendatas.Distinct(new OpenInterestComparer()).ToList();

                                var frdatas = resultfrdata.Where(p => p.market==(sy)).OrderByDescending(p => Convert.ToDateTime(p.times)).ToList();
                                frdatas = frdatas.Distinct(new FundRateComparer()).ToList();


                                List<UPermanentFuturesModel> Modellist = new List<UPermanentFuturesModel>();
                                Modellist = resultslist.Where(p => p.market == sy).OrderBy(p => p.times).ToList();
                                CreateTradeMater(types, savelist, dt, sy, Modellist, lqdatas, lqdatastf, opendatas, frdatas);
                                //saveupermanent(savelist, sy, Modellist, dt);

                            }
                        }
                    }


                }
                foreach (var item in savelist)
                {
                    //saveandupdatenew(item, key, item.exchange + "_" + item.timecode + "_" + item.market, hourtradeMasterlist);
                    //saveandupdatenew(item, key, item.exchange + "_" + item.Times.ToString("yyyy-MM-dd") + "_" + item.market, hourtradeMasterlist);
                    UpdateTradeList(item, key, item.exchange + "_" + item.timecode + "_" + item.market, hourtradeMasterlist);
                    UpdateTradeList(item, key, item.exchange + "_" + item.Times.ToString("yyyy-MM-dd") + "_" + item.market, hourtradeMasterlist);


                }

                savelist = new List<TradeMater>();
            }
        }

        public Dictionary<string, decimal> GetLQ(string type, List<LiquidationModel> lqdatas)
        {
            Dictionary<string, decimal> lists = new Dictionary<string, decimal>();
            decimal lqbuy = 0;
            decimal lqsell = 0;
            if (lqdatas.Count > 0)
            {
                var lqbybit = lqdatas.Where(p => p.exchange.ToUpper() == "BYBIT");

                if (lqbybit.Count() > 0)
                {
                    var lqall = lqdatas.Where(p => p.exchange.ToUpper() != "BYBIT");

                    lqbuy = lqall.Where(p => p.side.ToUpper() == "BUY").Sum(p => p.vol) + lqbybit.Where(p => p.side.ToUpper() == "SELL").Sum(p => p.vol);

                    lqsell = lqall.Where(p => p.side.ToUpper() == "SELL").Sum(p => p.vol) + lqbybit.Where(p => p.side.ToUpper() == "BUY").Sum(p => p.vol); 
                }
                else
                {
                    lqbuy = lqdatas.Where(p => p.side.ToUpper() == "BUY").Sum(p => p.vol);
                    lqsell = lqdatas.Where(p => p.side.ToUpper() == "SELL").Sum(p => p.vol);
                }
            }
            lists.Add("BUY", lqbuy);
            lists.Add("SELL", lqsell);
            return lists;

        }

    }
}
//var lqcount = RedisMsgQueueHelper.GetQueueCount(CommandEnum.RedisKey.liquidationList);
//while (true)
//{
//    var lq = RedisMsgQueueHelper.DeQueue(CommandEnum.RedisKey.liquidationList);

//    if (lq.Contains("["))
//    {
//        var res = lq.ToList<LiquidationModel>();
//        lqdata.AddRange(res);
//    }
//    else
//    {
//        var res = lq.ToObject<LiquidationModel>();
//        lqdata.Add(res);
//    }

//    //lqdata.Add(lq.ToObject<LiquidationModel>());

//    if (lqdata.Count == lqcount)
//    {
//        break;
//    }
//}






//private static void saveupermanent(List<PermanentFutures> savelist, string sy, List<UPermanentFuturesModel> Modellist, DateTime dt)
//{
//    if (Modellist.Count() > 0)
//    {
//        PermanentFutures df = new PermanentFutures();
//        df.contractcode = Modellist.FirstOrDefault().contractcode; ;
//        df.price = Modellist.Average(p => p.price);
//        df.open = Modellist.FirstOrDefault().price;
//        df.close = Modellist.Last().price;
//        df.low = Modellist.Min(p => p.price);
//        df.high = Modellist.Max(p => p.price);

//        //var buyslist = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY);


//        df.vol = Modellist.Sum(p => p.vol);
//        df.vol_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.vol);
//        df.vol_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.vol);

//        df.qty = Modellist.Sum(p => p.qty);
//        df.qty_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.qty);
//        df.qty_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.qty);
//        df.Unit = Modellist.FirstOrDefault().Unit == null ? "" : Modellist.FirstOrDefault().Unit;
//        df.pair = string.Join(",", Modellist.GroupBy(p => p.pair).Select(a => a.Key).ToList());
//        df.count = Modellist.Count();
//        df.count_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Count();
//        df.count_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Count();
//        df.exchange = string.Join(",", Modellist.GroupBy(p => p.exchange).Select(a => a.Key).ToList());


//        df.liquidation = 0;
//        df.liquidation_buy = 0;
//        df.liquidation_sell = 0;
//        df.basis = "0";
//        df.market = string.Join(",", Modellist.GroupBy(p => p.market).Select(a => a.Key).ToList());
//        df.timestamp = Modellist.FirstOrDefault().timestamp;
//        df.types = string.Join(",", Modellist.GroupBy(p => p.types).Select(a => a.Key).ToList());
//        df.utime = Modellist.FirstOrDefault().utctime;
//        df.Times = Convert.ToDateTime(Modellist.Max(a => a.times));

//        df.timecode = dt.ToString("yyyy-MM-dd HH");

//        savelist.Add(df);
//    }
//}
//private static void saveandupdatenew(PermanentFutures item, string key, string code, Dictionary<string, PermanentFutures> dts)
//{
//    PermanentFutures values = null;
//    bool flag = dts.TryGetValue(code, out values);
//    if (values != null)
//    {
//        //var values = tradedata.ToObject<PermanentFutures>();
//        values.Times = item.Times;
//        values.close = item.close;
//        values.count = values.count + item.count;
//        values.count_buy = values.count_buy + item.count_buy;
//        values.count_sell = values.count_sell + item.count_sell;
//        values.exchange = item.exchange;

//        values.qty = values.qty + item.qty;
//        values.qty_buy = values.qty_buy + item.qty_buy;
//        values.qty_sell = values.qty_sell + item.qty_sell;

//        values.vol = values.vol + item.vol;
//        values.vol_buy = values.vol_buy + item.vol_buy;
//        values.vol_sell = values.vol_sell + item.vol_sell;

//        values.price = item.price;
//        //o.open = item.open;
//        values.close = item.close;
//        values.low = item.low < values.low ? item.low : values.low;
//        values.high = item.high > values.high ? item.high : values.high;

//        values.pair = item.pair;
//        values.market = item.market;
//        //更新
//        RedisHelper.SetOrUpdateHash(key, code, values.ToJson());
//    }
//    else
//    {
//        RedisHelper.SetOrUpdateHash(key, code, item.ToJson());
//    }
//}

