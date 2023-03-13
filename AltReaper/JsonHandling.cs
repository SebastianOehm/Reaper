
namespace Reaper
{
    internal static class JsonHandling
    {
        public class config
        {
            public string apiKey { get; set; }
            public string senderMail { get; set; }

            public string senderMailPassword { get; set; }
            public string hostDomain { get; set; }
            public string portNumber { get; set; }
            public string bcc { get; set; }
        }
        public class langVal
        {
            public string shortLanguage { get; set; }
            public string unitQuery { get; set; }
            public string metric { get; set; }
            public string imperial { get; set; }
            public string nameOfCity { get; set; }
            public string invalidInput { get; set; }
            public string pressEnterContinue { get; set; }
            public string errorMessage { get; set; }
            public string theWeatherIn { get; set; }
            public string temp { get; set; }
            public string lowestTemp { get; set; }
            public string highestTemp { get; set; }
            public string description { get; set; }
            public string localSystemTime { get; set; }
            public string timeAtDestination { get; set; }
            public string yourWeatherInfo { get; set; }
            public string yes { get; set; }
            public string no { get; set; }
            public string mailWanted { get; set; }
            public string mailAddressQuery { get; set; }
            public string nameOr { get; set; }
            public string mailSuccessMessage { get; set; }
        }

    }
    public static class WeatherResponse
    {
        public class weather
        {
            public string description { get; set; }
            public string main { get; set; }
            public string icon { get; set; }
        }
        public class main
        {
            public double temp { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public double feels_like { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
        }
        public class sys
        {
            public long sunrise { get; set; }
            public long sunset { get; set; }
            public string country { get; set; }
        }
        public class wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
        }

        public class root
        {
            public List<weather> weather { get; set; }
            public main main { get; set; }
            public sys sys { get; set; }
            public wind wind { get; set; }
            public long dt { get; set; }
            public int timezone { get; set; }
            public string name { get; set; }
        }
    }
}