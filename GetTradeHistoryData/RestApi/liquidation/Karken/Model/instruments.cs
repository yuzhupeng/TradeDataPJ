using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class Karkeninstruments
    {
        /// <summary>
        /// 
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string underlying { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double tickSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int contractSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tradeable { get; set; }
    }

}

