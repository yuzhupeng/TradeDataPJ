using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
    /// <summary>
    /// U本位的公共类
    /// </summary>
    public class UPermanentFutures
    {
        public UPermanentFutures()
        {
            uuuid= QPP.Core.GuidHelper.NewSID12();
            types = "USDT";
            SYS_CreateDate = System.DateTime.Now.ToString(); ;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string SYS_CreateDate { get; set; }

        /// <summary>
        /// 货数量
        /// </summary>
        public string qty { get; set; }


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
        public string vol { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public string price { get; set; }
 

        /// <summary>
        /// 方向
        /// </summary>
        public string side { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

       /// <summary>
       /// 币
       /// </summary>
        public string contractcode { get; set; }

        /// <summary>
        /// 类型 SPOT PERP DELIVER
        /// </summary>
        public string kind { get; set; }
    }
}
