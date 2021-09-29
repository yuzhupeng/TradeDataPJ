using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// 处理爆仓小时汇总
    /// </summary>
    public class LiquidationDataProcess
    {


        public void ProcessLQdata()
        {

            try
            {
                List<LiquidationModel> lqdata = new List<LiquidationModel>();
                FundingRateAndOpenInterest fo = new FundingRateAndOpenInterest();
                var dit = RedisHelper.GetAllHash<string>("LastLQTime");
                var list = dit.Values.FirstOrDefault();
                DateTime nows = DateTime.Now;
                DateTime dts = new DateTime(nows.Year, nows.Month, nows.Day, 00, 00, 00);
                DateTime dt = new DateTime(nows.Year, nows.Month, nows.Day, 00, 00, 00);

                lqdata = fo.GetLQdata(DateTime.Now);


                if (list != null)
                {
                    dt = Convert.ToDateTime(list);
                }

                ProcessLQDateData(lqdata.Where(p => p.times >= dts).ToList());

                ProcessLQHourData(lqdata.Where(p => p.times >= dts).ToList(), dt);


                RedisHelper.SetOrUpdateHash("LastLQTime", CommandEnum.RedisKey.PERP, lqdata.Max(p=>p.times).ToString());


            }
            catch (Exception e)
            {
                Console.WriteLine("汇总统计BC数据出错，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "汇总统计BC数据出错，错误信息：" + e.Message.ToString());

            }

        }


        public void ProcessLQAlldata(DateTime st)
        {
            try
            {

                var end = DateTime.Now;
                var start = st;
                var datelist = TimeCore.GetDate(start, end);


                foreach (var times in datelist)
                {

                    DateTime dts = new DateTime(times.Year, times.Month, times.Day, 00, 00, 00);

                    List<LiquidationModel> lqdata = new List<LiquidationModel>();
                    FundingRateAndOpenInterest fo = new FundingRateAndOpenInterest();
                    //var dit = RedisHelper.GetAllHash<string>("LastLQTime");
                    //var list = dit.Values.FirstOrDefault();
                    DateTime dt = new DateTime(times.Year, times.Month, times.Day, 00, 00, 00);

                    lqdata = fo.GetLQdata(times);
                    if (lqdata.Count == 0)
                    {
                        continue;
                    }
                    Console.WriteLine("处理日期：" + times.ToShortDateString());
                    dt = lqdata.Min(p => Convert.ToDateTime(p.times));
                    //if (list != null)
                    //{
                    //    dt = Convert.ToDateTime(list);
                    //}

                    ProcessLQDateData(lqdata.Where(p => p.times >= dts).ToList());

                    ProcessLQHourData(lqdata.Where(p => p.times >= dts).ToList(), dt);

                    //RedisHelper.SetOrUpdateHash("LastLQTime", CommandEnum.RedisKey.liquidation, lqdata.Max(p => p.times).ToString());

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("汇总统计BC数据出错，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "汇总统计BC数据出错，错误信息：" + e.Message.ToString());

            }
        }


        /// <summary>
        /// 处理小时
        /// </summary>
        /// <param name="lqdata"></param>
        private void ProcessLQHourData(List<LiquidationModel> lqdata, DateTime dt)
        {
            //FundingRateAndOpenInterest fo = new FundingRateAndOpenInterest();
            //List<LiquidationModel> lqdata = new List<LiquidationModel>();//获取bc数据
            var dit = RedisHelper.GetAllHash<List<BinanceFuturesSymbol>>("SymbolList");
            var symbollist = dit.Values.FirstOrDefault();

            //lqdata = fo.GetLQdata(new DateTime(2021,03,30));
            //lqdata = fo.GetLQdata(DateTime.Now);

            List<LQSummarystatistics> savelist = new List<LQSummarystatistics>();

            if (lqdata.Count > 0)
            {
                var end = Convert.ToDateTime(lqdata.Max(p => Convert.ToDateTime(p.times)));
                //var start = Convert.ToDateTime(lqdata.Min(p => Convert.ToDateTime(p.times)));

                var datelist = TimeCore.GetStartAndEndTimeContent(dt, end);

                foreach (var hour in datelist)
                {
                    var resultopendata = lqdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == hour.ToString("yyyy-MM-dd HH")).ToList();

                    if (resultopendata.Count == 0)
                    {
                        continue;
                    }

                    ProcessSavelist(symbollist, savelist, resultopendata);
                }
            }


            foreach (var item in savelist)
            {
                RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.liquidationHour + item.Times.ToString("yyyy-MM-dd"), item.exchange + "_" + item.timecode + "_" + item.market, item.ToJson());
            }

        }


        /// <summary>
        /// 处理日
        /// </summary>
        /// <param name="lqdata"></param>
        private void ProcessLQDateData(List<LiquidationModel> lqdata)
        {

            //List<LiquidationModel> lqdata = new List<LiquidationModel>();//获取bc数据
            var dit = RedisHelper.GetAllHash<List<BinanceFuturesSymbol>>("SymbolList");
            var symbollist = dit.Values.FirstOrDefault();


            List<LQSummarystatistics> savelist = new List<LQSummarystatistics>();

            if (lqdata.Count > 0)
            {
                var end = Convert.ToDateTime(lqdata.Max(p => Convert.ToDateTime(p.times)));
                var start = Convert.ToDateTime(lqdata.Min(p => Convert.ToDateTime(p.times)));
                var datelist = TimeCore.GetDate(start, end);

                foreach (var hour in datelist)
                {
                    var resultopendata = lqdata.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd") == hour.ToString("yyyy-MM-dd")).ToList();

                    if (resultopendata.Count == 0)
                    {
                        continue;
                    }

                    ProcessSavelist(symbollist, savelist, resultopendata);
                }
            }
            foreach (var item in savelist)
            {
                RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.liquidationDate + item.Times.ToString("yyyy-MM-dd"), item.exchange + "_" + item.market, item.ToJson());
            }

        }





        private void ProcessSavelist(List<BinanceFuturesSymbol> symbollist, List<LQSummarystatistics> savelist, List<LiquidationModel> resultopendata)
        {
            foreach (var symbol in symbollist)
            {
                var resultlist = resultopendata.Where(p => p.market.ToUpper().Contains(symbol.BaseAsset.ToUpper())).ToList();//获取当前小时币对

                if (resultlist.Count == 0)
                {
                    continue;
                }
                var exchangelist = resultlist.GroupBy(p => p.exchange).Select(p => p.Key).ToList(); //交易所列表

                foreach (var exchange in exchangelist)
                {
                    var exchangeopenlist = resultlist.Where(p => p.exchange == exchange).ToList();//获取当前小时j交易所币对

                    var marklist = exchangeopenlist.GroupBy(p => p.market).Select(p => p.Key).ToList(); //交易所币对列表

                    foreach (var mark in marklist)//循环交易所的币对
                    {
                        var symbolresultlist = exchangeopenlist.Where(p => p.market == mark).ToList();//当前小时当前币对的列表

                        var maxtime = symbolresultlist.Max(p => Convert.ToDateTime(p.times));//当前币对最大时间
                        var mintime = symbolresultlist.Min(p => Convert.ToDateTime(p.times));//当前币对最小时间

                        var ENorder = symbolresultlist.FirstOrDefault(p => Convert.ToDateTime(p.times) == maxtime);
                        var STorder = symbolresultlist.FirstOrDefault(p => Convert.ToDateTime(p.times) == mintime);

                        LQSummarystatistics data = new LQSummarystatistics();
                        data.exchange = exchange;
                        data.market = mark;
                        data.symbol = mark;
                        data.price = STorder.price;

                        data.priceMax = symbolresultlist.Max(a => a.price);
                        data.priceMin = symbolresultlist.Min(a => a.price);
                        data.contractcode = STorder.contractcode;

                        decimal lqbuy = 0;
                        decimal lqsell = 0;
                        data.liquidation = symbolresultlist.Sum(p => p.vol);
                        var lqnow = GetLQ(symbolresultlist);
                        lqnow.TryGetValue("BUY", out lqbuy);
                        lqnow.TryGetValue("SELL", out lqsell);
                        data.liquidation_buy = lqbuy;//多bao==buy
                        data.liquidation_sell = lqsell;//空bao==sell
                        data.timecode = maxtime.ToString("yyyy-MM-dd HH");
                        data.Times = maxtime;
                        data.STimes = mintime;

                        data.kind = STorder.kinds;
                        savelist.Add(data);
                    }
                }
            }
        }


        private Dictionary<string, decimal> GetLQ(List<LiquidationModel> lqdatas)
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
