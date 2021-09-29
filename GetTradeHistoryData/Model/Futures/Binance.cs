using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
   public class Binance
    {
        // 事件类型
        public string e { get; set; }


        // 事件时间时间戳
        public long E { get; set; }

        // 交易对
        public string s { get; set; }


        // 成交价格
        public string a { get; set; }


        // 成交笔数
        public string p { get; set; }


        // 被归集的首个交易ID
        public string q { get; set; }

        // 被归集的末次交易ID
        public string f { get; set; }

        // 被归集的末次交易ID
        public string l { get; set; }

        // 成交时间
        public string T { get; set; }
        // 买方是否是做市方
        public bool m { get; set; }
        //"e": "aggTrade",  // 事件类型
        //"E": 123456789,   // 事件时间
        //"s": "BNBUSDT",    // 交易对
        //"a": "",
        //"p": "0.001",     // 成交价格
        //"q": "100",       // 成交笔数
        //"f": 100,         // 被归集的首个交易ID
        //"l": 105,         // 被归集的末次交易ID
        //"T": 123456785,   // 成交时间
        //"m": true         // 买方是否是做市方。如true，则此次成交是一个主动卖出单，否则是一个主动买入单。

        public DateTime actcualtime { get; set; }
    }
}
//{
//    "e": "aggTrade",  // 事件类型
//  "E": 123456789,   // 事件时间
//  "s": "BNBUSDT",    // 交易对
//  "a": 5933014,     // 归集成交 ID
//  "p": "0.001",     // 成交价格
//  "q": "100",       // 成交量
//  "f": 100,         // 被归集的首个交易ID
//  "l": 105,         // 被归集的末次交易ID
//  "T": 123456785,   // 成交时间
//  "m": true         // 买方是否是做市方。如true，则此次成交是一个主动卖出单，否则是一个主动买入单。
//}