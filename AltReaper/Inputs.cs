using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

/*cfg 
 * APIKey
 * senderAddress
 * password
 * host
 * port
*/
namespace Reaper
{
    internal class Inputs
    {
        public static string[] configReader (string cfgLoc) 
        {
            string[] config = File.ReadAllLines(cfgLoc);
            return config;
        }
        public static String[] langHandler(String langPreferenceLong)
        {
            //importing selected language file
            string[] langValue = File.ReadAllLines($"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper\\langFiles\\{langPreferenceLong}Text.txt");
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
    }
}
