using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
    public class BybitLQData
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal price { get; set; }
    }

    public class BybitLQ
    {
        /// <summary>
        /// 
        /// </summary>
        public int ret_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ret_msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ext_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ext_info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<BybitLQData> result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal time_now { get; set; }
    }

}
