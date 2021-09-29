using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
 
    public class MarkPrice
    {
        /// <summary>
        /// 
        /// </summary>
        public string instType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string instId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal markPx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ts { get; set; }
    }

}


//{
//    "code":"0",
//    "msg":"",
//    "data":[
//    {
//        "instType":"SWAP",
//        "instId":"BTC-USDT-SWAP",
//        "markPx":"200",
//        "ts":"1597026383085"
//    }
//  ]
//}
//返回参数
//参数名	类型	描述
//instType	String	产品类型
//MARGIN：币币杠杆
//SWAP：永续合约
//FUTURES：交割合约
//OPTION：期权
//instId	String	产品ID，如 BTC-USD-200214
//markPx	String	标记价格
//ts	String	接口数据返回时间，Unix时间戳的毫秒数格式，如1597026383085



