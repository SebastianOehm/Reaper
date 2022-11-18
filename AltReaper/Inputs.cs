using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static bool configGen (String cfgLoc, int cfgOptCount, String[] langValue, String[] config)
        {
            if (File.Exists(cfgLoc) && File.ReadAllLines(cfgLoc).Length == cfgOptCount)
            {
                return false;
            }

            string apiKey = config[0];
            Console.Write("\nEnter the mail address which you want use to send mails\n>");
            string senderMail = Console.ReadLine();
            Console.Write("\nEnter the password for the mail (won't be shown) letter by letter, then press enter\n>");
            string senderMailPassword = null;
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                break;
                senderMailPassword += key.KeyChar;
            }
            Console.Write("\nEnter the smtp host domain\"\n>");
            string hostDomain = Console.ReadLine();
            Console.Write("\nEnter the smtp port Number\n>");
            string portNumber = Console.ReadLine();
            Console.Write($"\nEnter the mail address of the BCC archive mail or type \"{langValue[17]}\" (without quotes) \n>");
            string BCC = Console.ReadLine();

            string[] cfgData = {apiKey,senderMail,senderMailPassword,hostDomain,portNumber,BCC};
            File.WriteAllLines(cfgLoc, cfgData);

            return true;
        }
    }
}
