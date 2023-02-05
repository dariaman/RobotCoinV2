using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCoinV2
{
    class Settings
    {
        //EnvironmentVariableTarget dihilangkan kalau deploy di linux
        public string? TELEGRAM_TOKEN_BOT = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN_BOT", EnvironmentVariableTarget.User);
        public string? TELEGRAM_CHATID_ERROR = Environment.GetEnvironmentVariable("TELEGRAM_CHATID_ERROR", EnvironmentVariableTarget.User);
        public string? TELEGRAM_CHATID_STATUS = Environment.GetEnvironmentVariable("TELEGRAM_CHATID_STATUS", EnvironmentVariableTarget.User);
        public string? TELEGRAM_CHATID_INFO = Environment.GetEnvironmentVariable("TELEGRAM_CHATID_INFO", EnvironmentVariableTarget.User);

        public string? INDODAX_PRICE_URL = Environment.GetEnvironmentVariable("INDODAX_PRICE_URL", EnvironmentVariableTarget.User);
        public string? NICEHASH_PRICE_URL = Environment.GetEnvironmentVariable("NICEHASH_PRICE_URL", EnvironmentVariableTarget.User);

        public string? AWS_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY", EnvironmentVariableTarget.User);
        public string? AWS_SECRET_KEY = Environment.GetEnvironmentVariable("AWS_SECRET_KEY", EnvironmentVariableTarget.User);

        public int GAP_NAIK = int.Parse(Environment.GetEnvironmentVariable("GAP_NAIK", EnvironmentVariableTarget.User) ?? "5");
        public int GAP_TURUN = int.Parse(Environment.GetEnvironmentVariable("GAP_TURUN", EnvironmentVariableTarget.User) ?? "5") * -1;
        public int LAST_HOUR = int.Parse(Environment.GetEnvironmentVariable("LAST_HOUR", EnvironmentVariableTarget.User) ?? "5") * -1;

        public List<string> FavoriteCoinList = new()
        {
            "AAVE",
            "ADA",
            "BTC",
            "CHZ",
            "CRV",
            "DOGE",
            "ETH",
            "GRT",
            "HBAR",
            "LRC",
            "LTC",
            "INCH",
            "RVN",
            "SAND",
            "SUSHI",
            "UNI",
            "XRP",
        };
    }
}
