using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
    /// <summary>
    /// 合约信息和资金费率
    /// </summary>
  public  class BybitFuturetickers
    {
        public string symbol{get;set;}
        public string bid_price{get;set;} // Empty if none
        public string ask_price{get;set;} // Empty if none
        public decimal last_price {get;set;} // Empty if none
        public string last_tick_direction{get;set;}
        public string prev_price_24h{get;set;} // Empty if none
        public string price_24h_pcnt{get;set;} // Empty if none
        public string high_price_24h{get;set;} // Empty if none
        public string low_price_24h{get;set;} // Empty if none
        public string prev_price_1h{get;set;} // Empty if none
        public string price_1h_pcnt{get;set;} // Empty if none
        public string mark_price{get;set;} // Empty if none
        public string index_price{get;set;} // Empty if none
        public decimal open_interest{get;set;}
        public decimal open_value{get;set;}
        public decimal total_turnover{get;set;}
        public decimal turnover_24h{get;set;}
        public decimal total_volume{get;set;}
        public decimal volume_24h{get;set;}
        public decimal funding_rate{get;set;}
        public decimal predicted_funding_rate{get;set;}
        public string next_funding_time{get;set;}
        public int countdown_hour{get;set;}
    }
}
//返回参数

//参数	类型	说明
//symbol	string	合约类型
//bid_price	string	第一笔挂单买入价
//ask_price	string	第一笔挂单卖出价
//last_price	string	最新成交价
//last_tick_direction	string	价格波动方向枚举
//prev_price_24h	string	24小时前的整点市价
//price_24h_pcnt	string	市价相对24h变化百分比
//high_price_24h	string	最近 24 小时最高价
//low_price_24h	string	最近 24 小时最低价
//prev_price_1h	string	1小时前的整点市价
//price_1h_pcnt	string	市价相对1小时前变化百分比
//mark_price	string	标记价格
//index_price	string	指数价格
//open_interest	number	未平仓合约数量
//open_value	string	未平仓价值
//total_turnover	string	总营业额
//turnover_24h	string	24小时营业额
//total_volume	number	总交易量
//volume_24h	number	最近 24 小时成交量
//funding_rate	string	资金费率
//predicted_funding_rate	string	预测资金费率
//next_funding_time	string	下次结算资金费用时间
//countdown_hour	number	结算资金费用剩余时间