using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
   public class BybitFuturesOpenInterestHistory
    {    /// <summary>
         /// The symbol the information is about
         /// </summary>
        public string Symbol { get; set; } = "";

        /// <summary>
        /// Total open interest
        /// </summary>
        public decimal open_interest { get; set; }
 
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(TimestampConverter))]
        public DateTime? timestamp { get; set; }
    }
}
