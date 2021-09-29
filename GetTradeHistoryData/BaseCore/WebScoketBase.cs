using log4net;
using Newtonsoft.Json;
using QPP.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WebSocketSharp;


namespace GetTradeHistoryData
{
    /// <summary>
    /// The abstract class that responsible to get data from websocket
    /// </summary>
    /// <typeparam name="DataResponseType"></typeparam>
    public abstract class WebSocketClientBase : AbstractWebSocketClient
    {
        public delegate void OnConnectionOpenHandler();
        public event OnConnectionOpenHandler OnConnectionOpen;
        public delegate void OnResponseReceivedHandler(string response);
        public event OnResponseReceivedHandler OnResponseReceived;

      

        //protected const string DEFAULT_HOST = "api.huobi.pro";
        //protected const string WS_PATH = "ws";
        //protected const string FEED_PATH = "feed";
        private string _host;
        private string message;
        private bool _isporxy;

        protected WebSocket _WebSocket;
        public delegate void OnGetsedmessage();
        public event OnGetsedmessage OnGetmessage;


        private bool _autoConnect;
        private Timer _timer;
        private const int TIMER_INTERVAL_SECOND = 5;
        private DateTime _lastReceivedTime;
        private const int RECONNECT_WAIT_SECOND = 20;
        private const int RENEW_WAIT_SECOND = 120;
        public ILog LogHelpers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">websocket host</param>
        public WebSocketClientBase(string url,string sendmessage,bool isporxy)
        {
            message = sendmessage;
            LogHelpers = LogHelper.CreateInstance();
            //_host = $"{host}/{path}";
            _host = url;
            _timer = new Timer(TIMER_INTERVAL_SECOND * 1000);
            _timer.Elapsed += _timer_Elapsed;
            _isporxy = isporxy;
            InitializeWebSocket();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double elapsedSecond = (DateTime.UtcNow - _lastReceivedTime).TotalSeconds;
            //_logger.Log(Log.LogLevel.Trace, $"WebSocket received data {elapsedSecond.ToString("0.00")} sec ago");

            if (elapsedSecond > RECONNECT_WAIT_SECOND && elapsedSecond <= RENEW_WAIT_SECOND)
            {
                LogHelpers.Info("WebSocket reconnecting...");
                _WebSocket.Close();
                _WebSocket.Connect();
                Connect();
            }
            else if (elapsedSecond > RENEW_WAIT_SECOND)
            {
                LogHelpers.Info("WebSocket re-initialize...");
                Disconnect();
                UninitializeWebSocket();
                InitializeWebSocket();
                Connect();
            }
            else
            {
                Console.WriteLine(message+" 当前状态" + this._WebSocket.ReadyState + "-----" + this._WebSocket.IsAlive + DateTime.Now.ToString());
            }
        }

        private void InitializeWebSocket()
        {
            _WebSocket = new WebSocket(_host);
            _WebSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.None;
            if (_isporxy)
            {
              this._WebSocket.SetProxy("http://127.0.0.1:1080", "", "");
            }


            _WebSocket.OnError += _WebSocket_OnError;
            _WebSocket.OnOpen += _WebSocket_OnOpen;

            _lastReceivedTime = DateTime.UtcNow;
        }

        private void UninitializeWebSocket()
        {
            _WebSocket.OnOpen -= _WebSocket_OnOpen;
            _WebSocket.OnError -= _WebSocket_OnError;
            _WebSocket = null;
        }

        /// <summary>
        /// Connect to websocket server
        /// </summary>
        /// <param name="autoConnect">whether auto connect to server after it is disconnected</param>
        public override void Connect(bool autoConnect = true)
        {
            _WebSocket.OnMessage += _WebSocket_OnMessage;

            _WebSocket.Connect();

            OnGetmessage?.Invoke();
            //_WebSocket.Send( );
            _autoConnect = autoConnect;
            if (_autoConnect)
            {
                _timer.Enabled = true;
            }
        }

        /// <summary>
        /// Disconnect to websocket server
        /// </summary>
        public override void Disconnect()
        {
            _timer.Enabled = false;

            _WebSocket.OnMessage -= _WebSocket_OnMessage;


            _WebSocket.Close(CloseStatusCode.Normal);
        }

        private void _WebSocket_OnOpen(object sender, EventArgs e)
        {
            //_logger.Log(Log.LogLevel.Debug, "WebSocket opened");
            _lastReceivedTime = DateTime.UtcNow;

            OnConnectionOpen?.Invoke();
        }

        private void _WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            _lastReceivedTime = DateTime.UtcNow;
            string data = e.Data;
            if (e.IsBinary)
            {
                  data = GZipDecompresser.Decompress(e.RawData);
               
                if (data == null && data == "")
                {
                    Console.WriteLine(message + ":平台数据为空！");
                }
                else
                {                  
                    try
                    {                      
                        OnResponseReceived?.Invoke(data.ToString());
                    }
                    catch (Exception ex)
                    {
                        LogHelpers.Error(message+":发送错误，错误信息"+ex.Message.ToString());
                        Console.WriteLine(message + ":发送错误，错误信息" + ex.Message.ToString());
                    }
                }
            }
            else
            {        
                OnResponseReceived?.Invoke(data.ToString());
            }
        }

        private void _WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            //_logger.Log(Log.LogLevel.Error, $"WebSocket Error: {e.Message}");
        }


 
    }
}