using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{

    /// <summary>
    /// 通用数据活动记录Model
    /// 创建人：FISHYUES
    /// 创建时间：2020年12月21日13:58:56
    /// </summary>
    public class ExchageDataMin
    {
        public ExchageDataMin()
        {
            this.ID = 0;
            this.DID= QPP.Core.GuidHelper.NewSID12();
            this.SYS_CreateDate = DateTime.Now; 
            this.SYS_Status = "@CLOSED";
            this.SYS_Createby = "SYSTEM";
        }




        public int ID { get; set; }
        /// <summary>
        ///  ID
        /// </summary>
        public string DID { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Times { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string utime { get; set; }

        /// <summary>
        /// 收盘
        /// </summary>
        public decimal close { get; set; }
        /// <summary>
        /// DJ数量
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 买盘数量
        /// </summary>
        public int count_buy { get; set; }
        /// <summary>
        /// 卖盘数量
        /// </summary>
        public int count_sell { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public string exchange { get; set; }
        /// <summary>
        /// 最高
        /// </summary>
        public decimal high { get; set; }
        /// <summary>
        /// 爆仓-多单
        /// </summary>
        public decimal liquidation_buy { get; set; }
        /// <summary>
        /// 爆仓-空单
        /// </summary>
        public decimal liquidation_sell { get; set; }
        /// <summary>
        /// 爆仓数量
        /// </summary>
        public decimal liquidation { get; set; }

        /// <summary>
        /// 最低
        /// </summary>
        public decimal low { get; set; }
        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal open { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string pair { get; set; }
        /// <summary>
        /// 交易量
        /// </summary>
        public decimal? vol { get; set; }
        /// <summary>
        /// 成交量-买盘
        /// </summary>
        public decimal? vol_buy { get; set; }
        /// <summary>
        /// 成交量-卖盘
        /// </summary>
        public decimal? vol_sell { get; set; }
        /// <summary>
        /// 单位 -Dollar
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string SYS_Createby { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime SYS_CreateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string SYS_Status { get; set; }

    }



    public class ExchageDataMinMapper : ClassMapper<ExchageDataMin>
    {
        public ExchageDataMinMapper()
        {
            Table("coin_exchagedatabymin_hour");
            Map(m => m.ID)
              .Key(KeyType.Identity);// 主键的类型		
            //Map(m => m.hourlist).Ignore();
            AutoMap();
        }
    }
}
