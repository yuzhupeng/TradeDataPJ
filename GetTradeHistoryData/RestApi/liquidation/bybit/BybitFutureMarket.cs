using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetTradeHistoryData 
{
   public class BybitFutureMarket
    {
        private const string recentTradesEndpoint = "trades";
        private const string historicalTradesEndpoint = "historicalTrades";
        private const string markPriceEndpoint = "premiumIndex";
        private const string price24HEndpoint = "ticker/24hr";
        private const string bookPricesEndpoint = "ticker/bookTicker";
        private const string allForcedOrdersEndpoint = "allForceOrders";
        private const string openInterestEndpoint = "openInterest";    
        private const string takerBuySellVolumeRatioEndpoint = "takerlongshortRatio";
        private const string compositeIndexApi = "indexInfo";
        private const string klinesEndpoint = "klines";
        private const string tradingDataApi = "futures/data";


        private const string symbols= "/symbols";
        private const string openInterestHistoryEndpoint = "/open-interest";
        private const string ticket = "/tickers";

        private const string BybitFUTURESUSDTMARKET_DEFAULT_HOST = "https://api.bytick.com/v2/public";




        /// <summary>
        /// 获取持仓信息
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="period">The period timespan</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Start time to get open interest history</param>
        /// <param name="endTime">End time to get open interest history</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Open Interest History info</returns>
        private List<BybitFuturesOpenInterestHistory> GetOpenInterestHistoryAsync(string symbol, string period="200", int? limit = null)
        {

            limit = 200;
            //long st = 0;
            //long end = 0;


            //if (startTime == null)
            //{
            //    st = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            //}
            //if (endTime == null)
            //{
            //    end = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            //}

            string location = $"?symbol={symbol}&&limit={limit}&period=5min";

            string url = BybitFUTURESUSDTMARKET_DEFAULT_HOST + openInterestHistoryEndpoint + location;

            List<BybitFuturesOpenInterestHistory> results = null;

            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);
                    results = ((object)list.result).ToString().ToList<BybitFuturesOpenInterestHistory>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取bybit持仓信息数据异常，异常信息：" + e.ToString());
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
        /// 获取费率
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="startTime">Start time to get funding rate history</param>
        /// <param name="endTime">End time to get funding rate history</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The funding rate history for the provided symbol</returns>
        private List<BybitFuturetickers> GetFundingRatesAsync()
        {
            //limit = 500;
            //long st = 0;
            //long end = 0;


            //if (startTime == null)
            //{
            //    st = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            //}
            //if (endTime == null)
            //{
            //    end = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            //}

             

            string url = BybitFUTURESUSDTMARKET_DEFAULT_HOST + ticket;

            List<BybitFuturetickers> results = null;

            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);
                    results = ((object)list.result).ToString().ToList<BybitFuturetickers>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取BYBIT费率信息数据异常，异常信息：" + e.ToString());
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
        /// 获取交易对列表
        /// </summary>
        /// <returns></returns>
        public static List<BybitSymbol> GetSymbolFutureList()
        {
            string url = BybitFUTURESUSDTMARKET_DEFAULT_HOST + symbols;
            List<BybitSymbol> results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);
                    results = ((object)list.result).ToString().ToList<BybitSymbol>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取bybit交易对数据异常，异常信息：" + e.ToString());
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
        /// 获取交易对
        /// </summary>
        /// //TYPE: USD/USDT
        /// <returns></returns>
        public static string GetSymbollist(string type)
        {
            List<BybitSymbol> list = null;
            if (type == "ALL")
            {
                list = GetSymbolFutureList().ToList();
            }
            else 
            {
                list = GetSymbolFutureList().Where(p => p.quote_currency == type).ToList();
            }
            string result = "";
            if (list.Count() != 0)
            {
                result = string.Join(",", list.Select(p => p.name).ToList());
            }
            else
            {
                result = "BTC,ETH";
            }
            return result;

        }



        /// <summary>
        /// 根据使用symbol获取费率和持仓
        /// 汇总时获取当前小时中时间最大和最小的值记录标记为开始和结束
        /// </summary>
        public void GetFundingRateAndOpenInterest()
        {

            List<FundRate> FundRatelist = new List<FundRate>();
            List<OpenInterest> OpenInterestlist = new List<OpenInterest>();
            string messagetype = "bybit持仓和费率";
             var symbollist = GetSymbolFutureList();
            List<BybitFuturetickers> fundingratelist = new List<BybitFuturetickers>();

            fundingratelist = GetFundingRatesAsync();//获取当前资金费和下期费率，和未平仓仓位
                                                                                //交易所    交易对
    
            //费 
            foreach (var item in fundingratelist)
             {                  
                    //List<BybitFuturesOpenInterestHistory> openinserthistorylist = new List<BybitFuturesOpenInterestHistory>();
                    //openinserthistorylist = GetOpenInterestHistoryAsync(item.symbol.ToString());                 
                    FundRate i = new FundRate();
                    i.market = item.symbol;
                    i.exchange = CommandEnum.RedisKey.bybit;
                    i.FundingRate = item.funding_rate.ToString() ;
                    i.symbol = item.symbol;
                    i.times = DateTime.Now.ToString(); 
                    i.NextFundingRate = item.predicted_funding_rate.ToString();              
                    FundRatelist.Add(i);



                OpenInterest c = new OpenInterest();
                c.market = item.symbol;
                c.exchange = CommandEnum.RedisKey.bybit;
                c.symbol = item.symbol;
                c.times = DateTime.Now.ToString();      
                c.type = CommandEnum.RedisKey.USDT;
                c.desc = "USDT";
                if (item.symbol.Contains("21"))
                {
                    c.kind = CommandEnum.RedisKey.DELIVERY;
                }
              
                c.price = item.last_price;
                if (item.symbol.Contains("USDT"))
                {
                    c.volumeUsd24h = item.turnover_24h;
                    c.SumOpenInterest = item.open_interest;
                    c.SumOpenInterestValue = item.open_interest*item.last_price;
                    c.coin = item.open_interest;
                }
                else
                {
                    c.SumOpenInterest = item.open_interest;
                    c.SumOpenInterestValue = item.open_interest;
                    c.coin = item.open_interest / item.last_price;
                    c.volumeUsd24h = item.volume_24h;
                }
                OpenInterestlist.Add(c);
            }

 



           
            RedisHelper.Pushdata(FundRatelist.ToJson(), "11", CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd"));
            RedisHelper.Pushdata(OpenInterestlist.ToJson(), "11", CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd"));
            Console.WriteLine("bybit 结束持仓和费率数据获取：" + CommandEnum.RedisKey.bybit+DateTime.Now.ToString()+ messagetype);

        }


    }
}
//持 
//foreach (var item in symbollist)
//{
//    List<BybitFuturesOpenInterestHistory> openinserthistorylist = new List<BybitFuturesOpenInterestHistory>();
//    openinserthistorylist = GetOpenInterestHistoryAsync(item.name.ToString());
//    //  var times = RedisHelper.GetHash(CommandEnum.RedisKey.LQTimeStamp, key + "_" + symbol);
//    var openinsettimes = RedisHelper.GetHash(CommandEnum.RedisKey.OpenInterestTimeStamp, CommandEnum.RedisKey.bybit + "_" + item.name.ToString());
//    if (openinsettimes != "")
//    {
//        openinserthistorylist = openinserthistorylist.Where(p => p.timestamp > Convert.ToDateTime(openinsettimes)).ToList();
//    }

//    if (openinserthistorylist.Count() > 0)
//    {
//        foreach (var insert in openinserthistorylist)
//        {
//            OpenInterest i = new OpenInterest();
//            i.market = item.name;
//            i.exchange = CommandEnum.RedisKey.bybit;
//            i.SumOpenInterest = insert.open_interest;
//            i.symbol = item.name;
//            i.times = insert.timestamp.ToString();
//            i.SumOpenInterestValue = insert.open_interest;
//            i.type = CommandEnum.RedisKey.USDT;
//            i.desc = "USDT";
//            if (item.name.Contains("21"))
//            {
//                i.kind = CommandEnum.RedisKey.DELIVERY;
//            }


//            OpenInterestlist.Add(i);
//        }
//    }

//    var maxtimestamp = openinserthistorylist.Max(a => a.timestamp).ToString();

//    RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.OpenInterestTimeStamp, CommandEnum.RedisKey.bybit + "_" + item.name, maxtimestamp);




//}