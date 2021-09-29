using DapperExtensions.Mapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// 爆仓数据处理
    /// </summary>
    public class LiquidationModel
    {
        public LiquidationModel()
        {
            this.amount = "";
            //SYS_CreateDate = System.DateTime.Now.ToString();
        }

        public long did { get; set; }

        public string uuuid { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime times { get; set; }

        /// <summary>
        /// UTC时间
        /// </summary>
        public string utctime { get; set; }


        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public string exchange { get; set; }

        /// <summary>
        /// ETH/ETC/LTC
        /// </summary>
        public string pair { get; set; }


        /// <summary>
        /// market---大饼-0326/u
        /// </summary>
        public string market { get; set; }



        /// <summary>
        /// types---大饼/u
        /// </summary>
        public string types { get; set; }

        /// <summary>
        /// --总价值数量
        /// </summary>
        public decimal vol { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [JsonConverter(typeof(DeciamlConverter))]
        public decimal price { get; set; }

        /// <summary>
        /// 货数量
        /// </summary>
        [JsonConverter(typeof(DeciamlConverter))]
        public decimal qty { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public string side { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string SYS_CreateDate { get; set; }


        /// <summary>
        ///  张数量
        /// </summary>
 
        public string amount { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }



        /// <summary>
        /// 币
        /// </summary>
        public string contractcode { get; set; }



        /// <summary>
        /// 交割,现货,永续,期权
        /// </summary>
        public string kinds { get; set; }

    }

    public class LiquidationModellMapper : ClassMapper<LiquidationModel>
    {
        public LiquidationModellMapper()
        {
            Table("Liquidation");
            Map(m => m.did)
              .Key(KeyType.Identity);// 主键的类型		
            Map(m => m.contractcode).Ignore();
            Map(m => m.kinds).Ignore();
            AutoMap();
        }
    }


    class ProductNoComparer : IEqualityComparer<LiquidationModel>
    {
        public bool Equals(LiquidationModel x, LiquidationModel y)
        {
            if ((x.price == y.price) && (x.times == y.times) && (x.vol == y.vol))
            {
                //Console.WriteLine("比较相等....:"+x.ToJson().ToString());
                return true;
            }
            return false;
 

            //if (p1 == null)
            //    return p2 == null;
            //return p1.price == p2.price;
        }

        public int GetHashCode(LiquidationModel obj)
        {
            return obj.price.GetHashCode() ^ obj.times.GetHashCode() ^ obj.vol.GetHashCode();
        }
    }
}
