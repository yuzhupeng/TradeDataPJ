using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GetTradeHistoryData 
{
   public class huobiSpot
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long tradeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(DeciamlConverter))]
        public decimal price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string direction { get; set; }


        public DateTime actcualtime { get; set; }
    }
}

//数据更新字段列表
//字段  数据类型 描述
//id integer 唯一成交ID（将被废弃）
//tradeId integer 唯一成交ID（NEW）
//amount float 成交量（买或卖一方）
//price float 成交价
//ts integer 成交时间(UNIX epoch time in millisecond)
//direction string 成交主动方(taker的订单方向) : 'buy' or 'sell'
