using System.Net;
using System.Text.Json;

/*
 * config structure
 * APIKey
 * senderAddress
 * password
 * host
 * port
 * bcc
*/
namespace Reaper
{
    internal class Inputs
    {
        public static String configReader (String cfgLoc) 
        {
            string config = File.ReadAllText(cfgLoc);
            return config;
        }
        public static String langHandler(String langPreferenceLong)
        {
            //importing selected language file
            string langValue = File.ReadAllText($"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper\\langFiles\\{langPreferenceLong}Text.json");
            return langValue;
        }
        public static String APICall(String city, String langPreferenceShort, String unitPreference, String APIKey)
        {
            //Use default system proxy settings
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            WebClient client = new WebClient { Proxy = defaultWebProxy };

            //build API Call url
            string key = APIKey;
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&lang={langPreferenceShort}&units={unitPreference}&appid={key}";

            // GET
            string json = client.DownloadString(url);
            return json;
        }

        public static bool configGen (String cfgLoc, bool status, JsonHandling.langVal langValue, JsonHandling.config config)
        {
            //checks if config file exists and if all lines have information
            if (File.Exists(cfgLoc) && Helper.cfgChecker(config) == false) { return true; }

            string apiKey = config.apiKey;
            Console.Write("\nEnter the mail address which you want use to send mails\n>");
            string senderMail = Console.ReadLine();
            Console.Write("\nEnter the password for the mail (won't be shown) letter by letter, then press enter\n>");
            string senderMailPassword = Helper.PasswordMaker();
            Console.Write("\nEnter the smtp host domain\"\n>");
            string hostDomain = Console.ReadLine(); 
            Console.Write("\nEnter the smtp port Number\n>");
            string portNumber = Console.ReadLine();
            Console.Write($"\nEnter the mail address of the BCC archive mail or type \"{langValue.no}\" (without quotes) \n>");
            string BCC = Console.ReadLine();

            var json = new JsonHandling.config
            {
                apiKey = apiKey,
                senderMail = senderMail,
                senderMailPassword = senderMailPassword,
                hostDomain = hostDomain,
                portNumber = portNumber,
                bcc = BCC
            };
            string configRaw = JsonSerializer.Serialize(json);
            File.WriteAllText(cfgLoc, configRaw);
            return true;
        }
    }
}
