using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class details
    {
        /// <summary>
        /// 
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string posSide { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bkPx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal sz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bkLoss { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ccy { get; set; }

   
        public long ts { get; set; }


        /// <summary>
        /// 
        /// </summary>
        //[JsonProperty("ts"), JsonConverter(typeof(BJTimestampConverter))]
        //public DateTime times { get; set; }
    }

    public class OkexLq
    {
        /// <summary>
        /// 
        /// </summary>
        public string instType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string totalLoss { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string instId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<details> details { get; set; }
    }


   
}


//instType String	产品类型
//totalLoss	String	当前underlying下，当期内的总穿仓亏损
//以USDT为单位，每天16:00（UTC + 8）清零
//  instId	String	产品ID，如 BTC-USD-SWAP
//uly	String	合约标的指数，仅适用于交割/永续/期权
//details	Array	详细内容
//>side	String	订单方向 buy：买 sell：卖
//>posSide	String	持仓方向，long：多 ；short：空
//side和posSide组合方式，sell/long：强平多 ; buy short:强平空 >
//>bkPx    String 破产价格
//>sz	   String	强平数量
//>bkLoss  String	穿仓亏损数量
//>ccy	   String	强平币种，仅适用于币币杠杆
//>ts	   String	强平发生的时间，Unix时间戳的毫秒数格式，如 1597026383085
