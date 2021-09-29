using QPP.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class CommandEnum
    {
        //public enum LMESStdWorkFlowState
        //{
        //    Init,
        //    Active,
        //    Hold,
        //    Closed,
        //}
        /// <summary>
        ///   狀態
        /// </summary>
        public class FlowStateX
        {
            public const string Init = "@INIT";
            public const string Active = "@ACTIVE";
            public const string Hold = "@HOLD";
            public const string Closed = "@CLOSED";

        }

        public class RedisKey
        {
            /// <summary>
            /// 大单标记
            /// </summary>
            /// 
            public const string timecheck = "timecheck";

            //redis--订单大单标记
            public const string MaxOrder = "Max";

            //redis--大额爆仓标记
            public const string MaxLQ = "MaxLQ";

            //redis--大额爆仓标记
            public const string BIGMaxLQ = "BIGMaxLQ";

            //redis--爆仓时间key
            public const string LQTimeStamp = "LQTimeStamp";

            public const string bitmexRedis = "DB0";

            public const string bybitRedis = "DB1";

            public const string COIN = "BTC";


            public const string binanceWebscoket = "DB2";

            public const string bitmex = "bitmex";

            public const string bybit = "bybit";

            public const string okex = "okex";

            public const string huobi = "huobi";

            public const string binance = "binance";

            public const string ftx = "ftx";

            public const string Karken = "Karken";

            public const string Deribit = "deribit";

            public const string CoinbasePro = "CoinbasePro";



            //U永续合约
            public const string UPermanentFutures = "UPermanentFutures";

            //U交割合约
            public const string UDeliveryFutures = "UDeliveryFutures";

            //现货key
            public const string USpot = "USpot";


            //持仓类型
            public const string coin = "coin";
            public const string amount = "amount";
            public const string USDT = "USDT";



            //永续
            public const string PERP = "PERP";
            //交割
            public const string DELIVERY = "DELIVERY";
            //现货
            public const string SPOT = "SPOT";

            //强平代号
            public const string liquidation = "liquidation";

            /// <summary>
            /// 费率
            /// </summary>
            public const string FundingRate = "FundingRate";
            /// <summary>
            ///  持仓
            /// </summary>
            public const string OpenInterest = "OpenInterest";


            //redis--持仓时间key
            public const string OpenInterestTimeStamp = "OpenInterestTimeStamp";


            //永续队列
            public const string PERPQueueList = "PERPQueueList";


            //交割队列
            public const string DeliverQueueList = "DeliverQueueList";

            //现货队列
            public const string SpotQueueList = "SpotQueueList";




            //强平队列代号
            public const string liquidationList = "liquidationList";



           //挂单大数据
            public const string BBoQueueList = "BBoQueueList";

            //所有挂单数据
            public const string AllBBoQueueList = "AllBBoQueueList";


            public const string BUY = "BUY";

            public const string SELL = "SELL";

        }

        public class ExchangeData
        {


            public static List<string> ExchangeList = new List<string>() { "bitmex", "bybit", "okex", "huobi", "binance", "ftx", "Karken", "deribit", "CoinbasePro" };

            public static List<string> symbolList = new List<string>() { "BTC", "ETH", "LTC", "BCH", "EOS", "DOT", "UNI", "LINK", "XRP", "ETC", "BSV" };


            public static string BybitCoinUsdlist = "BTCUSD,ETHUSD,EOSUSD,XRPUSD";

            public static string BybitCoinUsdTlist = "BTCUSDT,BCHUSDT,ETHUSDT,LTCUSDT,LINKUSDT,XTZUSDT,ADAUSDT,DOTUSDT,UNIUSDT";



            //public static string BybitAlllist = "BTCUSD,BTCUSDT,ETHUSD,EOSUSD,XRPUSD,BCHUSDT,ETHUSDT,LTCUSDT,LINKUSDT,XTZUSDT";

         //   public static string BybitAlllist = "BTCUSD,BTCUSDT,ETHUSD";
            public static string BybitAlllist = "BTCUSD,ETHUSD,EOSUSD,XRPUSD,BTCUSDT,BCHUSDT,ETHUSDT,LTCUSDT,LINKUSDT,XTZUSDT,ADAUSDT,DOTUSDT,UNIUSDT";

            //            public static List<string> LQsymbolList = new List<string>() {
            //"BTC","ETH","LTC","ADA","XRP","DOT","BNB","EOS","BCH","LINK","TRX","SUSHI","UNI","SOL","BSV","FIL","DOGE","AAVE","XTZ","XLM","SXP","ATOM","YFI","ETC","FTT","DEFI","GRT","COMP","CRV","SRM","KSM","ZEC","ALGO","SHIT","USDT","AVAX","RAY","1INCH","DASH","XMR","RUNE","EGLD","NEO","FTM","ALT","BTMX","IOST","SNX","TOMO","ONT","MATIC","EXCH","VET","THETA","LUNA","ALPHA","BAND","OMG","YFII","HT","RSR","REN","BADGER","IOTA","ZEN","MID","MKR","ZRX","KNC","QTUM","BAL","NEAR","UNISWAP","FLM","OKB","WAVES","BAT","ZIL","KAVA","LEO","CREAM","TRB","ICX","DODO","BZRX","BAGS","OCEAN","LRC","PRIV","LIT","ROOK","BTM","REEF","BEL","CHZ","AKRO","ENJ","SUN","BLZ","STORJ","ANKR","CVC","SAND","SKL","CTK","RLC","HNT","AXS","BTS","XAUT","TRU","PAXG","JST","LINA","ANT","ZKS","BNT","UNFI","FIDA","DRGN","HOLY","RVN","XEM","BTT","LON","AMPL","MASS","MTA","CUSDT","BAO","KIN","TRYB","PERP","SECO","BRZ","WNXM","SWRV","UMA","MANA" };
            public static List<string> LQsymbolList = new List<string>() { "BTC", "ETH", "LTC", "BCH", "EOS", "DOT", "ADA", "LINK", "XRP", "ETC", "BSV", "FIL" };
            //public static List<string> LQsymbolList = new List<string>() { "BTC", "ETH", "LTC" };
           // public static List<string> LQsymbolList = new List<string>() { "BTC", "ETH" };

        }

        public class BybitData
        {
            //获取bybit交易对
            public static List<BybitSymbol> GetContract_size()
            {
                string url = string.Format("https://api.bytick.com/v2/public/symbols");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list.result).ToString().ToList<BybitSymbol>();
                return results;
            }
        }
        public class BybitSymbol
        {

            //name string 合约名称
            //alias string 合约名称
            //status string 合约状态
            //base_currency string 基础货币
            //quote_currency string 报价货币
            //price_scale number  价格范围
            //taker_fee   string taker 手续费
            //maker_fee   string maker 手续费
            //leverage_filter > min_leverage number  最小杠杆数
            //leverage_filter > max_leverage number  最大杠杆数
            //leverage_filter > leverage_step string 杠杆最小增加或减少数量
            //price_filter > min_price string 最小价格
            //price_filter > max_price string 最大价格
            //price_filter > tick_size string 价格最小增加或减少数量
            //lot_size_filter > max_trading_qty number  最大交易数量
            //lot_size_filter > min_trading_qty number  最小交易数量
            //lot_size_filter > qty_step number  合约数量最小单位
          /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string alias { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string base_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string quote_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public long price_scale { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string taker_fee { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string maker_fee { get; set; }
        }



        /// <summary>
        /// 火币永续合约数据获取
        /// </summary>
        public class PerpHUOBI
        {
            //public static List<USDTModel> GetContract_size()
            //{
            //    return USDTcontract_Size.ToList<USDTModel>();

            //}

            public static List<USDTModel> GetContract_sizeapi()
            {
                string url = string.Format("https://api.hbdm.com/linear-swap-api/v1/swap_contract_info");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list.data).ToString().ToList<USDTModel>();
                return results;
            }


            public static List<USDTModel> GetUSDContract_sizeapi()
            {
                string url = string.Format("https://api.hbdm.com/swap-api/v1/swap_contract_info");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list.data).ToString().ToList<USDTModel>();
                return results;
            }



            //public static List<USDTModel> GetUSDContract_size()
            //{
            //    return USDcontract_Size.ToList<USDTModel>();
            //}




       


        }


        public class USDTModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string symbol { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string contract_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public decimal contract_size { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double price_tick { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string delivery_time { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string create_date { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int contract_status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string settlement_date { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string support_margin_mode { get; set; }
        }

        /// <summary>
        /// okex交割数据获取
        /// </summary>
        public class OkexMessage
        {

            /// <summary>
            /// 交割合约信息列表Mdoel
            /// </summary>
            /// <returns></returns>
            public static List<OkexDeliveryModel> GetDeliveryContract_size()
            {
                string url = string.Format("https://www.okex.com/api/futures/v3/instruments");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list).ToString().ToList<OkexDeliveryModel>();
                return results;
            }

            /// <summary>
            /// 永续合约包含USDHE 信息列表Mdoel
            /// </summary>
            /// <returns></returns>
            public static List<OkexSwapModel> GetSwapContract_size()
            {
                string url = string.Format("https://www.okex.com/api/swap/v3/instruments");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list).ToString().ToList<OkexSwapModel>();
                return results;

            }

            /// <summary>
            /// okex永续合约包含USDHE 信息列表Mdoel
            /// SPOT：币币
            //SWAP：永续合约
            //FUTURES：交割合约
            //OPTION：期权
            /// </summary>
            /// <returns></returns>
            public static List<OkexSwapv5> GetSwapContract_size_V5(string type)
            {
                string url = string.Format("https://www.okex.com/api/v5/market/tickers?instType={0}",type);
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list).ToString().ToList<OkexSwapv5>();
                return results;

            }



            /// <summary>
            /// 现货 信息列表Mdoel
            /// </summary>
            /// <returns></returns>
            public static List<OkexSpotModel> GetSpotContract_size()
            {
                string url = string.Format("https://www.okex.com/api/spot/v3/instruments");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list).ToString().ToList<OkexSpotModel>();
                return results;
            }

            

        }

        public class BinanceMessage
        {
            ///// <summary>
            /////  usdt信息列表Mdoel
            ///// </summary>
            ///// <returns></returns>
            //public static List<BinanceUSDTModel> GetSymoblUSDTContract_size()
            //{
            //    //string url = string.Format("https://www.okex.com/api/futures/v3/instruments");
            //    //var list = ApiHelper.GetExt(url);
            //    //var results = ((object)list).ToString().ToList<BinanceUSDTModel>();
            //    //return results;

            //    return USDTcontract_Size.ToList<BinanceUSDTModel>();
            //}

            public static List<BinanceUSDTModel> GetSymoblUSDTContract_sizeapi()
            {
                string url = string.Format("https://fapi.binance.com/fapi/v1/exchangeInfo");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list.symbols).ToString().ToList<BinanceUSDTModel>();
                return results;

                //return USDTcontract_Size.ToList<BinanceUSDTModel>();
            }
            public static List<BinanceUSDTModel> GetSymoblUSDContract_sizeapi()
            {
                string url = string.Format("https://dapi.binance.com/dapi/v1/exchangeInfo");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list.symbols).ToString().ToList<BinanceUSDTModel>();
                return results;
            }
        }
        public class BinanceUSDTModel
        {   /// <summary>
            /// 
            /// </summary>
            public string symbol { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string pair { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string contractType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string deliveryDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string onboardDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string maintMarginPercent { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string requiredMarginPercent { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string baseAsset { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string quoteAsset { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string marginAsset { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string pricePrecision { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string quantityPrecision { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string baseAssetPrecision { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string quotePrecision { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string underlyingType { get; set; }
            /// <summary>
            /// 
            /// </summary>

        }






        /// <summary>
        /// 交割合约信息列表Mdoel
        /// </summary>
        public class OkexDeliveryModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string instrument_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string underlying_index { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string quote_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string tick_size { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string contract_val { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string listing { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string delivery { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string trade_increment { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string @alias { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string underlying { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string base_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string settlement_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string is_inverse { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string contract_val_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string category { get; set; }
        }


        /// <summary>
        /// 永续合约信息列表Mdoel
        /// </summary>
        public class OkexSwapModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string instrument_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string underlying_index { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string quote_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string coin { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string contract_val { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string listing { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string delivery { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string size_increment { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string tick_size { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string base_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string underlying { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string settlement_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string is_inverse { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string category { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string contract_val_currency { get; set; }
        }


        /// <summary>
        /// 现货Model
        /// </summary>
        public class OkexSpotModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string base_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string instrument_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string min_size { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string quote_currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string size_increment { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string category { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string tick_size { get; set; }
        }




        /// <summary>
        /// 保存各节点状态信息
        /// </summary>
        /// <param name="dt">时间</param>
        /// <param name="message">信息</param>
        /// <param name="exchange">交易所</param>
        /// <param name="url">url</param>
        public static void   WriteSaveMessage(DateTime dt, string message, string exchange, string url)
        {
            if ((dt.Minute%10==0)&&(dt.Second%2==0))
            {
                //MailClasss model = new MailClasss();
                //model.dt = dt;
                //model.message = message;
                //model.exchange = exchange;
                //model.url = url;
                //model.ToJson();
                //RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.timecheck + DateTime.Now.ToString("yyyy-MM-dd"));

                RedisHelper.SetOrUpdateHash(CommandEnum.RedisKey.timecheck, url, dt+"---"+message);


            }
        }




        public class Deribit
        {
            public static List<SymbolClass> GetSymbolList()
            {

                //string url = string.Format("https://www.okex.com/api/futures/v3/instruments");
                //var list = ApiHelper.GetExt(url);
                //var results = ((object)list).ToString().ToList<BinanceUSDTModel>();
                //return results;

                return stringsymbol.ToList<SymbolClass>();
            }

            public  static string stringsymbol = "[{\"kind\":\"future\",\"baseCurrency\":\"ETH\",\"currency\":\"USD\",\"minTradeSize\":1.0,\"minTradeAmount\":1,\"instrumentName\":\"ETH-PERPETUAL\",\"isActive\":true,\"settlement\":\"perpetual\",\"created\":\"2019-03-14 13:00:54 GMT\",\"tickSize\":0.05,\"pricePrecision\":2,\"expiration\":\"3000-01-01 08:00:00 GMT\",\"contractSize\":1},{\"kind\":\"future\",\"baseCurrency\":\"BTC\",\"currency\":\"USD\",\"minTradeSize\":1.0,\"minTradeAmount\":10.0,\"instrumentName\":\"BTC-PERPETUAL\",\"isActive\":true,\"settlement\":\"perpetual\",\"created\":\"2018-08-14 10:24:47 GMT\",\"tickSize\":0.5,\"pricePrecision\":1,\"expiration\":\"3000-01-01 08:00:00 GMT\",\"contractSize\":10.0}]";


            public class SymbolClass
            {
                /// <summary>
                /// 
                /// </summary>
                public string kind { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string baseCurrency { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string currency { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public double minTradeSize { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public double minTradeAmount { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string instrumentName { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string isActive { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string settlement { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string created { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public double tickSize { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public int pricePrecision { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string expiration { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public int strike { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string optionType { get; set; }
            }

        }


        public class FTx 
        {

            //获取期货永续交易对
            public static List<Future> GetFutureSymbolList()
            {
                List<Future> results = null;
                string url = string.Format("https://ftx.com/api/futures");
                while (true)
                {
                    try
                    {
                        var list = ApiHelper.GetExtx(url);

                        results = ((object)list.result).ToString().ToList<Future>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("获取获取期货永续交易对数据异常，异常信息：" + e.ToString());
                        Console.WriteLine("再次执行");
                    }
                    if (results != null)
                    {
                        break;
                    }

                }
                return results;
            }



            //获取现货交易对
            public static List<FtxMarket> GetSpotSymbolList()
            {
                string url = string.Format("https://ftx.com/api/markets");
                var list = ApiHelper.GetExtx(url);
                var results = ((object)list.result).ToString().ToList<FtxMarket>();
                return results;
            }
        }


        public class Karkens {
            public static List<Karkeninstruments> GetSymbolList()
            {
                List<Karkeninstruments> results = null;

                string url = string.Format("https://futures.kraken.com/derivatives/api/v3/instruments");
                while (true)
                {
                    try
                    {
                        var list = ApiHelper.GetExtx(url);

                        results = ((object)list.instruments).ToString().ToList<Karkeninstruments>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("获取获取Karken期货永续交易对数据异常，异常信息：" + e.ToString());
                        Console.WriteLine("再次执行");
                    }
                    if (results != null)
                    {
                        break;
                    }

                }
                return results;
            }

        }

        public class coinbasepro {

            public static List<CoinbaseProduct> GetSymbolList()
            {
                List<CoinbaseProduct> results = null;

                string url = string.Format("https://api.pro.coinbase.com/products");
                while (true)
                {
                    try
                    {
                        var list = ApiHelper.GetExtx(url);

                        results = ((object)list).ToString().ToList<CoinbaseProduct>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("获取获取Coinbasepro交易对数据异常，异常信息：" + e.ToString());
                        Console.WriteLine("再次执行");
                    }
                    if (results != null)
                    {
                        break;
                    }

                }
                return results;
            }
        }




        public class MailClasss
        {
            public string url { get; set; }
            public string message { get; set; }
            public DateTime dt { get; set; }
            public string exchange { get; set; }
        }

    }


   
}
