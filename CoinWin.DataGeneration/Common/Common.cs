using System;
using System.Collections.Generic;
using System.Text;

namespace CoinWin.DataGeneration
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
            public const string timecheck = "timecheck";

            //redis--订单大单标记
            public const string MaxOrder = "Max";

            //redis--大额爆仓标记
            public const string MaxLQ = "MaxLQ";

            //redis--大额爆仓标记
            public const string BIGMaxLQ = "BIGMaxLQ";

            //强平代号
            public const string liquidation = "liquidation";


            public const string bitmexRedis = "DB0";

            public const string bybitRedis = "DB1";

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


            public const string coin = "BTC";

            public const string BUY = "BUY";

            public const string SELL = "SELL";
            //永续队列
            public const string PERPQueueList = "PERPQueueList";
            //交割队列
            public const string DeliverQueueList = "DeliverQueueList";

            //现货队列
            public const string SpotQueueList = "SpotQueueList";

            //所有挂单数据
            public const string AllBBoQueueList = "AllBBoQueueList";


            //所有大型挂单成交数据
            public const string BBoBigDataList = "BBoBigDataList";


            //现货
            public const string SPOT = "SPOT";

            //永续
            public const string PERP = "PERP";
            //交割
            public const string DELIVERY = "DELIVERY";



            //强平队列代号
            public const string liquidationList = "liquidationList";
            //永续实时成交
            public const string PERPRealDeal = "PERPRealDeal";
            //交割实时成交
            public const string DeliverRealDeal = "DeliverRealDeal";

            //永续实时成交
            public const string SPOTRealDeal = "SPOTRealDeal";

            //
            public const string ALLCoinALLExchangeDeal = "ALLCoinALLExchangeDeal";

 

            /// <summary>
            /// 费率
            /// </summary>
            public const string FundingRate = "FundingRate";
            /// <summary>
            ///  持仓
            /// </summary>
            public const string OpenInterest = "OpenInterest";



            /// <summary>
            ///  持仓小时
            /// </summary>
            public const string OpenInterestDataHour = "OpenInsertDataHour";


            /// <summary>
            ///  持仓日
            /// </summary>
            public const string OpenInterestDataDate = "OpenInsertDataDate";



            //强平数据汇总小时
            public const string liquidationHour = "liquidationHour";

            //强平数据汇总日
            public const string liquidationDate = "liquidationDate";
        }

        public class ExchangeData
        {

            public static List<string> ExchangeList = new List<string>() { "bitmex", "bybit", "okex", "huobi", "binance", "ftx", "Karken", "deribit", "CoinbasePro" };

            public static List<string> symbolList = new List<string>() { "BTC", "ETH", "LTC", "BCH", "EOS", "DOT", "UNI", "LINK", "XRP" };

        }

    }
}
