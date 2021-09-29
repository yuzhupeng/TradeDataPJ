using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using static GetTradeHistoryData.CommandEnum;

namespace GetTradeHistoryData.BaseCore
{
    /// <summary>
    /// Responsible to handle MBP data from WebSocket
    /// </summary>
    public class OkexAuctuals : WebSocketClientBase
    {
        private List<USDTModel> symbollist;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">websockethost</param>
        public OkexAuctuals(string host, string sendmessage, bool isporxy) : base(host, sendmessage, isporxy)
        {
            this.OnResponseReceived += MessageOperation;
            this.OnGetmessage += SendMessages;
        }

        public void MessageOperation(string ts)
        {



            var results = JsonConvert.DeserializeObject<dynamic>(ts);

       

            //var resultdata = (((object)results.tick.data).ToString()).ToList<huobi>();
            //Console.WriteLine(resultdata.ToJson().ToString());
            Console.WriteLine(results.tick);
        }

    
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessages()
        {

            var ops = "{ \"op\": \"subscribe\", \"args\": [\"channel:books\",\"instId\":" + "BTC-USD-210625" + "]}";

          
            if (this._WebSocket != null && _WebSocket.ReadyState == WebSocketState.Open)
            {
                this._WebSocket.Send(ops);
            }

            //string list = string.Empty;
            //foreach (var item in this.symbollist)
            //{
            //    string topic = item.instrument_id;
            //    list += "\"futures/trade:" + topic + "\",";
            //}
            //list = list.Remove(list.Length - 1);
            //var ops = "{ \"op\": \"subscribe\", \"args\": [" + list + "]}";
            //if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //{
            //    this._webSocket.Send(ops);
            //}

        }
    }




}
