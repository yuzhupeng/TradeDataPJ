using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class huobiticket
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "base-currency")]
        public string base_currency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "quote-currency")]
        public string quote_currency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "price-precision")]
        public int price_precision { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "amount-precision")]
        public int amount_precision { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "symbol-partition")]
        public string symbol_partition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string state { get; set; }
        /// <summary>




    }
}
