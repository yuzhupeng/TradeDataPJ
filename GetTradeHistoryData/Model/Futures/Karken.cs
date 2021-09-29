using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
    public class Karken
    {
        /// <summary>
        /// 
        /// </summary>
        public string feed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string product_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long seq { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal price { get; set; }




        public DateTime actcualtime { get; set; }
    }

}
