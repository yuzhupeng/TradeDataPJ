using log4net;
using Newtonsoft.Json;
using QPP.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GetTradeHistoryData
{
    public class DERIBITWebScoket : IDisposable
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

        /// <summary>
        /// 重试发出的消息
        /// </summary>
        public string Sendata { get; set; }

        ILog LogHelpers;

        string symbol;

        string delivery_date;


        /// <summary>
        /// DERIBIT
        /// </summary>
        /// <param name="url"></param>
        /// <param name="symbol"></param>
        /// <param name="datecode"></param>
        public DERIBITWebScoket(string url)
        {
            this.symbol = "ETH,BTC";
           

            LogHelpers = LogHelper.CreateInstance();
            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://www.deribit.com/ws/api/v2";
            }
            this._webSocket = new WebSocket(ServerPath);
            this._webSocket.OnOpen += WebSocket_Opened;
            this._webSocket.OnError += WebSocket_Error;
            this._webSocket.OnClose += WebSocket_Closed;
            this._webSocket.OnMessage += WebSocket_MessageReceived;
        }

        #region "web socket "
        /// <summary>
        /// 连接方法
        /// <returns></returns>
        public bool Start(string senddata)
        {
            Sendata = senddata;
            bool result = true;
            try
            {
                this._webSocket.Connect();

                this._isRunning = true;
                this._thread = new Thread(new ThreadStart(CheckConnection));
                this._thread.Start();
            }
            catch (Exception ex)
            {
                LogHelpers.Info(" DERIBIT永续合约启动失败：" + ex.ToString());

                Console.WriteLine(" DERIBIT永续合约启动失败：" + ex.ToString());
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 消息收到事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {
             
            if (e.IsPing)
            {
                Console.WriteLine(" DERIBIT永续合约消息ping");
                return;
            }
            //Console.WriteLine(" Received:" + e.Data);
            //LogHelpers.Info(" Received:" + e.Data);
            //MessageReceived?.Invoke(e.Data);
            //Task.Run(() =>
            //{
                Savedata(e.Data);

            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.Deribit, CommandEnum.RedisKey.Deribit+this.ServerPath);
            });

        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelpers.Info(" DERIBIT永续合约websocket_Closed：" + e.ToString());

            Console.WriteLine(" DERIBIT永续websocket_Closed：" + e.ToString());
            //_webSocket.Close();
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
           
            LogHelpers.Info(" DERIBIT永续合约websocket_Error：" + e.ToString());

            Console.WriteLine(" DERIBIT永续合约websocket_Error：" + e.ToString());
            //LogHelper.WriteLog(e.Exception.ToString());
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {
            LogHelpers.Info(" websocket_Opened");
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
                    if (this._webSocket.IsAlive != true || this._webSocket.ReadyState != WebSocketState.Open)
                    {


                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Connect();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("DERIBIT 正在重连");
                        Console.WriteLine("DERIBIT 正在重连");
                    }
                    else
                    {
                        LogHelpers.Info("DERIBIT 合约" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive+DateTime.Now.ToString());
                        Console.WriteLine("DERIBIT 合约" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DERIBIT 合约正在重连异常" + ex.ToString());
                    LogHelpers.Error("DERIBIT 合约重连websocket_Error:" + ex.ToString());

                }
                System.Threading.Thread.Sleep(5000);
            } while (this._isRunning);
        }
        #endregion


        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="message"></param>
        public void Savedata(string message)
        {
            if (message != null && message != "")
            {
                try
                {
                    var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    if (results.@params.data == null)
                    {
                        Console.WriteLine("DERIBIT 无返回值！！！！" + ((object)results).ToString());
                        return;
                    }
                    var resultdata = (((object)results.@params.data).ToString()).ToList<Deribit>();
                    List<string> maxlist = new List<string>();
                    if (resultdata != null && resultdata.Count > 0)
                    {
                        //var tradeIDlist = resultdata.GroupBy(a => a.id).ToList();
                        List<UPermanentFutures> list = new List<UPermanentFutures>();
                        string markert = resultdata.FirstOrDefault().instrument_name;
                        string[] arr3 = markert.Split('-');

                        var pair = arr3[0];//ETH
                        var coin = arr3[1];//0326/PERE

                      
                         PERPFuture(resultdata, list, pair, markert);                      
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("DERIBIT 信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("DERIBIT 信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("DERIBIT 信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("信息为空，无法保存");
                Console.WriteLine("信息为空，无法保存");
            }
        }

        private void PERPFuture(List<Deribit> resultdata, List<UPermanentFutures> list,string pair,string market)
        {

            List<string> maxlist = new List<string>();
            foreach (var item in resultdata)
            {
                var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.timestamp.ToString()); ;
                var key = times.ToString("g");
                var keys = times.ToString("d");
                item.actcualtime = times;
                UPermanentFutures model = new UPermanentFutures();
                model = Mapping(item, pair, market);
                model.kind = CommandEnum.RedisKey.PERP;
                list.Add(model);

                //var sizes = resultdata.Where(p => p.timestamp == item.timestamp).ToList().Sum(p => (Convert.ToDecimal(p.amount)));

 
                //if (sizes >= 1000000 && maxlist.FirstOrDefault(a => a == item.timestamp.ToString()) == null)
                //{
                //    UPermanentFutures models = new UPermanentFutures();
                //    ObjectUtil.MapTo(model, models);
                //    models.vol = sizes.ToString();
                //    models.qty = (sizes / Convert.ToDecimal(models.price)).ToString();
                //    RedisHelper.Pushdata(models.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));

                //    maxlist.Add(item.timestamp.ToString());
                //    Console.WriteLine(models.ToJson());
                //}
               
            }

            var sizes = list.Sum(p => Convert.ToDecimal(p.vol));
            if (sizes >= 1000000)
            {
                UPermanentFutures models = new UPermanentFutures();
                ObjectUtil.MapTo(list.FirstOrDefault(), models);
                models.vol = sizes.ToString();
                models.qty = (sizes / Convert.ToDecimal(models.price)).ToString();
                RedisHelper.Pushdata(models.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                Console.WriteLine(models.ToJson());
            }



            RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);

          
            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.PERPQueueList, list.ToJson());
         



            //if(market.Contains("PERP"))
            //RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);
            //else
            //    RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UDeliveryFutures);
        }

        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Deribit model,string pair,string market)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.trade_seq.ToString();
            o.exchange = CommandEnum.RedisKey.Deribit;
            o.market = market;
            o.pair = pair;
            o.price = model.price.ToString();
            o.qty = (model.amount / model.price).ToString();
            o.side = model.direction;
            //o.vol = ((Convert.ToDecimal(model.price)) * Convert.ToDecimal(model.amount)).ToString();
            o.vol = model.amount.ToString();
            o.utctime = model.actcualtime.ToString();
            o.times = model.actcualtime.AddHours(8).ToString();
            o.timestamp = model.timestamp.ToString(); ;
            o.contractcode = pair;

            o.Unit = "USDT";
            o.types = "永续U本位合约:" + market;
            return o;
        }






        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string message)
        {

            string[] arr1 = symbol.Split(',');


            foreach (var item in arr1)
            {

                //string topic = "public/subscribe";
            
                //string market = $"'trades.'{item}'.raw'";
            
                //var ops = $"{{\"method\": \"{topic}\"}}"+",\"params\": {channels:[" + market + "]}}";

                var ONES = "{\"method\":\"public/subscribe\",\"params\":{ \"channels\":[\"trades."+ item + "-PERPETUAL.raw\"]}}";


                if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                {
                    this._webSocket.Send(ONES.ToString());
                }

            }

         

        }
        public string GetSendData()
        {
            string topic = "subscribe";

            string clientId = "trades";

            string market = "BTC-PERP";

            var results = ($"{{ \"op\": \"{topic}\",\"channel\": \"{clientId}\",\"market\": \"{market}\" }}");

            return "1";
           
        }
        public void Dispose()
        {
            //this._isRunning = false;
            //try
            //{
            //    _thread.Abort();
            //}
            //catch
            //{

            //}
            //this._webSocket.Close();
            ////this._webSocket.Dispose();
            //this._webSocket = null;
        }
    }
}
