using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class BinanceSpot
    {
        // 事件类型
        public string e { get; set; }


        // 事件时间
        public long E { get; set; }

        // 交易对
        public string s { get; set; }


        // 交易ID
        public string t { get; set; }

        // 成交价格
        public string p { get; set; }


        // 成交笔数
        public string q { get; set; }


        // 买方的订单ID
        public string b { get; set; }

        // 卖方的订单ID
        public string a { get; set; }

       
        // 成交时间
        public string T { get; set; }
        // 买方是否是做市方
        public string m { get; set; }

        public string M { get; set; }
        public DateTime actcualtime { get; set; }
    }
}
//{
//  "e": "trade",     // 事件类型
//  "E": 123456789,   // 事件时间
//  "s": "BNBBTC",    // 交易对
//  "t": 12345,       // 交易ID
//  "p": "0.001",     // 成交价格
//  "q": "100",       // 成交笔数
//  "b": 88,          // 买方的订单ID
//  "a": 50,          // 卖方的订单ID
//  "T": 123456785,   // 成交时间
//  "m": true,        // 买方是否是做市方。如true，则此次成交是一个主动卖出单，否则是一个主动买入单。
//  "M": true         // 请忽略该字段
//}