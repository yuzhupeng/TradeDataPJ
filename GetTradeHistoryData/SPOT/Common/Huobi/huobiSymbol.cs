using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class huobiSymbol
    {
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
        /// <summary>
        /// 
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bidSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal askSize { get; set; }
    }

}

//字段名称 数据类型    描述
//amount  float 以基础币种计量的交易量（以滚动24小时计）
//count integer 交易笔数（以滚动24小时计）
//open float 开盘价（以新加坡时间自然日计）
//close float 最新价（以新加坡时间自然日计）
//low float 最低价（以新加坡时间自然日计）
//high float 最高价（以新加坡时间自然日计）
//vol float 以报价币种计量的交易量（以滚动24小时计）
//symbol string 交易对，例如btcusdt, ethbtc
//bid float 买一价
//bidSize float 买一量
//ask float 卖一价
//askSize float 卖一量

//获得所有交易对的 tickers。 shell curl "https://api.huobi.pro/market/tickers"