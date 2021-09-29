using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;


//install-package WebSocket4Net
//install-package nlog
//install-package nlog.config
namespace CoinWin.DataGeneration
{
    public class WSocketClient : IDisposable
    {
        //日志管理


        #region 向外传递数据事件
        public event Action<string> MessageReceived;
        #endregion

        WebSocket _webSocket;
        /// <summary>
        /// 检查重连线程
        /// </summary>
        Thread _thread;
        bool _isRunning = true;
        /// <summary>
        /// WebSocket连接地址
        /// </summary>
        public string ServerPath { get; set; }

        public WSocketClient(string url)
        {
            ServerPath = url;
            this._webSocket = new WebSocket(url);
            this._webSocket.Opened += WebSocket_Opened;
            this._webSocket.Error += WebSocket_Error;
            this._webSocket.Closed += WebSocket_Closed;
            this._webSocket.MessageReceived += WebSocket_MessageReceived;
        }

        #region "web socket "
        /// <summary>
        /// 连接方法
        /// <returns></returns>
        public bool Start()
        {
            bool result = true;
            try
            {
                this._webSocket.Open();

                this._isRunning = true;
                this._thread = new Thread(new ThreadStart(CheckConnection));
                this._thread.Start();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 消息收到事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {

            Console.WriteLine(" Received:" + e.Message);
            LogHelper.WriteLog(" Received:" + e.Message);
            MessageReceived?.Invoke(e.Message);
        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelper.WriteLog("websocket_Closed");

        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, ErrorEventArgs e)
        {
            // _Logger.Info("websocket_Error:" + e.Exception.ToString());

            LogHelper.WriteLog(e.Exception.ToString());
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {


            LogHelper.WriteLog(" websocket_Opened");
        }
        /// <summary>
        /// 检查重连线程
        /// </summary>
        private void CheckConnection()
        {
            do
            {
                try
                {
                    if (this._webSocket.State != WebSocket4Net.WebSocketState.Open && this._webSocket.State != WebSocket4Net.WebSocketState.Connecting)
                    {


                        LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Open();
                        this._webSocket.Send(GetSendData());
                        Console.WriteLine("正在重连");
                    }
                }
                catch (Exception ex)
                {


                    LogHelper.WriteLog(ex.ToString());
                }
                System.Threading.Thread.Sleep(5000);
            } while (this._isRunning);
        }
        #endregion

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {
            Task.Factory.StartNew(() =>
            {
                if (_webSocket != null && _webSocket.State == WebSocket4Net.WebSocketState.Open)
                {
                    this._webSocket.Send(Message);
                }
            });
        }

        public void Dispose()
        {
            this._isRunning = false;
            try
            {
                _thread.Abort();
            }
            catch
            {

            }
            this._webSocket.Close();
            this._webSocket.Dispose();
            this._webSocket = null;
        }


        public string GetSendData()
        {
            string[] names = { "XBT/USD", "LINK/USD", "ETH/USD", "LTC/USD" };
            Rootobject ONES = new Rootobject();
            ONES.pair = names;
            ONES.subscription = new Subscription() { name = "trade" };


            Dictionary<string, string> drow = new Dictionary<string, string>();
            drow.Add("event", "subscribe");
            drow.Add("pair", names.ToJson());
            drow.Add("subscription", "{name: 'trade'}");
            return ONES.ToJson();
        }

    }
    public class Rootobject
    {

        public Rootobject()
        {
            this.@event = "subscribe";



        }
        public string @event { get; set; }
        public string[] pair { get; set; }
        public Subscription subscription { get; set; }
    }

    public class Subscription
    {
        public string name { get; set; }
    }
}
