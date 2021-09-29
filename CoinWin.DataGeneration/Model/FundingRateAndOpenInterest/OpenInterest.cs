using System;
using System.Collections.Generic;
using System.Text;

namespace CoinWin.DataGeneration
{
  public  class OpenInterest 
    {
        /// <summary>
        /// 交易对
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 市场
        /// </summary>
        public string market { get; set; }
        /// <summary>
        /// 交易所
        /// </summary>
        public string exchange { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string times { get; set; }
        /// <summary>
        /// USDT数量
        /// </summary>
        public decimal SumOpenInterest { get; set; }

        /// <summary>
        /// 价值
        /// </summary>
        public decimal SumOpenInterestValue { get; set; }

        /// <summary>
        /// 张数
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 持仓币数量
        /// </summary>
        public decimal coin { get; set; }

        /// <summary>
        /// 持仓类型（币/张/USDT）coin amount USDT
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 持仓类型说明（币/张/USDT）
        /// </summary>
        public string desc { get; set; }


        public string kind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal volumeUsd24h { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { get; set; }

    }

    class OpenInterestComparer : IEqualityComparer<OpenInterest>
    {
        public bool Equals(OpenInterest x, OpenInterest y)
        {
            if ((x.market == y.market) && (x.exchange == y.exchange))
            {
                //Console.WriteLine("比较相等....:"+x.ToJson().ToString());
                return true;
            }
            return false;


            //if (p1 == null)
            //    return p2 == null;
            //return p1.price == p2.price;
        }

        public int GetHashCode(OpenInterest obj)
        {
            return obj.market.GetHashCode() ^ obj.exchange.GetHashCode() ;
        }
    }
}
