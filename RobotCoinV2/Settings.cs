namespace RobotCoinV2
{
    class Settings
    {
        //EnvironmentVariableTarget dihilangkan kalau deploy di linux

        public string? TELEGRAM_TOKEN_BOT = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN_BOT");
        public string? TELEGRAM_CHATID_ERROR = Environment.GetEnvironmentVariable("TELEGRAM_CHATID_ERROR");
        public string? TELEGRAM_CHATID_STATUS = Environment.GetEnvironmentVariable("TELEGRAM_CHATID_STATUS");
        public string? TELEGRAM_CHATID_INFO = Environment.GetEnvironmentVariable("TELEGRAM_CHATID_INFO");

        public string? INDODAX_PRICE_URL = Environment.GetEnvironmentVariable("INDODAX_PRICE_URL");
        public string? NICEHASH_PRICE_URL = Environment.GetEnvironmentVariable("NICEHASH_PRICE_URL");

        public string? AWS_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
        public string? AWS_SECRET_KEY = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");

        public int GAP_NAIK = int.Parse(Environment.GetEnvironmentVariable("GAP_NAIK") ?? "5");
        public int GAP_TURUN = int.Parse(Environment.GetEnvironmentVariable("GAP_TURUN") ?? "5") * -1;
        public int LAST_HOUR = int.Parse(Environment.GetEnvironmentVariable("LAST_HOUR") ?? "5") * -1;

        static string? LIST_COIN_NOTIF = Environment.GetEnvironmentVariable("LIST_COIN_NOTIF");
        static string? FAVORITE_COIN_LIST = Environment.GetEnvironmentVariable("FAVORITE_COIN_LIST");

        public List<string>? FavoriteCoinList;
        public List<string>? ListCoinNotif;

        public Settings()
        {
            //System.Environment.SetEnvironmentVariable("name", "val", EnvironmentVariableTarget.User);
            FavoriteCoinList = new();
            ListCoinNotif = new();

            if (FAVORITE_COIN_LIST == null)
                FavoriteCoinList = new() { "AAVE", "ADA", "BTC", "CHZ", "CRV", "DOGE", "ETH", "GRT", "HBAR", "LRC", "LTC", "INCH", "RVN", "SAND", "SUSHI", "UNI", "XRP", };
            else
            {
                foreach (var item in FAVORITE_COIN_LIST.Split(",").ToList())
                    if (!string.IsNullOrEmpty(item.Trim())) FavoriteCoinList.Add(item.Trim().ToUpper());
            }

            if (LIST_COIN_NOTIF == null)
                foreach (var item in FavoriteCoinList) ListCoinNotif.Add(item);
            else
                foreach (var item in LIST_COIN_NOTIF.Split(",").ToList())
                    if (!string.IsNullOrEmpty(item.Trim())) FavoriteCoinList.Add(item.Trim().ToUpper());
        }
    }
}
