using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class InsterestProcess
    {
        public List<BinanceFuturesSymbol> UpdateSymbolList()
        {

            var dit = RedisHelper.GetAllHash<List<BinanceFuturesSymbol>>("SymbolList");
            var list = dit.Values.FirstOrDefault();
            return list;
        }


        /// <summary>
        /// 根据交易对区分交割 永续 对于小时  天  汇总 得到 区间的价格 持仓量变化
        /// </summary>
        public void CalcInsterestHour(DateTime DT)
        {

            var dit = RedisHelper.GetAllHash<List<BinanceFuturesSymbol>>("SymbolList");
            var symbollist = dit.Values.FirstOrDefault();
            //DateTime st = DateTime.Now.AddHours(-1);
            DateTime st = DT;
            DateTime end = DateTime.Now;
            string types = CommandEnum.RedisKey.PERP;
            List<OpenInsterestData> savelist = new List<OpenInsterestData>();



            var datelist = TimeCore.GetStartAndEndTime(st, end);
            //var datelist = TimeCore.GetHourList(end);

            CRDataOut t = new CRDataOut();
            var openlist = t.GetDataObject<OpenInterest>(CommandEnum.RedisKey.OpenInterest + st.ToString("yyyy-MM-dd"));

            foreach (var hour in datelist)
            {
                var resultopendata = openlist.Where(p =>p!=null&& Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == hour.ToString("yyyy-MM-dd HH")).ToList();

                //var resultopendata = openlist.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH") == hour.ToString("yyyy-MM-dd HH") && p.kind == types).ToList();
                //var resultopendata = openlist.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd") == hour.ToString("yyyy-MM-dd")).ToList();
                if (resultopendata.Count == 0)
                {
                    continue;
                }

                ProcessSavelist(symbollist, savelist, resultopendata);
            }

            foreach (var item in savelist)
            {
                RedisHelper.SetOrUpdateHash(item.kind + "OpenInsertDataHour" + item.Times.ToString("yyyy-MM-dd"), item.exchange + "_" + item.timecode + "_" + item.market, item.ToJson());
            }

        }

        private static void ProcessSavelist(List<BinanceFuturesSymbol> symbollist, List<OpenInsterestData> savelist, List<OpenInterest> resultopendata)
        {
            foreach (var symbol in symbollist)
            {
                var resultlist = resultopendata.Where(p => p.symbol.ToUpper().Contains(symbol.BaseAsset.ToUpper())).ToList();//获取当前小时币对
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
                        var symbolresultlist = exchangeopenlist.Where(p => p.market == mark && p.SumOpenInterestValue > 0).ToList();//当前小时当前币对的列表
                        if (symbolresultlist.Count < 2)
                        {
                            continue;
                        }

                        var maxtime = symbolresultlist.Max(p => Convert.ToDateTime(p.times));//当前币对最大时间
                        var mintime = symbolresultlist.Min(p => Convert.ToDateTime(p.times));//当前币对最小时间

                        var ENorder = symbolresultlist.FirstOrDefault(p => Convert.ToDateTime(p.times) == maxtime);
                        var STorder = symbolresultlist.FirstOrDefault(p => Convert.ToDateTime(p.times) == mintime);

                        OpenInsterestData data = new OpenInsterestData();
                        data.exchange = exchange;
                        data.market = mark;
                        data.symbol = mark;
                        data.priceST = STorder.price;
                        data.priceEN = ENorder.price;
                        data.priceMax = symbolresultlist.Max(a => a.price);
                        data.priceMin = symbolresultlist.Min(a => a.price);
                        // (当前最新成交价（或 收盘价）-开盘参考价)÷开盘参考价×100 %。

                        //var sq = (Math.Round((((data.priceEN - data.priceST) / data.priceST)*100),2)).ToString() + "%"; ;

                        data.pricePrecent = ((Math.Round(((data.priceEN - data.priceST) / data.priceST), 4)) * 100).ToString() + "%";
                        data.contractcode = symbol.BaseAsset.ToUpper();
                        data.timecode = maxtime.ToString("yyyy-MM-dd HH");
                        data.Times = maxtime;
                        data.STimes = mintime;
                        data.SumOpenInterestValueST = STorder.SumOpenInterestValue;
                        data.SumOpenInterestValueEn = ENorder.SumOpenInterestValue;
                        data.SumOpenInterestValueMax = symbolresultlist.Max(a => a.SumOpenInterestValue);
                        data.SumOpenInterestValueMin = symbolresultlist.Min(a => a.SumOpenInterestValue);
                        //data.SumOpenInterestPrecent = (((data.SumOpenInterestValueEn - data.SumOpenInterestValueST) / data.SumOpenInterestValueST) * 100).ToString() + "%";
                        data.SumOpenInterestPrecent = ((Math.Round(((data.SumOpenInterestValueEn - data.SumOpenInterestValueST) / data.SumOpenInterestValueST), 4)) * 100).ToString() + "%";

                        data.STcoin = STorder.coin;
                        data.ENcoin = ENorder.coin;
                        //data.CoinPrecent = (((data.ENcoin - data.STcoin) / data.STcoin) * 100).ToString() + "%";
                        data.CoinPrecent = ((Math.Round(((data.ENcoin - data.STcoin) / data.STcoin), 4)) * 100).ToString() + "%";
                        data.kind = STorder.kind;
                        savelist.Add(data);
                    }
                }
            }
        }

        public void CalcInsterestDay()
        {
            var dit = RedisHelper.GetAllHash<List<BinanceFuturesSymbol>>("SymbolList");
            var symbollist = dit.Values.FirstOrDefault();

            //DateTime st = new DateTime(2021, 04, 12);
            DateTime st = DateTime.Now.AddDays(-1);
            DateTime end = DateTime.Now;
            string types = CommandEnum.RedisKey.PERP;
            List<OpenInsterestData> savelist = new List<OpenInsterestData>();


            var list = TimeCore.GetDate(st, end);
            foreach (var date in list)
            {
                //var datelist = TimeCore.GetHourList(date);
                CRDataOut t = new CRDataOut();
                var openlist = t.GetDataObject<OpenInterest>(CommandEnum.RedisKey.OpenInterest + date.ToString("yyyy-MM-dd"));
                //var resultopendata = openlist.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd") && p.kind == types).ToList();
                var resultopendata = openlist.Where(p => p!=null&&Convert.ToDateTime(p.times).ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd")).ToList();

                if (resultopendata.Count == 0)
                {
                    continue;
                }

                ProcessSavelist(symbollist, savelist, resultopendata);
            }

            foreach (var item in savelist)
            {
                RedisHelper.SetOrUpdateHash(item.kind + "OpenInsertDataDate" + item.Times.ToString("yyyy-MM-dd"), item.exchange + "_" + item.market, item.ToJson());
            }

        }




        public void getOpeninsterestData()
        {
            List<OpenInsterestDataMail> savelist = new List<OpenInsterestDataMail>();

            var item = DateTime.Now;


            //永续持仓小时
            var perplisthour = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataHour);
            //永续持仓日
            var perpdatalist = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataDate);
            //永续持仓小时
            var deliverhour = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataHour);
            //永续持仓日
            var deliverdate = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataDate);


            if (perplisthour.Count > 0)
            {
                perplisthour.ForEach(a => a.types = CommandEnum.RedisKey.PERP + "-" + CommandEnum.RedisKey.OpenInterestDataHour);
                savelist.AddRange(perplisthour.Where(a=> Math.Abs(a.pricePrecents)>5&& Math.Abs(a.SumOpenInterestPrecents)>5));
            }

            if (perpdatalist.Count > 0)
            {
                perpdatalist.ForEach(a => a.types = CommandEnum.RedisKey.PERP + "-" + CommandEnum.RedisKey.OpenInterestDataDate);
                savelist.AddRange(perpdatalist.Where(a => Math.Abs(a.pricePrecents) > 5 && Math.Abs(a.SumOpenInterestPrecents) > 5));
            }

            if (deliverhour.Count > 0)
            {
                deliverhour.ForEach(a => a.types = CommandEnum.RedisKey.DELIVERY + "-" + CommandEnum.RedisKey.OpenInterestDataHour);
                savelist.AddRange(deliverhour.Where(a => Math.Abs(a.pricePrecents) > 5 && Math.Abs(a.SumOpenInterestPrecents) > 5));
            }

            if (deliverdate.Count > 0)
            {
                deliverdate.ForEach(a => a.types = CommandEnum.RedisKey.DELIVERY + "-"+CommandEnum.RedisKey.OpenInterestDataDate);
                savelist.AddRange(deliverdate.Where(a => Math.Abs(a.pricePrecents) > 5 && Math.Abs(a.SumOpenInterestPrecents) > 5));
            }

        }


        //按天保存
        public List<T> GetList<T>(DateTime start, string key)
        {


            List<T> maxlist = new List<T>();
            var tablename = key + start.ToString("yyyy-MM-dd");

            maxlist = RedisHelper.GetAllHashList<T>(tablename);

            return maxlist;
        }

    }
}
