using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class okexFundingRate
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
        public decimal fundingRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal nextFundingRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fundingTime { get; set; }
    }

   
}



//获取永续合约当前资金费率
//获取当前资金费率

//限速： 20次/2s
//HTTP请求
//GET /api/v5/public/ funding - rate

//请求示例

//GET /api/v5/public/ funding - rate ? instId = BTC - USD - SWAP
//请求参数
//参数名 类型	是否必须	描述
//instId	String	是	产品ID ，如 BTC-USD-SWAP
//仅适用于永续
//返回结果

//{
//    "code":"0",
//    "msg":"",
//    "data":[
//     {
//        "instType":"SWAP",
//        "instId":"BTC-USDT-SWAP",
//        "fundingRate":"0.018",
//        "nextFundingRate":"0.017",
//        "fundingTime":"1597026383085"
//    }
//  ]
//}
//返回参数
//参数名	类型	描述
//instType	String	产品类型 SWAP：永续合约
//instId	String	产品ID，如BTC-USD-SWAP
//fundingRate	String	资金费率
//nextFundingRate	String	下一期预测资金费率
//fundingTime	String	资金费时间 ，Unix时间戳的毫秒数格式，如 1597026383085
