using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    /// <summary>
    /// 
    /// </summary>
    public class huoticket
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string symbol { get; set; }

        public string contract_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal open { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal close { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal low { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal high { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal vol { get; set; }
    }

}



//参数名称 是否必须	数据类型	描述	取值范围
//status	true	string	请求处理结果	"ok" , "error"
//<ticks>	true	object array		
//contract_code	true	string	合约代码	"BTC-USDT" ...
//id	true	long	K线id	
//amount	true	string	成交量(币)	
//ask true    array[卖1价, 卖1量(张)]
//bid true    array[买1价, 买1量(张)]
//open true    string 开盘价	
//close	true	string	收盘价, 当K线为最晚的一根时，是最新成交价	
//count	true	decimal	成交笔数	
//high	true	string	最高价	
//low	true	string	最低价	
//vol	true	string	成交量（张），买卖双边成交量之和	
//trade_turnover	true	string	成交额	
//ts	true	long	时间戳	
//</ticks>				
//ts	true	long	响应生成时间点，单位：毫秒