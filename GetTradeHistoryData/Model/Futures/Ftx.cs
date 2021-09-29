using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GetTradeHistoryData
{
    public class Ftx
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        [JsonConverter(typeof(DeciamlConverter))]
        public decimal price { get; set; }
        /// <summary>
        /// 
        [JsonConverter(typeof(DeciamlConverter))]
        public decimal size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liquidation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }

        public string actcualtime { get; set; }
    }



}
