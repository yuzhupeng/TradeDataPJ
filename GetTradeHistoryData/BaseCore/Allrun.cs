using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData.BaseCore
{
   public class Allrun
    {
        public void huobi()
        {

        //huobiAcutalsWebSocketClient o = new huobiAcutalsWebSocketClient("wss://api.hbdm.pro/swap-ws", "huobi",true);
        //o.Connect();
        // wss://api.btcgateway.pro/ws
      //  wss://api.hbdm.com/ws
            //huobiAcutalsWebSocketClient o = new huobiAcutalsWebSocketClient("wss://api.btcgateway.pro/ws", "huobi", false);
            //o.Connect();


            OkexAuctuals okex = new OkexAuctuals("wss://ws.okex.com:8443/ws/v5/public", "okex", true);
            okex.Connect();

        }
    }
}
