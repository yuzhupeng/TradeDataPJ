using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class OkexTicket
    {
        /// <summary>
        /// 
        /// </summary>
        public string best_ask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string best_bid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string instrument_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string open_utc0 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string open_utc8 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string product_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string last { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string last_qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string best_ask_size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string best_bid_size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string open_24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string high_24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string low_24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string base_volume_24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string quote_volume_24h { get; set; }
    }

}
