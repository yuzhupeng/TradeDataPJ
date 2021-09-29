using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class BinanceMarkPrice
    {
        /// <summary>
        /// Symbol
        /// </summary>

        public string symbol { get; set; }


        public string pair { get; set; }

        /// <summary>
        /// Mark Price
        /// </summary>

        public decimal markPrice { get; set; }

        /// <summary>
        /// Mark Price
        /// </summary>

        public decimal IndexPrice { get; set; }

        public decimal? FundingRate { get; set; }

        [JsonConverter(typeof(BJTimestampConverter))]
        public DateTime NextFundingTime { get; set; }

        public decimal estimatedSettlePrice { get; set; }

        public decimal lastFundingRate { get; set; }

        public decimal interestRate { get; set; }

        [JsonConverter(typeof(BJTimestampConverter))]
        public DateTime time { get; set; }
    }
}


//{
//     "symbol": "BTCUSD_PERP",    // 交易对
//     "pair": "BTCUSD",           // 基础标的
//     "markPrice": "11029.69574559",  // 标记价格
//     "indexPrice": "10979.14437500", // 指数价格
//     "estimatedSettlePrice": "10981.74168236",  // 预估结算价,仅在交割开始前最后一小时有意义
//     "lastFundingRate": "0.00071003",      // 最近更新的资金费率,只对永续合约有效，其他合约返回""
//     "interestRate": "0.00010000",       // 标的资产基础利率,只对永续合约有效，其他合约返回""
//     "nextFundingTime": 1596096000000,    // 下次资金费时间，只对永续合约有效，其他合约返回0
//     "time": 1596094042000   // 更新时间
// },