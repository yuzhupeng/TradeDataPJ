using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTradeHistoryData
{
    public class BinanceFuturesUsdtMarket
    {
        private const string recentTradesEndpoint = "trades";
        private const string historicalTradesEndpoint = "historicalTrades";
 
        private const string price24HEndpoint = "ticker/24hr";
        private const string bookPricesEndpoint = "ticker/bookTicker";
        private const string allForcedOrdersEndpoint = "allForceOrders";
        private const string openInterestEndpoint = "openInterest";
        private const string takerBuySellVolumeRatioEndpoint = "takerlongshortRatio";
        private const string compositeIndexApi = "indexInfo";
        private const string klinesEndpoint = "klines";
        private const string tradingDataApi = "futures/data";
        private const string exchangeInfo = "fapi/v1/exchangeInfo";


        //USDT
        //合约持仓量按时间点5min,1H（包含价值）
        private const string USDTopenInterestHistoryEndpoint = "futures/data/openInterestHist";
 
        //获取当前费率
        private const string USDTmarkPriceEndpoint = "/fapi/v1/premiumIndex";

        //未使用
        ///fapi/v1/openInterest 实时获取未平仓数
        private const string USDTopenInterest = "/fapi/v1/openInterest";
        //获取历史费率
        private const string USDTfundingRateHistoryEndpoint = "/fapi/v1/fundingRate";




        //USD
        //合约持仓量按时间点5min,1H（包含价值）
        private const string USDopenInterestHistoryEndpoint = "futures/data/openInterestHist";
        //获取当前费率
        private const string USDmarkPriceEndpoint = "/dapi/v1/premiumIndex";
        //未使用
        ///fapi/v1/openInterest 实时获取未平仓数
        private const string USDopenInterest = "/dapi/v1/openInterest";
        //获取历史费率
        private const string USDfundingRateHistoryEndpoint = "/dapi/v1/fundingRate";
       
 




        //usdt
        private const string BINANCEFUTURESUSDTMARKET_DEFAULT_HOST = "https://fapi.binance.com/";

        //usd
        private const string BINANCEFUTURESUSDMARKET_DEFAULT_HOST = "https://dapi.binance.com/";



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
        public List<BinanceFuturesOpenInterestHistory> GetOpenInterestHistoryAsync(string urls,string symbol, string period,string type, string contractype = "", int? limit = null, DateTime? startTime = null, DateTime? endTime = null)
        {

            limit = 1;
            long st = 0;
            long end = 0;
            System.DateTime startTimes = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            TimeSpan ts = DateTime.Now.AddMinutes(-5) - startTimes;         
            st = Convert.ToInt64(ts.TotalMilliseconds);

            
             
            if (startTime != null)
            {
                st = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            }
     

            if (endTime != null)
            {
                end = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            }


            
            //&&start_time={startTime}&&end_time={endTime}
            string url = "";
;            if (type == "USDT")
            {
                string location = $"?symbol={symbol}&&limit={limit}&&period=5m&&start_time={st}";
                url = urls + USDTopenInterestHistoryEndpoint + location;
            }
            else
            {
                string location = $"?pair={symbol}&&contractType={contractype}&&limit={limit}&&period=5m&&start_time={st}";
                url = urls + USDopenInterestHistoryEndpoint + location;
            }

            List<BinanceFuturesOpenInterestHistory> results = null;

            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtbinance(url);
                    results = ((object)list).ToString().ToList<BinanceFuturesOpenInterestHistory>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取币安持仓信息数据异常，异常信息：" + e.ToString());
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
        /// 获取当前费率
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="startTime">Start time to get funding rate history</param>
        /// <param name="endTime">End time to get funding rate history</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The funding rate history for the provided symbol</returns>
        public List<BinanceMarkPrice> GetFundingRatesAsync(string URL, string type = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null )
        {
            limit = 500;
            long st = 0;
            long end = 0;

            if (startTime != null)
            {
                st = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            }
            if (endTime != null)
            {
                end = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            }

            string location = $"?limit={limit}";
            //string location = $"?symbol={symbol}&&limit={limit}&&period=5m";
            //&&start_time={startTime}&&end_time={endTime}
            //string url = URL + USDTmarkPriceEndpoint + location;


            string url = "";
             if (type == "USDT")
            {
                url = URL + USDTmarkPriceEndpoint + location;
            }
            else
            {
                url = URL + USDmarkPriceEndpoint + location;
            }

            List<BinanceMarkPrice> results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtbinance(url);

                    results = (((object)list).ToString()).ToList<BinanceMarkPrice>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取币安费率信息数据异常，异常信息：" + e.ToString());
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
        /// 获取历史费率
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="startTime">Start time to get funding rate history</param>
        /// <param name="endTime">End time to get funding rate history</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The funding rate history for the provided symbol</returns>
        public List<BinanceFuturesUsdtFundingRateHistory> GetFundingRatesHistory(string URL, string symbol, string type, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            limit = 500;
            long st = 0;
            long end = 0;


            if (startTime != null)
            {
                st = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            }
            if (endTime != null)
            {
                end = GZipDecompresser.ToUnixTimestamp(startTime.Value);
            }
            string location = $"?symbol={symbol}&&limit={limit}";
            //string location = $"?symbol={symbol}&&limit={limit}&&period=5m";
            //&&start_time={startTime}&&end_time={endTime}

            //string url = URL + fundingRateHistoryEndpoint + location;


            string url = "";
            if (type == "USDT")
            {
                url = URL + USDTfundingRateHistoryEndpoint + location;
            }
            else
            {
                url = URL + USDfundingRateHistoryEndpoint + location;
            }


            List<BinanceFuturesUsdtFundingRateHistory> results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);
                    results = ((object)list).ToString().ToList<BinanceFuturesUsdtFundingRateHistory>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取币安费率信息数据异常，异常信息：" + e.ToString());
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
        public static List<BinanceFuturesSymbol> GetSymbolFutureList()
        {
            string url = BINANCEFUTURESUSDTMARKET_DEFAULT_HOST + exchangeInfo;
            List<BinanceFuturesSymbol> results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtx(url);
                    results = ((object)list.symbols).ToString().ToList<BinanceFuturesSymbol>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取币安交易对数据异常，异常信息：" + e.ToString());
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
        public static List<TFhrTicket> GetTFHrTicket(string type)
        {
        
            string usd = "/dapi/v1/ticker/24hr";
            string usdt = "/fapi/v1/ticker/24hr";
            string url = "";
        
                if (type == "USD")
                {
                    url = BINANCEFUTURESUSDMARKET_DEFAULT_HOST + usd;
                }
                else
                {
                    url = BINANCEFUTURESUSDTMARKET_DEFAULT_HOST + usdt;
                }


           


    
            List<TFhrTicket> results = null;

            var list = ApiHelper.GetExtx(url);
            results = ((object)list).ToString().ToList<TFhrTicket>();
            return results;
        
        }



        /// <summary>
        /// 获取交易对
        /// </summary>
        /// <returns></returns>
        public static string GetSymbollist()
        {
            string result = "";
            try
            {
                var list = GetSymbolFutureList();
               
                if (list.Count() != 0)
                {
                    result = string.Join(",", list.Select(p => p.symbol).ToList());
                }
                else
                {
                    result = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,UNI,DOGE,ADA,TRX,BNB";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("获取交易对错误，错误信息：" + e.ToString());
            }
            return result;

        }




        /// <summary>
        /// 根据使用symbol获取费率和持仓
        /// </summary>
        public void GetFundingRateAndOpenInterest(string url,string type)
        {
            try
            {
                string messagetpye = "";
                List<FundRate> FundRatelist = new List<FundRate>();
                List<OpenInterest> OpenInterestlist = new List<OpenInterest>();
                List<CommandEnum.BinanceUSDTModel> symbollist = new List<CommandEnum.BinanceUSDTModel>();
                List<TFhrTicket> ticketlist = new List<TFhrTicket>();
                if (type == "USDT")
                {
                    messagetpye = "U本位持仓费率信息";
                    symbollist = CommandEnum.BinanceMessage.GetSymoblUSDTContract_sizeapi();
                    ticketlist = GetTFHrTicket("USDT");
                }
                else
                {
                    messagetpye = "币本位持仓费率信息";
                    symbollist = CommandEnum.BinanceMessage.GetSymoblUSDContract_sizeapi();
                    ticketlist = GetTFHrTicket("USD");
                }

                //var  symbollistS = GetSymbolFutureList();
                if (symbollist == null || symbollist.Count == 0)
                {
                    Console.WriteLine("获取交易对新失败或者为0，无法进行获取费率信息！");
                    return;
                }

                //费率
                List<BinanceMarkPrice> fundingratelist = new List<BinanceMarkPrice>();

                fundingratelist = GetFundingRatesAsync(url, type);

                if (fundingratelist.Count == 0)
                {
                    Console.WriteLine("获取费率为0，无法进行保存费率持仓信息！");
                    return;
                }
                foreach (var funs in fundingratelist)
                {
                    if (funs.lastFundingRate == 0)
                    {
                        continue;
                    }
                    FundRate i = new FundRate();
                    i.market = funs.symbol;
                    i.exchange = CommandEnum.RedisKey.binance;
                    i.FundingRate = funs.lastFundingRate.ToString();
                    i.symbol = funs.symbol;
                    i.times = funs.time.ToString();
                    i.NextFundingRate = funs.lastFundingRate.ToString();
                    i.NextFundingRateTime = funs.NextFundingTime.ToString();
                    i.timestamp = funs.time.ToString();
                    FundRatelist.Add(i);
                }

                foreach (var item in symbollist)
                {
                
                    //持仓
                    List<BinanceFuturesOpenInterestHistory> openinserthistorylist = new List<BinanceFuturesOpenInterestHistory>();
                    if (type == "USDT")
                    {
                        openinserthistorylist = GetOpenInterestHistoryAsync(url, item.symbol, "", type);
                    }
                    else
                    {
                        openinserthistorylist = GetOpenInterestHistoryAsync(url, item.pair, "", type, item.contractType);
                    }
                    if (openinserthistorylist.Count == 0)
                    {
                        Console.WriteLine("获取持仓为0，无法进行保存费率持仓信息！");
                        return;
                    }

                    //var openinsettimes = RedisHelper.GetHash(CommandEnum.RedisKey.OpenInterestTimeStamp, CommandEnum.RedisKey.binance + "_" + item.symbol);
                    //if (openinsettimes != "")
                    //{
                    //    openinserthistorylist = openinserthistorylist.Where(p => p.Timestamp > Convert.ToDateTime(openinsettimes)).ToList();
                    //}

                    foreach (var opens in openinserthistorylist)
                    {
                        OpenInterest i = new OpenInterest();                 
                        i.exchange = CommandEnum.RedisKey.binance;
                        i.SumOpenInterest = opens.SumOpenInterest;
                    
                        i.times = opens.Timestamp.ToString();
                        var prices= fundingratelist.FirstOrDefault(p => p.symbol == item.symbol).markPrice;
                        i.price = prices;

                        if (type == "USDT")
                        {
                            i.market = opens.Symbol == "" ? opens.pair : opens.Symbol;
                            i.symbol = opens.Symbol == "" ? opens.pair : opens.Symbol;
                            i.SumOpenInterestValue = opens.SumOpenInterestValue;
                            i.coin = opens.SumOpenInterestValue/ prices;

                            if (ticketlist.FirstOrDefault(a => a.symbol == i.symbol) != null)
                            {
                                i.volumeUsd24h = ticketlist.FirstOrDefault(a => a.symbol == item.symbol).quoteVolume;

                            }
                        }
                        else
                        {
                            i.market = item.symbol;
                            i.symbol = item.symbol;
                            i.SumOpenInterestValue = opens.SumOpenInterestValue * prices;
                            i.coin = opens.SumOpenInterestValue;

                            if (ticketlist.FirstOrDefault(a => a.symbol == item.symbol) != null)
                            {
                                if (item.symbol.ToLower().Contains("btc"))
                                {
                                    i.volumeUsd24h = ticketlist.FirstOrDefault(a => a.symbol == item.symbol).volume*100;
                                }
                                else
                                {
                                    i.volumeUsd24h = ticketlist.FirstOrDefault(a => a.symbol == item.symbol).volume*10;
                                }
                            }

                        }
                        //i.SumOpenInterestValue = opens.SumOpenInterestValue;
                        i.type = CommandEnum.RedisKey.coin;
                        i.desc = "binance 持仓币";

                        if (item.contractType == "PERPETUAL")
                        {
                            i.kind = CommandEnum.RedisKey.PERP;
                        }
                        else
                        {
                            i.kind = CommandEnum.RedisKey.DELIVERY;
                        }
                
                        OpenInterestlist.Add(i);
                    }

                    //var maxtimestamp = openinserthistorylist.Max(a => a.Timestamp).ToString();
                    //RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.OpenInterestTimeStamp, CommandEnum.RedisKey.binance + "_" + item.symbol, maxtimestamp);


                }
                //var maxtimestamp = list.Max(a => a.times).ToString();

                RedisHelper.Pushdata(FundRatelist.ToJson(), "11", CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd"));
                RedisHelper.Pushdata(OpenInterestlist.ToJson(), "11", CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd"));
                Console.WriteLine("binance 结束持仓和费率数据获取：" + CommandEnum.RedisKey.binance + DateTime.Now.ToString()+ messagetpye);
            }
            catch (Exception e)
            {
                Console.WriteLine("binance 持仓和费率数据获取失败：" + CommandEnum.RedisKey.binance + DateTime.Now.ToString()+e.Message.ToString());
            }

        }


        public void runtest()
        {
            //binance
          string BINANCEFUTURESUSDTMARKET_DEFAULT_HOST = "https://fapi.binance.com/";
          string BINANCEFUTURESUSDMARKET_DEFAULT_HOST = "https://dapi.binance.com/";
  
          GetFundingRateAndOpenInterest(BINANCEFUTURESUSDMARKET_DEFAULT_HOST, "USD");
          GetFundingRateAndOpenInterest(BINANCEFUTURESUSDTMARKET_DEFAULT_HOST, "USDT");
       
        }


        //private List<T> GetData<T>(string symbol, int limit, long start_time, long end_time)
        //{
        //    // location
        //    //string location = $"/linear-swap-api/v1/swap_liquidation_orders?contract_code={contractCode}&&trade_type={tradeType}&&create_date={createDate}";

        //    string location = symbol + $"&&limit={limit}&&start_time={start_time}&&end_time={end_time}";

        //    BybitLQ results = null;


        //    string url = BYBIT_DEFAULT_HOST;
        //    url += location;

        //    while (true)
        //    {
        //        try
        //        {
        //            var list = ApiHelper.GetByProxy(url);

        //            results = ((object)list).ToString().ToObject<BybitLQ>();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("获取火币强平订单数据异常，异常信息：" + e.ToString());
        //            Console.WriteLine("再次执行");
        //        }
        //        if (results != null)
        //        {
        //            break;
        //        }

        //    }
        //    return results;
        //}
    }
}
