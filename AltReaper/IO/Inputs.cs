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
        public static async Task<WeatherResponse.Root> APICall(string city, string langPreferenceShort, string unitPreference, string APIKey)
        {
            //Use default system proxy settings
            IWebProxy? defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy?.Credentials = CredentialCache.DefaultCredentials;
            HttpClientHandler handler = new()
            {
                Proxy = defaultWebProxy,
            };
            HttpClient client = new(handler)
            {
                BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/"),
            };

            return await client.GetFromJsonAsync<WeatherResponse.Root>($"weather?q={city}&lang={langPreferenceShort}&units={unitPreference}&appid={APIKey}")
                ?? throw new InvalidOperationException("The weather API returned an empty response.");
        }

        public static bool ConfigGen(JsonHandling.Config config)
        {
            //checks if config file exists and if all lines have information
            if (File.Exists(GlobalVars.cfgLoc) && Checks.CfgChecker(config) == false) { return true; }

            string apiKey = config.ApiKey;
            Write("\nEnter the mail address which you want use to send mails\n>");
            CursorVisible = true;
            ForegroundColor = ConsoleColor.White;
            string senderMail = ReadLine() ?? "";
            ForegroundColor = ConsoleColor.Green;
            Write("\nEnter the password for the mail (won't be shown) letter by letter, then press enter\n>");
            string senderMailPassword = Helper.PasswordMaker();
            Write("\nEnter the smtp host domain\"\n>");
            ForegroundColor = ConsoleColor.White;
            string hostDomain = ReadLine() ?? "";
            ForegroundColor = ConsoleColor.Green;
            Write("\nEnter the smtp port Number\n>");
            ForegroundColor = ConsoleColor.White;
            string portNumber = ReadLine() ?? "";
            ForegroundColor = ConsoleColor.Green;
            //empty bcc means no BCC mail
            string BCC = "";
            string[] bccOptions = [Properties.Resources.YesOption, Properties.Resources.NoOption];
            Menu bccMenu = new(Properties.Resources.bccWanted, bccOptions);
            if (bccMenu.IRExecute() == 0)
            {
                Write("\nEnter the mail address of the BCC archive mail\n>");
                CursorVisible = true;
                ForegroundColor = ConsoleColor.White;
                BCC = ReadLine() ?? "";
                ForegroundColor = ConsoleColor.Green;
            }
            CursorVisible = false;

            var json = new JsonHandling.Config
            {
                ApiKey = apiKey,
                SenderMail = senderMail,
                SenderMailPassword = senderMailPassword,
                HostDomain = hostDomain,
                PortNumber = portNumber,
                Bcc = BCC
            };
            File.WriteAllText(GlobalVars.cfgLoc, JsonSerializer.Serialize(json));
            return true;
        }
        public static string UnitPreference()
        {
            //gets unit preference
            string[] unitOptions = [Properties.Resources.metric, Properties.Resources.imperial];
            Menu unitMenu = new(Properties.Resources.unitQuery, unitOptions);
            return unitMenu.IRExecute() == 0 ? "metric" : "imperial";
        }
        public static void ConfigGetter()
        {
            try
            {
                _ = JsonSerializer.Deserialize<JsonHandling.Config>(File.ReadAllText(GlobalVars.cfgLoc));
            }
            catch { File.Delete(GlobalVars.cfgLoc); }
            if (!File.Exists(GlobalVars.cfgLoc))
            {
                Write("\nEnter your APIKey\n>");
                string apiKey = ReadLine() ?? "";
                var tmp = new JsonHandling.Config
                {
                    ApiKey = apiKey,
                    SenderMail = "",
                    SenderMailPassword = "",
                    HostDomain = "",
                    PortNumber = "",
                    Bcc = ""
                };
                File.WriteAllText(GlobalVars.cfgLoc, JsonSerializer.Serialize(tmp));
            }
        }
        public static string LangPreference()
        {
            List<string> availableLanguages = [.. GlobalVars.appLanguages];

            Menu languageMenu = new(Properties.Resources.SelectPreferredLanguage, [.. availableLanguages]);
            string spacer = "-------------------------";
            string selectedLanguage = languageMenu.SRExecute();
            WriteLine($"\n{spacer}\n");
            WriteLine($"using {selectedLanguage}");
            WriteLine($"\n{spacer}");
            return selectedLanguage;
        }
    }
}
