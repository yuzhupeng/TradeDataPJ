using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GetTradeHistoryData 
{
   public class FtxFundingRateOpenInstert
    {
        /// <summary>
        /// 最近24小时内交易的数量
        /// </summary>
        public decimal volume { get; set; }
        /// <summary>
        /// 后续融资费率（只适用于永续合约）
        /// </summary>
        [JsonConverter(typeof(DeciamlConverter))]
        public decimal nextFundingRate { get; set; }
        /// <summary>
        /// 2019-03-29T03:00:00+00:00	后续融资时间（仅适用于永续合约）
        /// </summary>
        public string nextFundingTime { get; set; }
        /// <summary>
        /// 3992.1	期货到期的价格（仅在期货到期时适用）
        /// </summary>
        public decimal expirationPrice { get; set; }
        /// <summary>
        /// number	3993.0	仅适用于期货未到期的情况
        /// </summary>
        public decimal predictedExpirationPrice { get; set; }
        /// <summary>
        ///	到期日开始时标的价格（仅适用于MOVE合约）
        /// </summary>
        public decimal strikePrice { get; set; }
        /// <summary>
        /// 21124.583	本期货持仓合约数量
        /// </summary>
        public decimal openInterest { get; set; }
    }
}
