using DapperExtensions.Mapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration 
{
    [BsonIgnoreExtraElements]
    public class PermanentFuture: BaseEntity
    {
        public PermanentFuture()
        {
            SYS_CreateDate = System.DateTime.Now;
        }

 

        public long DID { get; set; }

        public string uuuid { get; set; }

        public DateTime  Times { get; set; }

        public string timestamp { get; set; }

        public string utime { get; set; }

        public string exchange { get; set; }

        public string ContractType { get; set; }

        public string pair { get; set; }

        public string market { get; set; }

        public string types { get; set; }

        public decimal  price { get; set; }

        public string Unit { get; set; }

        public long  count { get; set; }

        public decimal  count_buy { get; set; }

        public decimal  count_sell { get; set; }

        public decimal  vol { get; set; }

        public decimal  vol_buy { get; set; }

        public decimal  vol_sell { get; set; }

        public decimal qty { get; set; }

        public decimal qty_buy { get; set; }

        public decimal qty_sell { get; set; }


        public decimal  close { get; set; }

        public decimal  low { get; set; }

        public decimal  open { get; set; }

        public decimal  high { get; set; }

        public decimal  fundingrate { get; set; }

        public decimal  estimated_rate { get; set; }

        public decimal  liquidation_buy { get; set; }

        public decimal  liquidation_sell { get; set; }

        public decimal  liquidation { get; set; }

        public string basis { get; set; }

        public DateTime  SYS_CreateDate { get; set; }

        public string timecode { get; set; }

        /// <summary>
        /// 币
        /// </summary>
        public string contractcode { get; set; }


    }
 
}
