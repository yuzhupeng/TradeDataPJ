using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class TFhrTicket
    {
        /// <summary>
        /// 
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pair { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal priceChange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal priceChangePercent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal weightedAvgPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal lastPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal lastQty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal openPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal highPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal lowPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal volume { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 

        public decimal quoteVolume { get; set; }

        public decimal baseVolume { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long openTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long closeTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long firstId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long lastId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long count { get; set; }
    }

}


//u本位
//{
//    "symbol": "BTCUSDT",
//  "priceChange": "-94.99999800",    //24小时价格变动
//  "priceChangePercent": "-95.960",  //24小时价格变动百分比
//  "weightedAvgPrice": "0.29628482", //加权平均价
//  "lastPrice": "4.00000200",        //最近一次成交价
//  "lastQty": "200.00000000",        //最近一次成交额
//  "openPrice": "99.00000000",       //24小时内第一次成交的价格
//  "highPrice": "100.00000000",      //24小时最高价
//  "lowPrice": "0.10000000",         //24小时最低价
//  "volume": "8913.30000000",        //24小时成交量
//  "quoteVolume": "15.30000000",     //24小时成交额
//  "openTime": 1499783499040,        //24小时内，第一笔交易的发生时间
//  "closeTime": 1499869899040,       //24小时内，最后一笔交易的发生时间
//  "firstId": 28385,   // 首笔成交id
//  "lastId": 28460,    // 末笔成交id
//  "count": 76         // 成交笔数
//}


//币本位
//    [
//    {
//    "symbol": "BTCUSD_200925",
//        "pair": "BTCUSD",
//        "priceChange": "136.6",             //24小时价格变动
//        "priceChangePercent": "1.436",  //24小时价格变动百分比
//        "weightedAvgPrice": "9547.3",       //24小时加权平均价
//        "lastPrice": "9651.6",              //最近一次成交价
//        "lastQty": "1",                     //最近一次成交量
//        "openPrice": "9515.0",              //24小时内第一次成交的价格
//        "highPrice": "9687.0",              //24小时最高价
//        "lowPrice": "9499.5",               //24小时最低价
//        "volume": "494109",                 //24小时成交量
//        "baseVolume": "5192.94797687",  //24小时成交额(标的数量)
//        "openTime": 1591170300000,          //24小时内,第一笔交易的发生时间
//        "closeTime": 1591256718418,     //24小时内,最后一笔交易的发生时间
//        "firstId": 600507,                  // 首笔成交id
//        "lastId": 697803,                   // 末笔成交id
//        "count": 97297                      // 成交笔数     
//    }
//]