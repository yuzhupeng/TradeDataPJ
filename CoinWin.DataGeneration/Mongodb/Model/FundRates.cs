using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinWin.DataGeneration
{
    [BsonIgnoreExtraElements]
    public  class FundRates: BaseEntity
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
        /// 当前费率
        /// </summary>
        public string FundingRate { get; set; }

        /// <summary>
        /// 下一期费率
        /// </summary>
        public string NextFundingRate { get; set; }


        /// <summary>
        /// 下一期费率
        /// </summary>
        public string NextFundingRateTime { get; set; }


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

 


    }


    //class FundRateComparer : IEqualityComparer<FundRate>
    //{
    //    public bool Equals(FundRate x, FundRate y)
    //    {
    //        if ((x.market == y.market) && (x.exchange == y.exchange))
    //        {
    //            //Console.WriteLine("比较相等....:"+x.ToJson().ToString());
    //            return true;
    //        }
    //        return false;


    //        //if (p1 == null)
    //        //    return p2 == null;
    //        //return p1.price == p2.price;
    //    }

    //    public int GetHashCode(FundRate obj)
    //    {
    //        return obj.market.GetHashCode() ^ obj.exchange.GetHashCode();
    //    }
    //}
}
