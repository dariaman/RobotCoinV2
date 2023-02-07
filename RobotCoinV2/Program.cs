﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RobotCoinV2;

var setttings = new Settings();
var TELEGRAM_TOKEN_BOT = setttings.TELEGRAM_TOKEN_BOT;
var TELEGRAM_CHATID_ERROR = setttings.TELEGRAM_CHATID_ERROR;
var TELEGRAM_CHATID_STATUS = setttings.TELEGRAM_CHATID_STATUS;
var TELEGRAM_CHATID_INFO = setttings.TELEGRAM_CHATID_INFO;

var INDODAX_PRICE_URL = setttings.INDODAX_PRICE_URL;
var NICEHASH_PRICE_URL = setttings.NICEHASH_PRICE_URL;

var AWS_ACCESS_KEY = setttings.AWS_ACCESS_KEY;
var AWS_SECRET_KEY = setttings.AWS_SECRET_KEY;

var GAP_NAIK = setttings.GAP_NAIK;
var GAP_TURUN = setttings.GAP_TURUN;
var LAST_HOUR = setttings.LAST_HOUR;

DateTime DATE_NOW = DateTime.Now;

TelegramBot _telegram = new(TELEGRAM_TOKEN_BOT, TELEGRAM_CHATID_ERROR, TELEGRAM_CHATID_STATUS, TELEGRAM_CHATID_INFO);

if (AWS_ACCESS_KEY == null || AWS_SECRET_KEY == null)
{
    await _telegram.SendErrorAsync(DATE_NOW.ToString("yyyyMMddHHmmss") + "\naccessKey or secretKey is null");
    Environment.Exit(0);
}

if (INDODAX_PRICE_URL == null)
{
    await _telegram.SendErrorAsync(DATE_NOW.ToString("yyyyMMddHHmmss") + "\nIndodaxPriceUrl is null");
    Environment.Exit(0);
}

if (NICEHASH_PRICE_URL == null)
{
    await _telegram.SendErrorAsync(DATE_NOW.ToString("yyyyMMddHHmmss") + "\nNicehash Env is null");
    Environment.Exit(0);
}

if (TELEGRAM_TOKEN_BOT == null || TELEGRAM_CHATID_ERROR == null || TELEGRAM_CHATID_STATUS == null || TELEGRAM_CHATID_INFO == null)
{
    await _telegram.SendErrorAsync(DATE_NOW.ToString("yyyyMMddHHmmss") + "\nTelegram Key is null");
    Environment.Exit(0);
}

await _telegram.SendStatusAsync("Tgl start =>" + DATE_NOW.ToString("dd MMM yyyy HH:mm:ss") + $" >> " + DATE_NOW.ToString("yyyyMMddHHmmss"));

var awsCredentials = new BasicAWSCredentials(AWS_ACCESS_KEY, AWS_SECRET_KEY);
var client_db = new AmazonDynamoDBClient(awsCredentials, RegionEndpoint.APSoutheast1); // singapore

var PriceNicehash = await GetPriceNicehashAsync();
var PriceIndodax = await GetPriceIndodaxAsync();

///* Insert Data Coin

if (PriceNicehash != null || PriceIndodax != null)
{
    if (PriceNicehash?.AAVEUSDT > 0 || PriceNicehash?.AAVEBTC > 0 || PriceIndodax?.aave_idr > 0)
        await InsertCoin(client_db, "AAVE", PriceNicehash?.AAVEUSDT, PriceNicehash?.AAVEBTC, PriceIndodax?.aave_idr);

    if (PriceNicehash?.ADAUSDT > 0 || PriceNicehash?.ADABTC > 0 || PriceIndodax?.ada_idr > 0)
        await InsertCoin(client_db, "ADA", PriceNicehash?.ADAUSDT, PriceNicehash?.ADABTC, PriceIndodax?.ada_idr);

    if (PriceNicehash?.BTCUSDT > 0 || PriceIndodax?.btc_idr > 0)
        await InsertCoin(client_db, "BTC", PriceNicehash?.BTCUSDT, 1, PriceIndodax?.btc_idr);

    if (PriceNicehash?.ETHUSDT > 0 || PriceNicehash?.ETHBTC > 0 || PriceIndodax?.eth_idr > 0)
        await InsertCoin(client_db, "ETH", PriceNicehash?.ETHUSDT, PriceNicehash?.ETHBTC, PriceIndodax?.eth_idr);

    if (PriceNicehash?.ONEINCHUSDT > 0 || PriceNicehash?.ONEINCHBTC > 0 || PriceIndodax?.inch_idr > 0)
        await InsertCoin(client_db, "INCH", PriceNicehash?.ONEINCHUSDT, PriceNicehash?.ONEINCHBTC, PriceIndodax?.inch_idr);

    if (PriceNicehash?.CHZUSDT > 0 || PriceNicehash?.CHZBTC > 0 || PriceIndodax?.chz_idr > 0)
        await InsertCoin(client_db, "CHZ", PriceNicehash?.CHZUSDT, PriceNicehash?.CHZBTC, PriceIndodax?.chz_idr);

    if (PriceNicehash?.CRVUSDT > 0 || PriceNicehash?.CRVBTC > 0 || PriceIndodax?.crv_idr > 0)
        await InsertCoin(client_db, "CRV", PriceNicehash?.CRVUSDT, PriceNicehash?.CRVBTC, PriceIndodax?.crv_idr);

    if (PriceNicehash?.DOGEUSDT > 0 || PriceNicehash?.DOGEBTC > 0 || PriceIndodax?.doge_idr > 0)
        await InsertCoin(client_db, "DOGE", PriceNicehash?.DOGEUSDT, PriceNicehash?.DOGEBTC, PriceIndodax?.doge_idr);

    if (PriceNicehash?.GRTUSDT > 0 || PriceNicehash?.GRTBTC > 0 || PriceIndodax?.grt_idr > 0)
        await InsertCoin(client_db, "GRT", PriceNicehash?.GRTUSDT, PriceNicehash?.GRTBTC, PriceIndodax?.grt_idr);

    if (PriceNicehash?.HBARUSDT > 0 || PriceNicehash?.HBARBTC > 0 || PriceIndodax?.hbar_idr > 0)
        await InsertCoin(client_db, "HBAR", PriceNicehash?.HBARUSDT, PriceNicehash?.HBARBTC, PriceIndodax?.hbar_idr);

    if (PriceNicehash?.LRCUSDT > 0 || PriceNicehash?.LRCBTC > 0 || PriceIndodax?.lrc_idr > 0)
        await InsertCoin(client_db, "LRC", PriceNicehash?.LRCUSDT, PriceNicehash?.LRCBTC, PriceIndodax?.lrc_idr);

    if (PriceNicehash?.LTCUSDT > 0 || PriceNicehash?.LTCBTC > 0 || PriceIndodax?.ltc_idr > 0)
        await InsertCoin(client_db, "LTC", PriceNicehash?.LTCUSDT, PriceNicehash?.LTCBTC, PriceIndodax?.ltc_idr);

    if (PriceNicehash?.RVNUSDT > 0 || PriceNicehash?.RVNBTC > 0 || PriceIndodax?.rvn_idr > 0)
        await InsertCoin(client_db, "RVN", PriceNicehash?.RVNUSDT, PriceNicehash?.RVNBTC, PriceIndodax?.rvn_idr);

    if (PriceNicehash?.SANDUSDT > 0 || PriceNicehash?.SANDBTC > 0 || PriceIndodax?.sand_idr > 0)
        await InsertCoin(client_db, "SAND", PriceNicehash?.SANDUSDT, PriceNicehash?.SANDBTC, PriceIndodax?.sand_idr);

    if (PriceNicehash?.SUSHIUSDT > 0 || PriceNicehash?.SUSHIBTC > 0 || PriceIndodax?.sushi_idr > 0)
        await InsertCoin(client_db, "SUSHI", PriceNicehash?.SUSHIUSDT, PriceNicehash?.SUSHIBTC, PriceIndodax?.sushi_idr);

    if (PriceNicehash?.UNIUSDT > 0 || PriceNicehash?.UNIBTC > 0 || PriceIndodax?.uni_idr > 0)
        await InsertCoin(client_db, "UNI", PriceNicehash?.UNIUSDT, PriceNicehash?.UNIBTC, PriceIndodax?.uni_idr);

    if (PriceNicehash?.XRPUSDT > 0 || PriceNicehash?.XRPBTC > 0 || PriceIndodax?.xrp_idr > 0)
        await InsertCoin(client_db, "XRP", PriceNicehash?.XRPUSDT, PriceNicehash?.XRPBTC, PriceIndodax?.xrp_idr);

    Thread.Sleep(500); // sleep 3s biar bisa terbaca data yang baru diinsert
}
//End Insert Data Coin  */

///* Baca Data Coin 
var data_coin = await GetLast2HoursCoinPriceAsync(client_db);

if (data_coin.Count > 0)
{
    Decimal percentGapBTC;
    Decimal percentGapUSDT;
    Decimal percentGapIDR;
    string pesan = "";
    foreach (var _coin in setttings.FavoriteCoinList)
    {
        percentGapBTC = 0.0M;
        percentGapUSDT = 0.0M;
        percentGapIDR = 0.0M;
        var currentPrice = data_coin.Where(x => x.CoinCode == _coin).OrderByDescending(x => x.DateString).FirstOrDefault();
        var lastPrice = data_coin.Where(x => x.CoinCode == _coin).OrderBy(x => x.DateString).FirstOrDefault();

        if (currentPrice?.BTC > 0 && lastPrice?.BTC > 0) percentGapIDR = ((currentPrice.BTC - lastPrice.BTC) / lastPrice.BTC) * 100;
        if (currentPrice?.USDT > 0 && lastPrice?.USDT > 0) percentGapUSDT = ((currentPrice.USDT - lastPrice.USDT) / lastPrice.USDT) * 100;
        if (currentPrice?.IDR > 0 && lastPrice?.IDR > 0) percentGapIDR = ((currentPrice.IDR - lastPrice.IDR) / (decimal)lastPrice.IDR) * 100;

        if (percentGapBTC <= GAP_TURUN || percentGapBTC >= GAP_NAIK || percentGapUSDT <= GAP_TURUN || percentGapUSDT >= GAP_NAIK || percentGapIDR <= GAP_TURUN || percentGapIDR >= GAP_NAIK)
            pesan += $"{(string.IsNullOrEmpty(pesan) ? "" : "\n\n")}<b>{_coin}</b> curs ";

        if (percentGapBTC <= GAP_TURUN || percentGapBTC >= GAP_NAIK)
            pesan += $"\n<b>{(percentGapBTC < 0 ? "<u>" : "")}BTC={percentGapBTC.ToString()[..Math.Min(5, percentGapBTC.ToString().Length)]}% {(percentGapBTC < 0 ? "</u>" : "")}</b>\n" +
                $"currPrice={currentPrice?.BTC} \nlast {Math.Abs(LAST_HOUR)}hours Price={lastPrice?.BTC}";

        if (percentGapUSDT <= GAP_TURUN || percentGapUSDT >= GAP_NAIK)
            pesan += $"\n<b>{(percentGapUSDT < 0 ? "<u>" : "")}USDT={percentGapUSDT.ToString()[..Math.Min(5, percentGapUSDT.ToString().Length)]}% {(percentGapUSDT < 0 ? "</u>" : "")}</b>\n" +
                $"currPrice={currentPrice?.USDT} \nlast {Math.Abs(LAST_HOUR)}hours Price={lastPrice?.USDT}";

        if (percentGapIDR <= GAP_TURUN || percentGapIDR >= GAP_NAIK)
            pesan += $"\n<b>{(percentGapIDR < 0 ? "<u>" : "")}IDR={percentGapIDR.ToString()[..Math.Min(5, percentGapIDR.ToString().Length)]}% {(percentGapIDR < 0 ? "</u>" : "")}</b>\n" +
                $"currPrice={currentPrice?.IDR:n0} \nlast {Math.Abs(LAST_HOUR)}hours Price={lastPrice?.IDR:n0}";
    }
    if (!string.IsNullOrEmpty(pesan)) await _telegram.SendMessageAsync(pesan);
}

/* End Baca Data Coin */
await _telegram.SendStatusAsync("Finish =>" + DATE_NOW.ToString("dd MMM yyyy HH:mm:ss") + $" >> " + DATE_NOW.ToString("yyyyMMddHHmmss"));

async Task InsertCoin(IAmazonDynamoDB client, string CoinCode, decimal? usdt, decimal? btc, int? idr)
{
    string TableName = "CoinPrice";
    await CreateTableIfExist(client, TableName);

    var LastPrice = await GetLastCoinPriceAsync(client, CoinCode);
    if (LastPrice.USDT == usdt && LastPrice.BTC == btc && LastPrice.IDR == idr) return;

    ///// write data to table
    try
    {
        Table tabel_coin = Table.LoadTable(client, TableName);
        var price_usdt = new Document
        {
            ["CoinCode"] = CoinCode,
            ["DateString"] = DATE_NOW.ToString("yyyyMMddHHmmss"),
            ["USDT"] = usdt,
            ["BTC"] = btc,
            ["IDR"] = idr
        };
        await tabel_coin.PutItemAsync(price_usdt);
    }
    catch (Exception ex)
    {
        await _telegram.SendErrorAsync("Tgl =>" + DATE_NOW.ToString("dd MMM yyyy HH:mm:ss") + "\n" + (ex.InnerException?.Message ?? ex.Message));
    }
}

async Task<CoinPrice> GetLastCoinPriceAsync(IAmazonDynamoDB client, string CoinCode)
{
    CoinPrice LatestPrice = new();

    var request = new QueryRequest
    {
        TableName = "CoinPrice",
        KeyConditionExpression = $"CoinCode = :vCoinCode",
        ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":vCoinCode", new AttributeValue { S = CoinCode } } },
        // Optional parameter.
        ConsistentRead = true,
        Limit = 1,
        ExclusiveStartKey = null,
        ScanIndexForward = false
    };

    var response = await client.QueryAsync(request);

    if (response.Items.Count > 0)
    {

        foreach (Dictionary<string, AttributeValue> item in response.Items)
        {

            foreach (KeyValuePair<string, AttributeValue> kvp in item)
            {
                if (kvp.Key.ToUpper() == "USDT") LatestPrice.USDT = decimal.Parse(kvp.Value.N);
                if (kvp.Key.ToUpper() == "BTC") LatestPrice.BTC = decimal.Parse(kvp.Value.N);
                if (kvp.Key.ToUpper() == "IDR") LatestPrice.IDR = Int32.Parse(kvp.Value.N);
            }
        }
    }
    return LatestPrice;
}

async Task<List<CoinPrice>> GetLast2HoursCoinPriceAsync(IAmazonDynamoDB client)
{
    List<CoinPrice> TwoHoursCoinPrice = new();
    try
    {
        DynamoDBContext _context;
        _context = new DynamoDBContext(client, new DynamoDBContextConfig { ConsistentRead = true });
        var search = _context.ScanAsync<CoinPrice>(new[] { new ScanCondition("DateString", ScanOperator.GreaterThanOrEqual, DATE_NOW.AddHours(LAST_HOUR).ToString("yyyyMMddHHmmss")) });
        TwoHoursCoinPrice = await search.GetRemainingAsync();
    }
    catch (Exception e)
    {
        await _telegram.SendErrorAsync(DATE_NOW.ToString("yyyyMMddHHmmss") + "\n" + (e.InnerException?.Message ?? e.Message));
    }

    return TwoHoursCoinPrice;
}

async Task CreateTableIfExist(IAmazonDynamoDB client, string tableName)
{
    try
    {
        var res = await client.DescribeTableAsync(new DescribeTableRequest { TableName = tableName });
    }
    catch
    {
        await _telegram.SendErrorAsync("Create Table >> " + tableName);
        await CreateTable(client, tableName);
    }
}

static async Task<DescribeTableResponse> CreateTable(IAmazonDynamoDB client, string tableName)
{

    var response = await client.CreateTableAsync(new CreateTableRequest
    {
        TableName = tableName,
        AttributeDefinitions = new List<AttributeDefinition>()
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "CoinCode",
                            AttributeType = "S",
                        },
                        new AttributeDefinition
                        {
                            AttributeName = "DateString",
                            AttributeType = "S"
                        },
                    },
        KeySchema = new List<KeySchemaElement>()
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "CoinCode",
                            KeyType = "HASH"
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "DateString",
                            KeyType = "RANGE"
                        },
                    },
        ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10, WriteCapacityUnits = 5 },
    });

    var result = await WaitTillTableCreated(client, tableName, response);

    return result;
}

///// Must wait create table until status active(finish)
static async Task<DescribeTableResponse> WaitTillTableCreated(IAmazonDynamoDB client, string tableName, CreateTableResponse response)
{
    DescribeTableResponse resp = new DescribeTableResponse();
    var tableDescription = response.TableDescription;
    string status = tableDescription.TableStatus;
    int sleepDuration = 1000; // One second

    // Don't wait more than 10 seconds.
    while (status != "ACTIVE" && sleepDuration < 10000)
    {
        Thread.Sleep(sleepDuration);

        resp = await client.DescribeTableAsync(new DescribeTableRequest { TableName = tableName });

        status = resp.Table.TableStatus;
        sleepDuration *= 2;
    }

    return resp;
}

async Task<Nicehash_Price?> GetPriceNicehashAsync()
{
    try
    {
        HttpClient client = new();
        var resp = await client.GetAsync(NICEHASH_PRICE_URL).Result.Content.ReadAsStringAsync();
        var _data_price = JsonConvert.DeserializeObject<Nicehash_Price>(resp);

        return _data_price;
    }
    catch { throw; }
}

async Task<Indodax_Price?> GetPriceIndodaxAsync()
{
    try
    {
        HttpClient client = new();
        var resp = await client.GetAsync(INDODAX_PRICE_URL).Result.Content.ReadAsStringAsync();
        JObject jObject = JObject.Parse(resp);
        if (jObject["tickers"] == null) return null;
        var coin = new Indodax_Price();

        if (jObject["tickers"]?["aave_idr"]?["last"] != null) coin.aave_idr = (int)jObject["tickers"]?["aave_idr"]?["last"];
        if (jObject["tickers"]?["ada_idr"]?["last"] != null) coin.ada_idr = (int)jObject["tickers"]?["ada_idr"]?["last"];
        if (jObject["tickers"]?["btc_idr"]?["last"] != null) coin.btc_idr = (int)jObject["tickers"]?["btc_idr"]?["last"];
        if (jObject["tickers"]?["1inch_idr"]?["last"] != null) coin.inch_idr = (int)jObject["tickers"]?["1inch_idr"]?["last"];
        if (jObject["tickers"]?["ada_idr"]?["last"] != null) coin.ada_idr = (int)jObject["tickers"]?["ada_idr"]?["last"];
        if (jObject["tickers"]?["chz_idr"]?["last"] != null) coin.chz_idr = (int)jObject["tickers"]?["chz_idr"]?["last"];
        if (jObject["tickers"]?["crv_idr"]?["last"] != null) coin.crv_idr = (int)jObject["tickers"]?["crv_idr"]?["last"];
        if (jObject["tickers"]?["doge_idr"]?["last"] != null) coin.doge_idr = (int)jObject["tickers"]?["doge_idr"]?["last"];
        if (jObject["tickers"]?["eth_idr"]?["last"] != null) coin.eth_idr = (int)jObject["tickers"]?["eth_idr"]?["last"];
        if (jObject["tickers"]?["grt_idr"]?["last"] != null) coin.grt_idr = (int)jObject["tickers"]?["grt_idr"]?["last"];
        if (jObject["tickers"]?["hbar_idr"]?["last"] != null) coin.hbar_idr = (int)jObject["tickers"]?["hbar_idr"]?["last"];
        if (jObject["tickers"]?["lrc_idr"]?["last"] != null) coin.lrc_idr = (int)jObject["tickers"]?["lrc_idr"]?["last"];
        if (jObject["tickers"]?["ltc_idr"]?["last"] != null) coin.ltc_idr = (int)jObject["tickers"]?["ltc_idr"]?["last"];
        if (jObject["tickers"]?["rvn_idr"]?["last"] != null) coin.rvn_idr = (int)jObject["tickers"]?["rvn_idr"]?["last"];
        if (jObject["tickers"]?["sand_idr"]?["last"] != null) coin.sand_idr = (int)jObject["tickers"]?["sand_idr"]?["last"];
        if (jObject["tickers"]?["sushi_idr"]?["last"] != null) coin.sushi_idr = (int)jObject["tickers"]?["sushi_idr"]?["last"];
        if (jObject["tickers"]?["uni_idr"]?["last"] != null) coin.uni_idr = (int)jObject["tickers"]?["uni_idr"]?["last"];
        if (jObject["tickers"]?["xrp_idr"]?["last"] != null) coin.xrp_idr = (int)jObject["tickers"]?["xrp_idr"]?["last"];

        return coin;
    }
    catch { throw; }
}