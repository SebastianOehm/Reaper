﻿using System.Net;
using System.Net.Http.Json;
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
        public static string UnitPreference(JsonHandling.langVal langValue)
        {
            //gets unit preference
            string unitPreference = "";
            while (true)
            {
                Console.Write($"\n{langValue.unitQuery} ({langValue.metric},{langValue.imperial})\n>");
                unitPreference = Console.ReadLine().ToLower();
                if (unitPreference == langValue.metric ^ unitPreference == langValue.imperial)
                {
                    if (unitPreference == langValue.metric) { unitPreference = "metric"; }
                    if (unitPreference == langValue.imperial) { unitPreference = "imperial"; }
                    break;
                }
                Console.WriteLine(langValue.invalidInput);
                Console.WriteLine(langValue.pressEnterContinue);
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            }
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
                string apiKey = Console.ReadLine();
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
            Console.WriteLine($"\n{spacer}\n");
            Console.WriteLine($"using {selectedLanguage}");
            Console.WriteLine($"\n{spacer}");
            return selectedLanguage;
        }
    }
}