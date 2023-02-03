using Amazon.DynamoDBv2.DataModel;

namespace RobotCoinV2
{
    [DynamoDBTable("CoinPrice")]
    internal class CoinPrice
    {
        [DynamoDBHashKey]
        public string? CoinCode { get; set; }
        [DynamoDBRangeKey]
        public string? DateString { get; set; }
        public decimal USDT { get; set; } = 0;
        public decimal BTC { get; set; } = 0;
        public int IDR { get; set; } = 0;
    }
}
