using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class Okexopeninterest
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
            public decimal oi { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public decimal oiCcy { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string ts { get; set; }
        }
 
}

//instType String	产品类型
//insId	String	产品ID
//oi	String	持仓量（按张折算）
//oiCcy	String	持仓量（按币折算）
//ts	String	数据返回时间， Unix时间戳的毫秒数格式 ，如 1597026383085