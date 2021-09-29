using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public abstract class AbstractWebSocketClient
    {
 
        public abstract void Connect( bool autoConnect = true);

        public abstract void Disconnect();
    }
}
