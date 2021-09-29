using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration 
{
    public class TradeMater
    {
        public TradeMater()
        {
            SYS_CreateDate = System.DateTime.Now;
        }

        public long DID { get; set; }

        public string uuuid { get; set; }

        public DateTime Times { get; set; }

        public string timestamp { get; set; }

        public string utime { get; set; }

        public string exchange { get; set; }

        public string ContractType { get; set; }

        public string pair { get; set; }

        public string market { get; set; }

        public string types { get; set; }

        public decimal price { get; set; }

        public string Unit { get; set; }

        public long count { get; set; }

        public decimal count_buy { get; set; }

        public decimal count_sell { get; set; }

        public decimal vol { get; set; }

        public decimal vol_buy { get; set; }

        public decimal vol_sell { get; set; }

        public decimal vol_auctual { get; set; }

        public decimal qty { get; set; }

        public decimal qty_buy { get; set; }

        public decimal qty_sell { get; set; }

        public decimal close { get; set; }

        public decimal low { get; set; }

        public decimal open { get; set; }

        public decimal high { get; set; }

        public decimal fundingrate { get; set; }

        public decimal estimated_rate { get; set; }

        public decimal liquidation_buy { get; set; }

        public decimal liquidation_sell { get; set; }

        public decimal liquidation { get; set; }

        public string basis { get; set; }

        public DateTime SYS_CreateDate { get; set; }

        public string timecode { get; set; }

        /// <summary>
        /// 币
        /// </summary>
        public string contractcode { get; set; }

        /// <summary>
        /// 类型 SPOT PERP DELIVER
        /// </summary>
        public string kind { get; set; }

        /// <summary>
        /// 持仓开始价值
        /// </summary>
        public decimal SumOpenInterestValueStart { get; set; }

        /// <summary>
        /// 持仓当前/结束价值
        /// </summary>
        public decimal SumOpenInterestValueNow { get; set; }

        /// <summary>
        /// 费率开始
        /// </summary>
        public string FundingRateStart { get; set; }

        /// <summary>
        /// 费率结束
        /// </summary>
        public string FundingRateNow { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 持仓币数量
        /// </summary>
        public decimal startcoin { get; set; }


        /// <summary>
        /// 持仓币数量
        /// </summary>
        public decimal nowcoin { get; set; }

        /// <summary>
        /// 24小时成交
        /// </summary>
        public decimal volumeUsd24h { get; set; }



        public decimal liquidation_TFHbuy { get; set; }

        public decimal liquidation_TFHsell { get; set; }

        public decimal liquidationTFH { get; set; }


    }
}
