using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    /// <summary>
    /// 公共类
    /// </summary>
    public class UPermanentFuturesModel
    {
        public UPermanentFuturesModel()
        {
          
            SYS_CreateDate = System.DateTime.Now.ToString();
        }

        public long did { get; set; }

        public string uuuid { get; set; }

 
        /// <summary>
        /// 
        /// </summary>
        public string times { get; set; }

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
        public decimal price { get; set; }

        /// <summary>
        /// 货数量
        /// </summary>
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



    }

    public class UPermanentFuturesModelMapper : ClassMapper<UPermanentFuturesModel>
    {
        public UPermanentFuturesModelMapper()
        {
            Table("UPermanentFutures");
            Map(m => m.did)
              .Key(KeyType.Identity);// 主键的类型		
            //Map(m => m.hourlist).Ignore();
            AutoMap();
        }
    }
}
