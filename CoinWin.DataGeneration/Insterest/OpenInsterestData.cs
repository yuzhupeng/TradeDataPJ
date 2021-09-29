using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
   public class OpenInsterestData
    {
        public string uid { get; set; }

        //类型 - 交割  永续
        public string kind { get; set; }

        public DateTime Times { get; set; }
        public DateTime STimes { get; set; }

        public string exchange { get; set; }

        public string market { get; set; }

        public string symbol { get; set; }

        //
        public decimal priceST { get; set; }

        public decimal priceEN { get; set; }

        public decimal priceMax { get; set; }

        public decimal priceMin { get; set; }


        public string pricePrecent { get; set; }
 

        /// <summary>
        /// 币 
        /// </summary>
        public string contractcode { get; set; }


        /// <summary>
        /// 持仓开始价值
        /// </summary>
        public decimal SumOpenInterestValueST { get; set; }

        /// <summary>
        /// 持仓当前/结束价值
        /// </summary>
        public decimal SumOpenInterestValueEn { get; set; }

        public decimal SumOpenInterestValueMax { get; set; }
  
        public decimal SumOpenInterestValueMin { get; set; }


        public string SumOpenInterestPrecent { get; set; }
         
        public string timecode { get; set; }



        /// <summary>
        /// 持仓币数量
        /// </summary>
        public decimal STcoin { get; set; }
        /// <summary>
        /// 持仓币数量
        /// </summary>
        public decimal ENcoin { get; set; }
        public string CoinPrecent { get; set; }

    }
}
