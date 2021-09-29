//Program q = new Program();

//var sq = q.GetFundingRateAsync("BTCUSDT");




//var list = CommandEnum.FTx.GetSpotSymbolList().Where(p => p.Type == "spot" && p.VolumeUsd24H > 1000000&&p.QuoteCurrency!="BTC");

//var list = CommandEnum.FTx.GetSpotSymbolList().Where(p => p.Type == "spot" && p.VolumeUsd24H > 1000000 && (p.QuoteCurrency == "USD" || p.QuoteCurrency == "USDT"));

//LogHelper.CreateInstance().Info("开始获取USDt永续 huobi数据");


//FTXSpotWebScoket clients = new FTXSpotWebScoket("wss://ftx.com/ws/");
//clients.Start(clients.GetSendData());
//clients.SendMessage(clients.GetSendData());



//BinanceFuturesUsdtMarket binance = new BinanceFuturesUsdtMarket();
//string BINANCEFUTURESUSDTMARKET_DEFAULT_HOST = "https://fapi.binance.com/";
//string BINANCEFUTURESUSDMARKET_DEFAULT_HOST = "https://dapi.binance.com/";

//binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDMARKET_DEFAULT_HOST, "USD");


//new Thread(() =>
//{
//while (true)
//{
//binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDTMARKET_DEFAULT_HOST, "USDT");

//binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDMARKET_DEFAULT_HOST, "USD");
//Thread.Sleep(300000);

//}
//}).Start();



//huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/linear-swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USDT");
//client.Start(client.GetSendData());
//client.SendMessage(client.GetSendData());


//var ftxlist = CommandEnum.FTx.GetFutureSymbolList().Where(p=>p.volumeUsd24h>10000000);
//var ftxlistjson = ftxlist.ToJson();


//var coinusdtlist = CommandEnum.ExchangeData.BybitCoinUsdTlist;
//bybitWebScoket bybitWebScokets = new bybitWebScoket("wss://stream.bytick.com/realtime_public", coinusdtlist);
//bybitWebScokets.Start(bybitWebScokets.GetSendData());
//bybitWebScokets.SendMessage(bybitWebScokets.GetSendData());

//OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ADA");
//OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
//OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());



//var ftxlist = CommandEnum.FTx.GetFutureSymbolList().Where(p=>p.volumeUsd24h>10000000);
//var ftxlistjson = ftxlist.ToJson();

//var ftxspotlist = CommandEnum.FTx.GetSpotSymbolList();
//var ftxjson = ftxspotlist.ToJson();



//var list = CommandEnum.BinanceMessage.GetSymoblUSDContract_sizeapi();
//var json = list.ToJson();



//var usdtlist= CommandEnum.BinanceMessage.GetSymoblUSDTContract_sizeapi();
//var usdtjson = usdtlist.ToJson();
//Console.ReadKey();



//new Thread(() =>
//{
//    LogHelper.CreateInstance().Info("开始获取Okex永续币本位和USDT数据");
//    OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ADA");
//    OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
//    OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());
//}).Start();



//DERIBITWebScoket DERIBITWebScoket = new DERIBITWebScoket("wss://www.deribit.com/ws/api/v2");
//DERIBITWebScoket.Start(DERIBITWebScoket.GetSendData());
//DERIBITWebScoket.SendMessage(DERIBITWebScoket.GetSendData());



//var lists = CommonProcess.GetKrakenAssetPair();
// var lists = CommonProcess.GetKrakenAssetPair().Where(p => p.PairName.Contains("USD")).ToList();
//Console.ReadKey();
//var ONES =lists.Where(p => p.PairName.Contains("BT")).ToList();

//var list= CommandEnum.BybitData.GetContract_size();

//LogHelper.CreateInstance().Info("开始获取kraken数据");
//KarkenSpotWSocketClient clients = new KarkenSpotWSocketClient("wss://ws.kraken.com");
//clients.Start(clients.GetSendData());
//clients.SendMessage(clients.GetSendData());


//var lists = CommonProcess.GetHuobiSymbol();
//var ones = lists.Where(p => p.symbol.Contains("usdt") && p.vol > 20000000 && !(p.symbol.Contains("3")));
//string sss = "";
//foreach (var item in ones.Where(p => p.symbol.Contains("usdt") && p.vol > 20000000 && !(p.symbol.Contains("3"))))
//{
//    var symbols = $"{item.symbol}";
//    string topic = $"market.{symbols}.trade.detail";
//    string clientId = "id7";
//    var op = ($" \"sub\": \"{topic}\",\"id\": \"{clientId}\",");
//    sss += op;
//}


//var symbol=    CommonProcess.GetOKEXSymbolticket();

// var symsss = symbol.Where(a=>a.instrument_id.Contains("USDT")&&Convert.ToDecimal(a.quote_volume_24h)>20000000);

// Console.ReadKey(true);

//var biannacelists = CommonProcess.GetBinanceSymbol().ToList().Where(a => a.QuoteAsset == "USDT" && a.Permissions.Length==1);

// var biannacelist= CommonProcess.GetBinanceSymbol().ToList().Where(a => a.QuoteAsset == "USDT"&&a.Permissions.Length>1);

// var lists=  CommonProcess.GetHuobiSymbol();
//var ones= lists.Where(p => p.symbol.Contains("usdt")&&p.vol>20000000&&!(p.symbol.Contains("3")));

// CommonProcess.GetHuobiSymbolticket();







#region Spot
//LogHelper.CreateInstance().Info("开始获取kraken数据");
//KarkenSpotWSocketClient clients = new KarkenSpotWSocketClient("wss://ws.kraken.com");
//clients.Start(clients.GetSendData());
//clients.SendMessage(clients.GetSendData());



//BinanceSpotWebscoket clientb = new BinanceSpotWebscoket("wss://stream.binance.com:9443/stream?streams=", "BTC");
//clientb.Start(clientb.GetSendData());
//clientb.SendMessage("");



//HuobiSpotWebscoket client = new HuobiSpotWebscoket("wss://api.huobi.pro/ws", "");
//client.Start(client.GetSendData());
//client.SendMessage("");



//OkexWebscoketSpot okexclient = new OkexWebscoketSpot("wss://real.okex.com:8443/ws/v3", "");
//okexclient.Start(okexclient.GetSendData());
//okexclient.SendMessage("");


#endregion
//Console.ReadKey(true);
//client.Dispose();



// Console.ReadKey();
//var usdt=CommandEnum.PerpHUOBI.GetContract_size().Where(p => p.create_date.Contains("2020")); ;
//var usd=CommandEnum.PerpHUOBI.GetUSDContract_size().Where(p=>p.create_date.Contains("2020"));

//FtxMarketHelper.GetSymbollist("Part");

//BybitFutureMarket bybitmark = new BybitFutureMarket();
//bybitmark.GetFundingRateAndOpenInterest();

//FtxMarketHelper ftx = new FtxMarketHelper();
//ftx.GetFundingRateAndOpenInterest();



//LogHelper.CreateInstance().Info("开始获取kraken数据");
//KarkenWSocketClient client = new KarkenWSocketClient("wss://ws.kraken.com");
//client.Start(client.GetSendData());
//client.SendMessage(client.GetSendData());
//Console.ReadKey(true);
//client.Dispose();



//QuenProcess t = new QuenProcess();
//t.CalcPere();

//string BINANCEFUTURESUSDTMARKET_DEFAULT_HOST = "https://fapi.binance.com/";


//string BINANCEFUTURESUSDMARKET_DEFAULT_HOST = "https://dapi.binance.com/";

//BinanceFuturesUsdtMarket binance = new BinanceFuturesUsdtMarket();
//binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDTMARKET_DEFAULT_HOST,"USDT");



//HuobiliquidationHelper huobi = new HuobiliquidationHelper();


//USDT永续
//string fundingurl = "/linear-swap-api/v1/swap_batch_funding_rate";
//string openinteresturl = $"/linear-swap-api/v1/swap_open_interest";
//huobi.GetFundingRateAndOpenInterest(fundingurl, openinteresturl, false, "USDT");

//币本位永续
//string fundingurl = "/swap-api/v1/swap_batch_funding_rate";
//string openinteresturl = "/swap-api/v1/swap_open_interest";
//huobi.GetFundingRateAndOpenInterest(fundingurl, openinteresturl, false, "USD");

//交割
//string fundingurl = "";
//string openinteresturl = $"/api/v1/contract_open_interest";
//huobi.GetFundingRateAndOpenInterest(fundingurl, openinteresturl, true, "USDT");


//BybitLQhelper p = new BybitLQhelper();
//HuobiliquidationHelper help = new HuobiliquidationHelper();



//new Thread(() =>
//{

//    while (true)
//    {
//        p.runtest();
//        Thread.Sleep(20000);
//    }
//}).Start();

//new Thread(() =>
//{
//    while (true)
//    {
//        help.runall();
//        Thread.Sleep(20000);

//    }
//}).Start();


//BybitLQhelper p = new BybitLQhelper();
//HuobiliquidationHelper help = new HuobiliquidationHelper();

//new Thread(() =>
//{
//    p.runtest();
//}).Start();

//new Thread(() =>
//{
//    help.runall();
//}).Start();





//var counts=   CommandEnum.PerpHUOBI.GetContract_size();

//BybitLQhelper p = new BybitLQhelper();
//p.runtest();


//HuobiliquidationHelper helps = new HuobiliquidationHelper();

//helps.runall();


//KarkenSpotWSocketClient clients = new KarkenSpotWSocketClient("wss://ws.kraken.com");
//clients.Start(clients.GetSendData());
//clients.SendMessage(clients.GetSendData());


//HuobiliquidationHelper help = new HuobiliquidationHelper();
//help.runall();



//RunAll o = new RunAll();
////o.liquidationData();

//o.FundingOpeninsert();

//OkexLQHelpernew o = new OkexLQHelpernew();
//o.runlqdata();
//o.runall();
//o.runopenfundingrate();





//BitmexMarket a = new BitmexMarket();
//a.GetFundingRateAndOpenInterest();


//RunAll o = new RunAll();
//o.liquidationData();
//o.FundingOpeninsert();
//o.liquidationData();
//o.RunhuobiFutures();//火币
//o.runhuobis();
//  o.RunActuals(); //Deribit bitmex
//o.RunFutures();//ftx bybit 

// o.RunbinanceFutures();//币安
//o.RunPermanentOKFutures();//ok

//o.runspot();
//OkexLQHelper o = new OkexLQHelper();
//o.runopenfundingrate();

////o.RunbinanceFutures();//币安
////  o.RunActuals();//测试
//  //o.RunFutures();// ftx bitmex bybit
//o.runhuobis();//火币
//o.RunPermanentOKFutures();//ok
//o.RunhuobiFutures();//测试
//o.RunhuobiFutures();// 

//o.RunFutures();// ftx bitmex bybit
//o.RunhuobiFutures();//火币



//o.RunbinanceFutures();//币安
//o.RunFutures();// ftx bitmex bybit
//o.RunhuobiFutures();//火币

//HuobiliquidationHelper help = new HuobiliquidationHelper();

//help.runall();

//RunAll o = new RunAll();




//LogHelper.CreateInstance().Info("开始获取Okex永续币本位和USDT数据");
//OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ADA");
//OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
//OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());



//DERIBITWebScoket DERIBITWebScoket = new DERIBITWebScoket("wss://www.deribit.com/ws/api/v2");
//DERIBITWebScoket.Start(DERIBITWebScoket.GetSendData());
//DERIBITWebScoket.SendMessage(DERIBITWebScoket.GetSendData());

//o.RunActuals();
//o.liquidationData();
//bitmexWebscoket bitmexclient = new bitmexWebscoket("wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD,trade:LTCUSD,trade:ETHUSD,liquidation:XBTUSD,liquidation:ETHUSD,liquidation:LTCUSD");
//bitmexclient.Start(bitmexclient.GetSendData());
//bitmexclient.SendMessage(bitmexclient.GetSendData());


//string Symbol = "BTC,LTC,BCH,ETH,EOS,ETC,LINK,DOT,XRP,UNI,DOGE,ADA,TRX,BNB,AVAX,SUSHI";
//string date = "210326";
//binanceWebscoket binanceWebscoket = new binanceWebscoket("wss://fstream.binance.com/stream?streams=", Symbol, date);
//binanceWebscoket.Start(binanceWebscoket.GetSendData(Symbol));
//binanceWebscoket.SendMessage(binanceWebscoket.GetSendData(Symbol));

//RunAll o = new RunAll();

//o.RunhuobiFutures();//火币

//o.RunbinanceFutures();//币安

//o.RunPermanentOKFutures();//ok

//o.RunActuals();

//o.RunFutures();// ftx bitmex bybit

//o.RunhuobiFutures();
//   o.runhuobis();//火币
//o.RunActuals();
//o.RunPermanentOKFutures();//ok
//o.RunbinanceFutures();//币安
//o.runspot();


//o.RunhuobiFutures();//火币


//o.RunhuobiFutures();//火币
//o.RunPermanentOKFutures();//ok
//o.RunbinanceFutures();//币安
//o.RunFutures();// ftx bitmex bybit
//o.RunActuals();//测试
