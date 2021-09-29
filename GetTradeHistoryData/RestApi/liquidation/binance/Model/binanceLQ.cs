using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class binanceLQ
    {
    

        // 交易对
        public string s { get; set; }
        // // 标的交易对 BTCUSD
        public string ps { get; set; }
        // 订单方向
        public string S { get; set; }
        //   // 订单类型
        public string o { get; set; }
        // 有效方式
        public string f { get; set; }
        // 订单数量
        public string q { get; set; }
        // 订单价格
        public string p { get; set; }
        // 平均价格
        public string ap { get; set; }
        // 订单状态
        public string X { get; set; }
        // 订单最近成交量
        public string l { get; set; }
        // 订单累计成交量
        public string z { get; set; }
        // 交易时间
        public string T { get; set; }
        public DateTime actcualtime { get; set; }
    }
}

//{

//    "e":"forceOrder",                    // 事件类型
//    "E": 1591154240950,                 // 事件时间
//    "o":{

//        "s":"BTCUSD_200925",           // 交易对
//        "ps": "BTCUSD",                 // 标的交易对
//        "S":"SELL",                     // 订单方向
//        "o":"LIMIT",                    // 订单类型
//        "f":"IOC",                      // 有效方式
//        "q":"1",                        // 订单数量
//        "p":"9425.5",                   // 订单价格
//        "ap":"9496.5",                  // 平均价格
//        "X":"FILLED",                   // 订单状态
//        "l":"1",                        // 订单最近成交量
//        "z":"1",                        // 订单累计成交量
//        "T": 1591154240949,             // 交易时间

//    }
//}
