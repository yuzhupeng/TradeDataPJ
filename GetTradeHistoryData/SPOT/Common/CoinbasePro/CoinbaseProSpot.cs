using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
   
    public class CoinbaseProSpot
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trade_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string maker_order_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string taker_order_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string product_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long sequence { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }


        public DateTime actcualtime { get; set; }
    }

}
