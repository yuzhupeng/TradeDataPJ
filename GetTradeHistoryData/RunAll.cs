using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace GetTradeHistoryData
{
   public class RunAll
    {
        public void RunallWebScoket(string symbol = "")
        {
            //string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,ADA,FIL,TRX,BNB";
            //string date = "210326";
            //binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/stream?streams=", Symbol, date);
            //binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
            //binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            new Thread(() =>
            {
             string Symbol = "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE";
              string date = "210326";
            binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/stream?streams=", Symbol,date);
            binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
            binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            }).Start();
            new Thread(() =>
            {
                string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,ADA,FIL,TRX,BNB";
                binanceUSDWebscoket binanceWebscoket = new binanceUSDWebscoket("wss://dstream.binance.com/stream?streams=", Symbol);
                binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
                binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            }).Start();
            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取Okex交割数据");
            //    OkexWebscoketDeliveryFutures Okexclient = new OkexWebscoketDeliveryFutures("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV", "210326");
            //    Okexclient.Start(Okexclient.GetSendData());
            //    Okexclient.SendMessage(Okexclient.GetSendData());
            //}).Start();
            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取Okex永续币本位和USDT数据");
            //    OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI");
            //    OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
            //    OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());
            //}).Start();
            //LogHelper.CreateInstance().Info("开始获取Okex交割数据");
            //OkexWebscoketDeliveryFutures Okexclient = new OkexWebscoketDeliveryFutures("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV", "210326");
            //Okexclient.Start(Okexclient.GetSendData());
            //Okexclient.SendMessage(Okexclient.GetSendData());
            //string Symbol = "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE";
            //binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/stream?streams=", Symbol);
            //binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
            //binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            //LogHelper.CreateInstance().Info("开始获取币本位永续huobi数据");
            //huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ETC", "USD");
            //client.Start(client.GetSendData());
            //client.SendMessage(client.GetSendData());
            //string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,UNI,DOGE";
            ////string Symbol = "ETC";
            //binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/stream?streams=", Symbol);
            //binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
            //binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            //Console.ReadKey(true);
            //LogHelper.CreateInstance().Info("关闭获取数据");
            //Console.WriteLine("关闭获取数据");
            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取币本位永续huobi数据");
            //    huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ETC", "USD");
            //    client.Start(client.GetSendData());
            //    client.SendMessage(client.GetSendData());
            //    Console.ReadKey(true);
            //    client.Dispose();
            //}).Start();
            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取USDThuobi数据");
            //    huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/linear-swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ETC", "USDT");
            //    client.Start(client.GetSendData());
            //    client.SendMessage(client.GetSendData());
            //    Console.ReadKey(true);
            //    client.Dispose();
            //}).Start();
        }
        /// <summary>
        /// 运行其他的WebScoket和api
        /// </summary>
        public void RunFutures(string symbol="",string coin="")
        {
 
     
            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取bybit usd数据");
            //    var coinlist = CommandEnum.ExchangeData.BybitCoinUsdlist;
            //    bybitWebScoket bybitWebScoket = new bybitWebScoket("wss://stream.bytick.com/realtime", coinlist);
            //    bybitWebScoket.Start(bybitWebScoket.GetSendData());
            //    bybitWebScoket.SendMessage(bybitWebScoket.GetSendData());
            //}).Start();
            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取bybit usdt数据");
            //    var coinusdtlist = CommandEnum.ExchangeData.BybitCoinUsdTlist;
            //    bybitWebScoket bybitWebScokets = new bybitWebScoket("wss://stream.bytick.com/realtime_public", coinusdtlist);
            //    bybitWebScokets.Start(bybitWebScokets.GetSendData());
            //    bybitWebScokets.SendMessage(bybitWebScokets.GetSendData());
            //}).Start();


            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取bybit usd数据");
                var coinlist = CommandEnum.ExchangeData.BybitCoinUsdlist;
                bybitWebScoket bybitWebScoket = new bybitWebScoket("wss://stream.bybit.com/realtime", coinlist);
                bybitWebScoket.Start(bybitWebScoket.GetSendData());
                bybitWebScoket.SendMessage(bybitWebScoket.GetSendData());
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取bybit usdt数据");
                var coinusdtlist = CommandEnum.ExchangeData.BybitCoinUsdTlist;
                bybitWebScoket bybitWebScokets = new bybitWebScoket("wss://stream.bybit.com/realtime_public", coinusdtlist);
                bybitWebScokets.Start(bybitWebScokets.GetSendData());
                bybitWebScokets.SendMessage(bybitWebScokets.GetSendData());
            }).Start();



            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取ftx永续和期货数据");
                //FtxMarketHelper.GetSymbollist("Part");
                //FTXWebScoket fTXWebScoket = new FTXWebScoket("wss://ftx.com/ws/", FtxMarketHelper.GetSymbollist("Part"), "0326");
                FTXWebScoket fTXWebScoket = new FTXWebScoket("wss://ftx.com/ws/", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE", "0326");
                fTXWebScoket.Start(fTXWebScoket.GetSendData());
                fTXWebScoket.SendMessage(fTXWebScoket.GetSendData());
            }).Start();




   
        }
        /// <summary>
        /// 运行永续的Webscoket
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="coin"></param>
        public void RunPermanentOKFutures(string symbol="", string coin="")
        {             
                new Thread(() =>
                {
                    LogHelper.CreateInstance().Info("开始获取Okex交割数据");
                    OkexWebscoketDeliveryFutures Okexclient = new OkexWebscoketDeliveryFutures("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV", "210326");
                    Okexclient.Start(Okexclient.GetSendData());
                    Okexclient.SendMessage(Okexclient.GetSendData());
                }).Start();
                new Thread(() =>
                {
                    LogHelper.CreateInstance().Info("开始获取Okex永续币本位和USDT数据");
                    OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ADA");
                    OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
                    OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());
                }).Start();
        }
        /// <summary>
        /// 币安
        /// </summary>
        /// <param name="symbol"></param>
        public void RunbinanceFutures(string symbol = "")
        {
            new Thread(() =>
            {
                string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,UNI,DOGE,ADA,TRX,BNB,AVAX,SUSHI";
                string date = "210326";
                binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/stream?streams=", Symbol, date);
                binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
                binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            }).Start();

            new Thread(() =>
            {
                string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,ADA,FIL,TRX,BNB";
                binanceUSDWebscoket binanceWebscoket = new binanceUSDWebscoket("wss://dstream.binance.com/stream?streams=", Symbol);
                binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
                binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            }).Start();

            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取bitmex数据");
            //    bitmexWebscoket bitmexclient = new bitmexWebscoket("wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD,trade:LTCUSD,trade:ETHUSD,liquidation:XBTUSD,liquidation:ETHUSD,liquidation:LTCUSD");
            //    //bitmexWebscoketnew bitmexclient = new bitmexWebscoketnew("wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD,trade:LTCUSD,trade:ETHUSD,liquidation:XBTUSD,liquidation:ETHUSD,liquidation:LTCUSD"); 
            //    bitmexclient.Start(bitmexclient.GetSendData());
            //    bitmexclient.SendMessage(bitmexclient.GetSendData());
            //}).Start();


        }
        /// <summary>
        /// 运行火币
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="coin"></param>
        public void RunhuobiFutures(string symbol="", string coin="")
        {
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取huobi交割数据");
                huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.hbdm.com/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV,ADA,FIL,ETC");
                clients.Start(clients.GetSendData());
                clients.SendMessage(clients.GetSendData());
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取币本位永续huobi数据");
                huobiWebScoket client = new huobiWebScoket("wss://api.hbdm.pro/swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USD");
                client.Start(client.GetSendData());
                client.SendMessage(client.GetSendData());
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取USDt永续 huobi数据");
                huobiWebScoket client = new huobiWebScoket("wss://api.hbdm.pro/linear-swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USDT");
                client.Start(client.GetSendData());
                client.SendMessage(client.GetSendData());
            }).Start();
        }

        public void runhuobis()
        {
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取huobi交割数据");
                huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.btcgateway.pro/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV,ADA,FIL,ETC");
                clients.Start(clients.GetSendData());
                clients.SendMessage(clients.GetSendData());
            }).Start();

            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取币本位永续huobi数据");
                huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USD");
                client.Start(client.GetSendData());
                client.SendMessage(client.GetSendData());
            }).Start();

            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取USDt永续 huobi数据");
                huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/linear-swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USDT");
                client.Start(client.GetSendData());
                client.SendMessage(client.GetSendData());
            }).Start();
        }

        public void runallstest()
        {
            RunhuobiFutures();//火币
            RunActuals();//bitmex deribit
            RunbinanceFutures();//币安
            RunPermanentOKFutures();//ok
            RunFutures();// ftx bitmex bybit
        }


        /// <summary>
        /// 
        /// </summary>
        public void RunActuals()
        {

           
                bitmexWebscoket bitmexclient = new bitmexWebscoket("wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD,trade:LTCUSD,trade:ETHUSD,liquidation:XBTUSD,liquidation:ETHUSD,liquidation:LTCUSD");
            bitmexclient.Start(bitmexclient.GetSendData());
            bitmexclient.SendMessage(bitmexclient.GetSendData());
           


            DERIBITWebScoket DERIBITWebScoket = new DERIBITWebScoket("wss://www.deribit.com/ws/api/v2");
            DERIBITWebScoket.Start(DERIBITWebScoket.GetSendData());
            DERIBITWebScoket.SendMessage(DERIBITWebScoket.GetSendData());



            LogHelper.CreateInstance().Info("开始获取kraken数据");
            KarkenFutureWSocketClient clients = new KarkenFutureWSocketClient("wss://futures.kraken.com/ws/v1");
            clients.Start(clients.GetSendData());
            clients.SendMessage(clients.GetSendData());



 
        }
        /// <summary>
        /// 运行其他的WebScoket和api
        /// </summary>
        public void RunALL(string symbol = "", string coin = "")
        {
            new Thread(() =>
            {
                string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,UNI,DOGE,ADA,TRX,BNB";
                string date = "210326";
                binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/stream?streams=", Symbol, date);
                binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
                binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            }).Start();
            new Thread(() =>
            {
                string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,ADA,FIL,TRX,BNB";
                binanceUSDWebscoket binanceWebscoket = new binanceUSDWebscoket("wss://dstream.binance.com/stream?streams=", Symbol);
                binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
                binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取bybit usd数据");
                var coinlist = CommandEnum.ExchangeData.BybitCoinUsdlist;
                bybitWebScoket bybitWebScoket = new bybitWebScoket("wss://stream.bytick.com/realtime", coinlist);
                bybitWebScoket.Start(bybitWebScoket.GetSendData());
                bybitWebScoket.SendMessage(bybitWebScoket.GetSendData());
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取bybit usdt数据");
                var coinusdtlist = CommandEnum.ExchangeData.BybitCoinUsdTlist;
                bybitWebScoket bybitWebScokets = new bybitWebScoket("wss://stream.bytick.com/realtime_public", coinusdtlist);
                bybitWebScokets.Start(bybitWebScokets.GetSendData());
                bybitWebScokets.SendMessage(bybitWebScokets.GetSendData());
            }).Start();
   
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取ftx永续和期货数据");
                FTXWebScoket fTXWebScoket = new FTXWebScoket("wss://ftx.com/ws/", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE", "0326");
                fTXWebScoket.Start(fTXWebScoket.GetSendData());
                fTXWebScoket.SendMessage(fTXWebScoket.GetSendData());
            }).Start();


            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取huobi交割数据");
                huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.hbdm.com/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV,ADA,FIL,ETC");
                clients.Start(clients.GetSendData());
                clients.SendMessage(clients.GetSendData());
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取币本位永续huobi数据");
                huobiWebScoket client = new huobiWebScoket("wss://api.hbdm.pro/swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USD");
                client.Start(client.GetSendData());
                client.SendMessage(client.GetSendData());
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取USDt永续 huobi数据");
                huobiWebScoket client = new huobiWebScoket("wss://api.hbdm.pro/linear-swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USDT");
                client.Start(client.GetSendData());
                client.SendMessage(client.GetSendData());
            }).Start();




            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取Okex交割数据");
                OkexWebscoketDeliveryFutures Okexclient = new OkexWebscoketDeliveryFutures("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV", "210326");
                Okexclient.Start(Okexclient.GetSendData());
                Okexclient.SendMessage(Okexclient.GetSendData());
            }).Start();
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取Okex永续币本位和USDT数据");
                OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI");
                OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
                OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());
            }).Start();
        }


        public void runspot()
        {
            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始获取kraken数据");
            KarkenSpotWSocketClient clients = new KarkenSpotWSocketClient("wss://ws.kraken.com");
            clients.Start(clients.GetSendData());
            clients.SendMessage(clients.GetSendData());

            }).Start();

            new Thread(() =>
            {
                BinanceSpotWebscoket clientb = new BinanceSpotWebscoket("wss://stream.binance.com:9443/stream?streams=", "BTC");
            clientb.Start(clientb.GetSendData());
            clientb.SendMessage("");
            }).Start();

            new Thread(() =>
            {
                HuobiSpotWebscoket client = new HuobiSpotWebscoket("wss://api.huobi.pro/ws", "");
            client.Start(client.GetSendData());
            client.SendMessage("");
            }).Start();


            new Thread(() =>
            {
                OkexWebscoketSpot okexclient = new OkexWebscoketSpot("wss://real.okex.com:8443/ws/v3", "");
            okexclient.Start(okexclient.GetSendData());
            okexclient.SendMessage("");
            }).Start();


        //    new Thread(() =>
        //    {
        //        FTXSpotWebScoket clients = new FTXSpotWebScoket("wss://ftx.com/ws/");
        //    clients.Start(clients.GetSendData());
        //    clients.SendMessage(clients.GetSendData());
        //}).Start();


    }


        public void runspots()
        {
            new Thread(() =>
            {
                CoinbaseProWebScoket coinbase = new CoinbaseProWebScoket("wss://ws-feed.pro.coinbase.com");
                coinbase.Start(coinbase.GetSendData());
                coinbase.SendMessage(coinbase.GetSendData());
            }).Start();

            new Thread(() =>
            {
                FTXSpotWebScoket clients = new FTXSpotWebScoket("wss://ftx.com/ws/");
                clients.Start(clients.GetSendData());
                clients.SendMessage(clients.GetSendData());
            }).Start();
        }





        /// <summary>
        /// 获取爆仓信息
        /// </summary>
        public void liquidationData()
        {
            BybitLQhelper p = new BybitLQhelper();
            HuobiliquidationHelper help = new HuobiliquidationHelper();
            OkexLQHelper o = new OkexLQHelper();





            new Thread(() =>
            {
                while (true)
                {
                    p.runtest();
                    Thread.Sleep(1000);
                }
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    help.runall();
                    Thread.Sleep(1000);
                }
            }).Start();


            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        o.runall();
            //        Thread.Sleep(1000);
            //    }
            //}).Start();


            new Thread(() =>
            {
                LogHelper.CreateInstance().Info("开始记录binance U本位合约（USDT）");
                binanceUSDWebscoket4NET binanceUSDT = new binanceUSDWebscoket4NET("", "USD");
                binanceUSDT.Start(binanceUSDT.GetSendData("1"));
                binanceUSDT.SendMessage(binanceUSDT.GetSendData("1"));


            }).Start();

            new Thread(() =>
            {

                LogHelper.CreateInstance().Info("开始记录binance 币本位合约（USD）");
                binanceUSDWebscoket4NET binanceUSD = new binanceUSDWebscoket4NET("", "USDT");
                binanceUSD.Start(binanceUSD.GetSendData("1"));
                binanceUSD.SendMessage(binanceUSD.GetSendData("1"));

            }).Start();

        }

        /// <summary>
        /// 获取费率和持仓
        /// </summary>
        public void FundingOpeninsert()
        {
            BybitFutureMarket bybit = new BybitFutureMarket();
            FtxMarketHelper ftx = new FtxMarketHelper();
            BinanceFuturesUsdtMarket binance = new BinanceFuturesUsdtMarket();
            HuobiliquidationHelper huobi = new HuobiliquidationHelper();
            OkexLQHelper o = new OkexLQHelper();
            BitmexMarket bit = new BitmexMarket();
            KarkenMarket kar = new KarkenMarket();


            //okex
            new Thread(() =>
            {
                while (true)
                {              
                    o.runopenfundingrate();
                    Thread.Sleep(300000);
                }
            }).Start();



            //bybit
            new Thread(() =>
            {
                while (true)
                {
                    bybit.GetFundingRateAndOpenInterest();
                    Thread.Sleep(300000);
                }
            }).Start();

            //ftx
            new Thread(() =>
            {
                while (true)
                {
                    ftx.GetFundingRateAndOpenInterest();
                    Thread.Sleep(300000);
                }
            }).Start();





            //binance
            string BINANCEFUTURESUSDTMARKET_DEFAULT_HOST = "https://fapi.binance.com/";
            string BINANCEFUTURESUSDMARKET_DEFAULT_HOST = "https://dapi.binance.com/";
            new Thread(() =>
            {
                while (true)
                {              
                    binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDMARKET_DEFAULT_HOST, "USD");
                    Thread.Sleep(300000);
                }
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDTMARKET_DEFAULT_HOST, "USDT");            
                    Thread.Sleep(300000);
                }
            }).Start();



            //huobi
            new Thread(() =>
            {
                while (true)
                {
                    huobi.runFundingRateAndOpenInterest();
                    Thread.Sleep(300000);
                }
            }).Start();


            //bitmex
            new Thread(() =>
            {
                while (true)
                {
                    bit.GetFundingRateAndOpenInterest();
                    Thread.Sleep(300000);
                }
            }).Start();

            //Karken
            new Thread(() =>
            {
                while (true)
                {
                    kar.GetFundingRateAndOpenInterest();
                    Thread.Sleep(300000);
                }
            }).Start();

        }

    }
}


//new Thread(() =>
//{
//    LogHelper.CreateInstance().Info("开始获取huobi交割数据");
//    huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.btcgateway.pro/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV");
//    clients.Start(clients.GetSendData());
//    clients.SendMessage(clients.GetSendData());
//}).Start();
//new Thread(() =>
//{
//    LogHelper.CreateInstance().Info("开始获取币本位永续huobi数据");
//    huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ETC","USD");
//    client.Start(client.GetSendData());
//    client.SendMessage(client.GetSendData());
//}).Start();
//new Thread(() =>
//{
//    LogHelper.CreateInstance().Info("开始获取USDt永续 huobi数据");
//    huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/linear-swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ETC", "USDT");
//    client.Start(client.GetSendData());
//    client.SendMessage(client.GetSendData());
//}).Start();
//new Thread(() =>
//{
//    LogHelper.CreateInstance().Info("开始获取Okex交割数据");
//OkexWebscoketDeliveryFutures Okexclient = new OkexWebscoketDeliveryFutures("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV", "210326");
//Okexclient.Start(Okexclient.GetSendData());
//Okexclient.SendMessage(Okexclient.GetSendData());
//}).Start();
//new Thread(() =>
//{
//    LogHelper.CreateInstance().Info("开始获取Okex永续币本位和USDT数据");
//OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI");
//OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
//OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());
//}).Start();
//string Symbol = "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE";
//binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/", Symbol);
//binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
//binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));
//Console.ReadKey(true);
//binanceWebscoket.Dispose();
//LogHelper.CreateInstance().Info("关闭获取数据");
//Console.WriteLine("关闭获取数据");
//FTXWebScoket fTXWebScoket = new FTXWebScoket("wss://ftx.com/ws/", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE", "0326");
//fTXWebScoket.Start(fTXWebScoket.GetSendData());
//fTXWebScoket.SendMessage(fTXWebScoket.GetSendData());
//Console.ReadKey(true);
//fTXWebScoket.Dispose();
//LogHelper.CreateInstance().Info("关闭获取数据");
//Console.WriteLine("关闭获取数据");
//火币
//huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.btcgateway.pro/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP");
//LogHelper.CreateInstance().Info("开始获取Okex数据");
//OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI");
//OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
//OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());
//Console.ReadKey(true);
//OkexWebscoketclient.Dispose();