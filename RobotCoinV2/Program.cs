using Amazon;
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

var ListCoinNotif = setttings.ListCoinNotif;
setttings = null;

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

var _coinPrice = await GetCoinPriceAsync();

///* Insert Data Coin
var last_data_coin = await GetLastTimeCoinPriceAsync(client_db);

try
{
    if (_coinPrice != null) await InsertPriceCoinAsync(client_db, _coinPrice);
}
catch (Exception ex)
{
    await _telegram.SendErrorAsync(DATE_NOW.ToString("yyyyMMddHHmmss") + "\nError Insert Price\n" + (ex.InnerException?.Message ?? ex.Message));
}

_coinPrice = null;

//End Insert Data Coin  */

///* Baca Data Coin 
try
{
    if (last_data_coin.Count > 0)
    {
        Decimal percentGapBTC;
        Decimal percentGapUSDT;
        Decimal percentGapIDR;
        string pesan = "";
        if (ListCoinNotif != null)
            foreach (var _coin in ListCoinNotif)
            {
                if (string.IsNullOrEmpty(_coin)) continue;

                percentGapBTC = 0.0M;
                percentGapUSDT = 0.0M;
                percentGapIDR = 0.0M;
                var currentPrice = last_data_coin.Where(x => x.CoinCode == _coin).OrderByDescending(x => x.DateString).FirstOrDefault();
                var lastPrice = last_data_coin.Where(x => x.CoinCode == _coin).OrderBy(x => x.DateString).FirstOrDefault();

                if (currentPrice?.BTC > 0 && lastPrice?.BTC > 0) percentGapIDR = ((currentPrice.BTC - lastPrice.BTC) / lastPrice.BTC) * 100;
                if (currentPrice?.USDT > 0 && lastPrice?.USDT > 0) percentGapUSDT = ((currentPrice.USDT - lastPrice.USDT) / lastPrice.USDT) * 100;
                if (currentPrice?.IDR > 0 && lastPrice?.IDR > 0) percentGapIDR = ((currentPrice.IDR - lastPrice.IDR) / (decimal)lastPrice.IDR) * 100;

                if (percentGapBTC <= GAP_TURUN || percentGapBTC >= GAP_NAIK || percentGapUSDT <= GAP_TURUN || percentGapUSDT >= GAP_NAIK || percentGapIDR <= GAP_TURUN || percentGapIDR >= GAP_NAIK)
                    pesan += $"{(string.IsNullOrEmpty(pesan) ? "" : "\n\n")}<b>{_coin}</b> curs ";

                if (percentGapBTC <= GAP_TURUN || percentGapBTC >= GAP_NAIK)
                    pesan += $"\n<b>{(percentGapBTC < 0 ? "<u>" : "")}BTC={percentGapBTC.ToString()[..Math.Min(5, percentGapBTC.ToString().Length)]}% {(percentGapBTC < 0 ? "</u>" : "")}</b>\n" +
                        $"currPrice={currentPrice?.BTC} \nlast <b>({Math.Abs(LAST_HOUR)})</b> hours Price={lastPrice?.BTC}";

                if (percentGapUSDT <= GAP_TURUN || percentGapUSDT >= GAP_NAIK)
                    pesan += $"\n<b>{(percentGapUSDT < 0 ? "<u>" : "")}USDT={percentGapUSDT.ToString()[..Math.Min(5, percentGapUSDT.ToString().Length)]}% {(percentGapUSDT < 0 ? "</u>" : "")}</b>\n" +
                        $"currPrice={currentPrice?.USDT} \nlast <b>({Math.Abs(LAST_HOUR)})</b> hours Price={lastPrice?.USDT}";

                if (percentGapIDR <= GAP_TURUN || percentGapIDR >= GAP_NAIK)
                    pesan += $"\n<b>{(percentGapIDR < 0 ? "<u>" : "")}IDR={percentGapIDR.ToString()[..Math.Min(5, percentGapIDR.ToString().Length)]}% {(percentGapIDR < 0 ? "</u>" : "")}</b>\n" +
                        $"currPrice={currentPrice?.IDR:n0} \nlast <b>({Math.Abs(LAST_HOUR)})</b> hours Price={lastPrice?.IDR:n0}";
            }
        if (!string.IsNullOrEmpty(pesan)) await _telegram.SendMessageAsync(pesan);
    }
}
catch (Exception ex)
{
    await _telegram.SendErrorAsync(DATE_NOW.ToString("yyyyMMddHHmmss") + "\nError Baca Data\n" + (ex.InnerException?.Message ?? ex.Message));
}
/* End Baca Data Coin */
await _telegram.SendStatusAsync("Finish =>" + DATE_NOW.ToString("dd MMM yyyy HH:mm:ss") + $" >> " + DATE_NOW.ToString("yyyyMMddHHmmss"));

async Task InsertPriceCoinAsync(IAmazonDynamoDB client, List<CoinPrice> listCoinPrice)
{
    string TableName = "CoinPrice";
    await CreateTableIfExist(client, TableName);

    //var last_data_coin = await GetLast2HoursCoinPriceAsync(client);

    DynamoDBContext context = new(client);
    var coinBatch = context.CreateBatchWrite<CoinPrice>();
    List<CoinPrice> savelistCoinPrice = new();
    ///// write data to table
    try
    {
        foreach (CoinPrice xcoinPrice in listCoinPrice)
        {
            var lastPrice = last_data_coin?.Where(x => x.CoinCode == xcoinPrice.CoinCode).OrderByDescending(x => x.DateString).FirstOrDefault();
            if (lastPrice == null) savelistCoinPrice.Add(xcoinPrice);
            else
                if (!(lastPrice.USDT == xcoinPrice.BTC && lastPrice.USDT == xcoinPrice.USDT && lastPrice.IDR == xcoinPrice.IDR)) savelistCoinPrice.Add(xcoinPrice);
        }

        coinBatch.AddPutItems(savelistCoinPrice);
        await coinBatch.ExecuteAsync();

    }
    catch (Exception ex)
    {
        await _telegram.SendErrorAsync("Tgl =>" + DATE_NOW.ToString("dd MMM yyyy HH:mm:ss") + "\n" + (ex.InnerException?.Message ?? ex.Message));
    }
}

async Task<List<CoinPrice>> GetLastTimeCoinPriceAsync(IAmazonDynamoDB client)
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

async Task<List<CoinPrice>?> GetCoinPriceAsync()
{
    try
    {
        HttpClient client = new();
        // Ambil Price dari nicehash
        var resp = await client.GetAsync(NICEHASH_PRICE_URL).Result.Content.ReadAsStringAsync();
        var _data_price2 = JsonConvert.DeserializeObject(resp);

        // Ambil Price dari Indodax
        resp = await client.GetAsync(INDODAX_PRICE_URL).Result.Content.ReadAsStringAsync();
        JObject jObject = JObject.Parse(resp);
        if (jObject["tickers"] == null) return null;

        var coinList = new Settings();
        var listCoinPrice = new List<CoinPrice>();
        foreach (var item in coinList.FavoriteCoinList)
        {
            CoinPrice coinPrice = new()
            {
                CoinCode = item.ToUpper(),
                DateString = DATE_NOW.ToString("yyyyMMddHHmmss")
            };

            // ambil nilai USDT dari data nicehash
            decimal? _temp;
            if (item.ToUpper() == "INCH")
                _temp = (decimal?)((JProperty)((JContainer)_data_price2).Where(x => x.Path == "ONEINCHUSDT").FirstOrDefault())?.Value;
            else
                _temp = (decimal?)((JProperty)((JContainer)_data_price2).Where(x => x.Path == item.ToUpper() + "USDT").FirstOrDefault())?.Value;

            if (_temp != null) coinPrice.USDT = _temp ?? 0;

            // ambil nilai BTC dari data nicehash
            if (item.ToUpper() == "INCH")
                _temp = (decimal?)((JProperty)((JContainer)_data_price2).Where(x => x.Path == "ONEINCHBTC").FirstOrDefault())?.Value;
            else
                _temp = (decimal?)((JProperty)((JContainer)_data_price2).Where(x => x.Path == item.ToUpper() + "BTC").FirstOrDefault())?.Value;

            if (_temp != null) coinPrice.BTC = _temp ?? 0;

            // ambil nilai IDR dari data indodax
            if (jObject["tickers"]?[$"{item.ToLower()}_idr"]?["last"] != null)
            {
                var _temp2 = (int?)jObject["tickers"]?[$"{item.ToLower()}_idr"]?["last"];
                if (_temp2 != null) coinPrice.IDR = _temp2 ?? 0;
            }

            if (coinPrice.BTC > 0 || coinPrice.IDR > 0 || coinPrice.USDT > 0) listCoinPrice.Add(coinPrice);
        }

        return listCoinPrice;
    }
    catch { throw; }
}

