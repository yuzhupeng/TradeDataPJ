using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class okexticket
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
        public decimal last { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal lastSz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal askPx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal askSz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bidPx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bidSz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal open24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal high24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal low24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal volCcy24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal vol24h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal sodUtc0 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal sodUtc8 { get; set; }
    }

}



//instType String	产品类型
//instId	String	产品ID
//last	String	最新成交价
//lastSz	String	最新成交的数量
//askPx	String	卖一价
//askSz	String	卖一价的挂单数数量
//bidPx	String	买一价
//bidSz	String	买一价的挂单数量
//open24h	String	24小时开盘价
//high24h	String	24小时最高价
//low24h	String	24小时最低价
//volCcy24h	String	24小时成交量，以币为单位
//如果是衍生品合约，数值为结算货币的数量。
//如果是币币/币币杠杆，数值为计价货币的数量。
//vol24h	String	24小时成交量，以张为单位
//如果是衍生品合约，数值为合约的张数。
//如果是币币/币币杠杆，数值为交易货币的数量。
//sodUtc0	String	UTC 0 时开盘价
//sodUtc8	String	UTC+8 时开盘价
//ts	String	ticker数据产生时间，Unix时间戳的毫秒数格式，如 1597026383085