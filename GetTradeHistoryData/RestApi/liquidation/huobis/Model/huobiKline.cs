using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class huobiKline
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal close { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal high { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal low { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal open { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal vol { get; set; }

        public string symbol { get; set; }
    }

}
