using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class Deribit
    {
        /// <summary>
        /// 
        /// </summary>
        public long trade_seq { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string trade_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long timestamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long tick_direction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal mark_price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string instrument_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal index_price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string direction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal amount { get; set; }


        public DateTime actcualtime { get; set; }
    }

 

}
