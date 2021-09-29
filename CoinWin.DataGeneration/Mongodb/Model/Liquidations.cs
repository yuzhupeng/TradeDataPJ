using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinWin.DataGeneration
{
    //biglqmax
    [BsonIgnoreExtraElements]
    public class Liquidations : BaseEntity
    {
        public Liquidations()
        {
            uuuid = QPP.Core.GuidHelper.NewSID12();

            SYS_CreateDate = System.DateTime.Now.ToString(); ;
        }

        /// <summary>
        /// 
        /// </summary>
        public string times { get; set; }

        /// <summary>
        /// --总价值数量
        /// </summary>
        public decimal vol { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal price { get; set; }


        /// <summary>
        /// 方向
        /// </summary>
        public string side { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string SYS_CreateDate { get; set; }

        /// <summary>
        /// 货数量
        /// </summary>
        public decimal qty { get; set; }

        /// <summary>
        /// 张数
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// ETH/ETC/LTC
        /// </summary>
        public string pair { get; set; }


        public string uuuid { get; set; }

        /// <summary>
        /// 撮合id
        /// </summary>
        public string cross_seq { get; set; }

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
        /// market---大饼-0326/u
        /// </summary>
        public string market { get; set; }



        /// <summary>
        /// types---大饼/u 说明
        /// </summary>
        public string types { get; set; }

        /// <summary>
        /// 交割,现货,永续,期权
        /// </summary>
        public string kinds { get; set; }


        ///// <summary>
        ///// --总价值数量
        ///// </summary>
        //public string vol { get; set; }
        ///// <summary>
        ///// 单价
        ///// </summary>
        //public string price { get; set; }


        ///// <summary>
        ///// 方向
        ///// </summary>
        //public string side { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }


        /// <summary>
        /// 币
        /// </summary>
        public string contractcode { get; set; }

    }
}
