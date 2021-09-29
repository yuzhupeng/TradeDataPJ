using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    /// <summary>
    /// bitmex model
    /// </summary>
   public class BitmexModel
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public string size { get; set; }
        /// <summary>
        /// 价值
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tickDirection { get; set; }
        /// <summary>
        /// 唯一id
        /// </summary>
        public string trdMatchID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string grossValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string homeNotional { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string foreignNotional { get; set; }


        public string actcualtime { get; set; }
    }


}
