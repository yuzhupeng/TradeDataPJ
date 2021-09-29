using Newtonsoft.Json;
using System;

namespace GetTradeHistoryData
{
    /// <summary>
    /// Open Interest History info
    /// </summary>
    public class BinanceFuturesOpenInterestHistory
    {
        /// <summary>
        /// The symbol the information is about
        /// </summary>
        public string Symbol { get; set; } = "";

        /// <summary>
        /// Total open interest
        /// </summary>
        public decimal SumOpenInterest { get; set; }

        /// <summary>
        /// Total open interest value
        /// </summary>
        public decimal SumOpenInterestValue { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(BJTimestampConverter))]
        public DateTime? Timestamp { get; set; }


        /// <summary>
        /// The symbol the information is about
        /// </summary>
        public string pair { get; set; } = "";
    }
}
