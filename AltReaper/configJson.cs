
namespace Reaper
{
    internal class configJson
    {
        public class root
        {
            public string apiKey { get; set; }
            public string senderMail { get; set; }

            public string senderMailPassword { get; set; }
            public string hostDomain { get; set; }
            public int portNumber { get; set; }
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
}
