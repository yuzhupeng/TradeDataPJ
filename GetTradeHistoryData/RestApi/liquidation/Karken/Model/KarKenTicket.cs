using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GetTradeHistoryData 
{
     
    public class KarKenTicket
    {
        /// <summary>
        /// 
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pair { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Decimal markPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long bid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long bidSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Decimal ask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long askSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal vol24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal openInterest { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long open24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long last { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lastTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long lastSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string suspended { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(DeciamlConverter))]
        public Decimal fundingRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(DeciamlConverter))]
        public Decimal fundingRatePrediction { get; set; }
    }

}
//tag: "month",
//pair: "XRP:USD",
//symbol: "fi_xrpusd_210430",
//markPrice: 1.8006,
//bid: 1.8049,
//bidSize: 504,
//ask: 1.8079,
//askSize: 454,
//vol24h: 2590270,
//openInterest: 741742,
//open24h: 2.0011,
//last: 1.8054,
//lastTime: "2021-04-15T06:09:36.835Z",
//lastSize: 14720,
//suspended: false

