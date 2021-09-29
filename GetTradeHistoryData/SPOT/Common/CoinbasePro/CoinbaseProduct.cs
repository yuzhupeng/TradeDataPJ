using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
 
    public class CoinbaseProduct
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
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
        public string base_min_size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string base_max_size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string quote_increment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string base_increment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string display_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string min_market_funds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string max_market_funds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string margin_enabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string post_only { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string limit_only { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cancel_only { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string trading_disabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status_message { get; set; }
    }

}
