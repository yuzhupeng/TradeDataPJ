using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class instruments
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
        public string uly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string baseCcy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string quoteCcy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string settleCcy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ctVal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ctMult { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ctValCcy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string optType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string stk { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string listTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lever { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tickSz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lotSz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string minSz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ctType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string state { get; set; }
    }

}

//HTTP请求
//GET /api/v5/public/ instruments

//请求示例

//GET /api/v5/public/ instruments ? instType = SPOT
//请求参数
//参数名 类型	是否必须	描述
//instType	String	是	产品类型
//SPOT：币币
//SWAP：永续合约
//FUTURES：交割合约
//OPTION：期权
//uly	String	可选	合约标的指数，仅适用于交割/永续/期权，期权必填
//instId	String	否	产品ID







//nstType String	产品类型
//instId	String	产品id， 如 BTC-USD-SWAP
//uly	String	合约标的指数，如 BTC-USD ，仅适用于交割/永续/期权
//category	String	手续费档位，每个交易产品属于哪个档位手续费
//baseCcy	String	交易货币币种，如 BTC-USDT 中的 BTC ，仅适用于币币
//quoteCcy	String	计价货币币种，如 BTC-USDT 中的USDT ，仅适用于币币
//settleCcy	String	盈亏结算和保证金币种，如 BTC 仅适用于交割/永续/期权
//ctVal	String	合约面值 ，仅适用于交割/永续/期权
//ctMult	String	合约乘数 ，仅适用于交割/永续/期权
//ctValCcy	String	合约面值计价币种，仅适用于交割/永续/期权
//optType	String	期权类型，C或P 仅适用于期权
//stk	String	行权价格 ，仅适用于期权
//listTime	String	上线日期 ，仅适用于交割 和 期权
//Unix时间戳的毫秒数格式，如 1597026383085
//expTime	String	交割日期 仅适用于交割 和 期权
//Unix时间戳的毫秒数格式，如 1597026383085
//lever	String	杠杆倍数 ， 不适用于币币，用于区分币币和币币杠杆
//tickSz	String	下单价格精度， 如 0.0001
//lotSz	String	下单数量精度， 如 BTC-USDT-SWAP：1
//minSz	String	最小下单数量
//ctType	String	linear：正向合约
//inverse：反向合约
//仅交割/永续有效
//alias	String	合约日期别名
//this_week：本周
//next_week：次周
//quarter：季度
//next_quarter：次季度
//仅适用于交割
//state	String	产品状态
//live：交易中
//suspend：暂停中
//preopen：预上线