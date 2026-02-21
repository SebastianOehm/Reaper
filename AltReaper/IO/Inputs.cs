using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Console;

/*
 * config structure
 * APIKey
 * senderAddress
 * password
 * host
 * port
 * bcc
*/
namespace Reaper.IO
{
    internal class Inputs
    {
        public static string configReader()
        {
            return File.ReadAllText(globalVars.cfgLoc);
        }

        public static async Task<WeatherResponse.root> APICall(string city, string langPreferenceShort, string unitPreference, string APIKey)
        {
            //Use default system proxy settings
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            HttpClientHandler handler = new()
            {
                Proxy = defaultWebProxy,
            };
            HttpClient client = new()
            {
                BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/"),
            };

            return await client.GetFromJsonAsync<WeatherResponse.root>($"weather?q={city}&lang={langPreferenceShort}&units={unitPreference}&appid={APIKey}");
        }

        public static bool configGen(JsonHandling.config config)
        {
            //checks if config file exists and if all lines have information
            if (File.Exists(globalVars.cfgLoc) && Checks.cfgChecker(config) == false) { return true; }

            string apiKey = config.apiKey;
            Write("\nEnter the mail address which you want use to send mails\n>");
            CursorVisible = true;
            ForegroundColor = ConsoleColor.White;
            string senderMail = ReadLine();
            ForegroundColor = ConsoleColor.Green;
            Write("\nEnter the password for the mail (won't be shown) letter by letter, then press enter\n>");
            string senderMailPassword = Helper.PasswordMaker();
            Write("\nEnter the smtp host domain\"\n>");
            ForegroundColor = ConsoleColor.White;
            string hostDomain = ReadLine();
            ForegroundColor = ConsoleColor.Green;
            Write("\nEnter the smtp port Number\n>");
            ForegroundColor = ConsoleColor.White;
            string portNumber = ReadLine();
            ForegroundColor = ConsoleColor.Green;
            Write($"\nEnter the mail address of the BCC archive mail or type \"{Properties.Resources.NoOption}\" (without quotes) \n>");
            ForegroundColor = ConsoleColor.White;
            string BCC = ReadLine();
            CursorVisible = false;

            var json = new JsonHandling.config
            {
                apiKey = apiKey,
                senderMail = senderMail,
                senderMailPassword = senderMailPassword,
                hostDomain = hostDomain,
                portNumber = portNumber,
                bcc = BCC
            };
            File.WriteAllText(globalVars.cfgLoc, JsonSerializer.Serialize(json));
            return true;
        }
        public static string UnitPreference()
        {
            //gets unit preference
            string[] unitOptions = { Properties.Resources.metric, Properties.Resources.imperial };
            Menu unitMenu = new(Properties.Resources.unitQuery, unitOptions);
            return unitMenu.IRExecute() == 0 ? "metric" : "imperial";
        }
        public static void ConfigGetter()
        {
            JsonHandling.config test = null;
            try { test = JsonSerializer.Deserialize<JsonHandling.config>(File.ReadAllText(globalVars.cfgLoc)); }
            catch { File.Delete(globalVars.cfgLoc); }
            if (!File.Exists(globalVars.cfgLoc))
            {
                Write("\nEnter your APIKey\n>");
                string apiKey = ReadLine();
                var tmp = new JsonHandling.config
                {
                    apiKey = apiKey,
                    senderMail = "",
                    senderMailPassword = "",
                    hostDomain = "",
                    portNumber = "",
                    bcc = ""
                };
                File.WriteAllText(globalVars.cfgLoc, JsonSerializer.Serialize(tmp));
            }
        }
        public static string langPreference()
        {
            List<string> availableLanguages = new();
            foreach (string l in globalVars.appLanguages)
            {
                availableLanguages.Add(l);
            }

            Menu languageMenu = new(Properties.Resources.SelectPreferredLanguage, availableLanguages.ToArray());
            string spacer = "-------------------------";
            string selectedLanguage = languageMenu.SRExecute();
            WriteLine($"\n{spacer}\n");
            WriteLine($"using {selectedLanguage}");
            WriteLine($"\n{spacer}");
            return selectedLanguage;
        }
    }
}
