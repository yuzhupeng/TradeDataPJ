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
    public class huobiAcutalsWebSocketClient : WebSocketClientBase
    {
        private List<USDTModel> symbollist;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">websockethost</param>
        public huobiAcutalsWebSocketClient(string host,string sendmessage,bool isporxy): base(host,sendmessage,isporxy)
        {
            this.OnResponseReceived+= MessageOperation;
            this.OnGetmessage += SendMessages;
        }

        public void MessageOperation(string ts)
        {


            var results = JsonConvert.DeserializeObject<dynamic>(ts);

            if ((object)results.tick == null)
            {
                if ((object)results.ping == null)
                {
                    Console.WriteLine("火币挂单无返回值！！！！" + ((object)results).ToString());
                    return;
                }
                Console.WriteLine("火币挂单回应ping！！！！" + (object)results.ping);
                string ping = ((object)results.ping).ToString();
                sendping("{\"pong\":" + ping + "}");
                return;
            }

            var resultdata = (((object)results.tick).ToString()).ToObject<HuobiQuatelMarkData>();
            CollectBigData(resultdata);

        }

        public void CollectBigData(HuobiQuatelMarkData t)
        {
            if (t != null)
            {
                //t.sellcount = Convert.ToDecimal(t.ask[1]);
                //t.buycount = Convert.ToDecimal(t.bid[1]);
                //t.buyprice = Convert.ToDecimal(t.bid[0]);
                //t.sellprice = Convert.ToDecimal(t.ask[0]);
                t.sellcount =  (t.ask[1]);
                t.buycount =  (t.bid[1]);
                t.buyprice =  (t.bid[0]);
                t.sellprice =  (t.ask[0]);
                t.times = GZipDecompresser.GetTimeFromUnixTimestampthree(t.ts.ToString());
                if (t.ask[1] > 10000 || t.bid[1] > 10000)
                {
                    Console.WriteLine(t.ToJson().ToString());
                    LogHelper.CreateInstance().Info("记录大数据：" + t.ToJson().ToString());        
                    RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.BBoQueueList, t.ToJson());        
                }
                else
                {
                    RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.AllBBoQueueList, t.ToJson());
                }
            }      
        }

        public void sendping(string op)
        {
            if (_WebSocket != null && _WebSocket.ReadyState == WebSocketState.Open)
            {
                this._WebSocket.Send(op);
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessages()
        {
            string topic = $"market.BTC_CQ.bbo";
            string clientId = "id7";
            var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");
            if (this._WebSocket != null && _WebSocket.ReadyState == WebSocketState.Open)
            {
                this._WebSocket.Send(op);
            }

            //symbollist = CommandEnum.PerpHUOBI.GetUSDContract_sizeapi();

            //if (true)
            //{
            //    foreach (var item in symbollist)
            //    {
            //        var symbols = $"{item.contract_code}";
            //        //string topic = $"market.{symbols}.depth.size_20.high_freq";
            //        string topic = $"market.{symbols}.bbo";
            //        string clientId = "id7";
            //        var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");
            //        if (this._WebSocket != null && _WebSocket.ReadyState == WebSocketState.Open)
            //        {
            //            this._WebSocket.Send(op);
            //        }
            //    }
            //}
            //else
            //{
            //    //  var result = CommandEnum.PerpHUOBI.GetContract_size();
            //    foreach (var item in symbollist)
            //    {
            //        var symbols = $"{item.contract_code}";
            //        string topic = $"market.{symbols}.trade.detail";
            //        string clientId = "id7";
            //        var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");
            //        if (_WebSocket != null && _WebSocket.ReadyState == WebSocketState.Open)
            //        {
            //            this._WebSocket.Send(op);
            //        }
            //    }
            //}


        }
    }



   
}
