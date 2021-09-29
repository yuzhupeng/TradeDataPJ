using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
   public class BybitSymbol
    {
        public string name { get; set; }

        public string base_currency { get; set; }

        public string quote_currency { get; set; }

        public decimal price_scale { get; set; }

        public string taker_fee { get; set; }

        public string maker_fee { get; set; }
    }
}

//name string 合约名称
//base_currency	string	基础货币
//quote_currency	string	报价货币
//price_scale	number	价格范围
//taker_fee	string	taker 手续费
//maker_fee	string	maker 手续费
//leverage_filter > min_leverage	number	最小杠杆数
//leverage_filter > max_leverage	number	最大杠杆数
//leverage_filter > leverage_step	string	杠杆最小增加或减少数量
//price_filter > min_price	string	最小价格
//price_filter > max_price	string	最大价格
//price_filter > tick_size	string	价格最小增加或减少数量
//lot_size_filter > max_trading_qty	number	最大交易数量
//lot_size_filter > min_trading_qty	number	最小交易数量
//lot_size_filter > qty_step	number	合约数量最小单位