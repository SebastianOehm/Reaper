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
        public static async Task<WeatherResponse.root> APICall(String city, String langPreferenceShort, String unitPreference, String APIKey)
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

            var jsonResponse = await client.GetFromJsonAsync<WeatherResponse.root>($"weather?q={city}&lang={langPreferenceShort}&units={unitPreference}&appid={APIKey}");
            return jsonResponse;
        }

        public static bool configGen (String cfgLoc, bool status, JsonHandling.langVal langValue, JsonHandling.config config)
        {
            //checks if config file exists and if all lines have information
            if (File.Exists(cfgLoc) && Checks.cfgChecker(config) == false) { return true; }

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
            Write($"\nEnter the mail address of the BCC archive mail or type \"{langValue.no}\" (without quotes) \n>");
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
            string configRaw = JsonSerializer.Serialize(json);
            File.WriteAllText(cfgLoc, configRaw);
            return true;
        }
        public static string UnitPreference(JsonHandling.langVal langValue)
        {
            //gets unit preference
            string[] unitOptions = { langValue.metric, langValue.imperial };
            Menu unitMenu = new(langValue.unitQuery, unitOptions);
            int unitChoice = unitMenu.IRExcecute();
            string unitPreference = unitChoice == 0 ? "metric" : "imperial";
            return unitPreference;
        }
        public static void ConfigGetter(string cfgLoc)
        {
            JsonHandling.config test = null;
            try { test = JsonSerializer.Deserialize<JsonHandling.config>(File.ReadAllText(cfgLoc)); }
            catch { File.Delete(cfgLoc); }
            if (!File.Exists(cfgLoc))
            {
                Console.Write("\nEnter your APIKey\n>");
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
                string tmpConfig = JsonSerializer.Serialize(tmp);
                File.WriteAllText(cfgLoc, tmpConfig);
            }
        }
        public static String langPreference(String[] fullySupportedLanguages, String tree)
        {
            List<string> availableLanguages = new();
            foreach (string l in fullySupportedLanguages)
            {
                if (File.Exists($"{tree}{l}Text.json"))
                {
                    availableLanguages.Add($"{char.ToUpper(l[0]) + l.Substring(1)}");
                }
            }


            string languagePrompt = "Please select your desired language or \"new\" to implement a new language";
            string[] languageOptions = availableLanguages.ToArray();
            Menu languageMenu = new Menu(languagePrompt, languageOptions);
            string spacer = "-------------------------";
            string selectedLanguage = languageMenu.SRExcecute();
            WriteLine($"\n{spacer}\n");
            WriteLine($"using {selectedLanguage}");
            WriteLine($"\n{spacer}");
            return selectedLanguage;
        }
    }
}
