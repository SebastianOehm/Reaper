using System.Text.Json;
using static Reaper.configJson;

/* LineByLineTranslationFileInstructions | W = Word | number=line index
  
0       ShortLanguageCode (not a query)
1       Enter unit preference
2   W   metric
3   W   imperial
4       Enter name of place for which you want weather data
5       invalidInput
6       pressEnterContinue
7       errorFound
8       theWeatherIn
9   W   temperature
10  W   lowestTemperature
11  W   highestTemperature
12  W   description
13      LocalSystemTime
14      Time at destination
15      Your weather Info (formal)
16  W   yes
17  W   no
18      mailWanted
19      mailAddressQuery
20      nameOr
21      mailSuccessMessage
 
*/

namespace Reaper
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            //Set window title to Reaper.versionName
            string appName = "Reaper", devName = "WetterSenseDev";
            string[] devData = { appName, devName };
            string versionNumber = "0.8.2";
            string title = $"{appName} v{versionNumber}";
            Console.Title = title;

            //generate default directory structure and langFile
            string baseLoc = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper";
            string tree = $"{baseLoc}\\langFiles\\";
            string cfgLoc = $"{baseLoc}\\config.json";
            if (!Directory.Exists(tree)) { Directory.CreateDirectory(tree); }
            Console.Write("\nDo you want to enter supervisor mode? (yes, no)\n>");
            string a = Console.ReadLine();
            if (a == "yes")
            {
                Console.Write("\nEnter the supervisor password. Password won't be shown, type each char individually, backspace to correct, enter to continue\n>");
                string supervisorPwd = Helper.PasswordMaker();
                Helper.SupervisorMode(supervisorPwd, appName, baseLoc);
            }
            else { Console.Write("Continuing in standard mode"); }
            TranslationMaker.defaultFileMaker();

            //select language or implement new language
            //string[] langValue = null;
            Console.Write("\nPlease either input your desired language or input \"new\" to implement another one. (without quotes)\n>");
            string langPreferenceLong = Console.ReadLine().ToLower();
            configJson.langVal langValue = null;
            if (langPreferenceLong == "new")
            {
                Console.WriteLine("Please input the name of the language you want to implement");
                string langName = Console.ReadLine();
                if (TranslationMaker.newFileMaker(langName) == true)
                {
                    //langValue = Inputs.langHandler(langName);
                    langValue = JsonSerializer.Deserialize<configJson.langVal>(Inputs.langHandler(langName));
                }
            }
            else
            {
                try { langValue = JsonSerializer.Deserialize<configJson.langVal>(Inputs.langHandler(langPreferenceLong)); }
                catch
                {
                    //resort to default on fail
                    langValue = JsonSerializer.Deserialize<configJson.langVal>(Inputs.langHandler("default"));
                        
                    Console.WriteLine($"{langValue.invalidInput} Using default language (English)");
                }
            }
            string langPreferenceShort = langValue.shortLanguage;

            //cfg getter
            configJson.root test = null;
            try { test = JsonSerializer.Deserialize<configJson.root>(File.ReadAllText(cfgLoc)); }
            catch { File.Delete(cfgLoc); }
            test = null;
            if (!File.Exists(cfgLoc))
            {
                Console.Write("\nEnter your APIKey\n>");
                string apiKey = Console.ReadLine();
                var tmp = new configJson.root
                {
                    apiKey = apiKey,
                    senderMail = "",
                    senderMailPassword = "",
                    hostDomain = "",
                    portNumber = "",
                    bcc = ""
                };
                string v = JsonSerializer.Serialize(tmp);
                File.WriteAllText(cfgLoc, v);
            }
            configJson.root config = JsonSerializer.Deserialize<configJson.root>(File.ReadAllText(cfgLoc));
            
            //gets unit preference
            string unitPreference = null;
            bool suc = false;
            while (suc != true)
            {
                Console.Write($"\n{langValue.unitQuery} ({langValue.metric},{langValue.imperial})\n>");
                unitPreference = Console.ReadLine().ToLower();
                if (unitPreference == langValue.metric ^ unitPreference == langValue.imperial)
                { 
                    break;
                }
                Console.WriteLine(langValue.invalidInput);
                Console.WriteLine(langValue.pressEnterContinue);
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            }

            //Build data for API call
            Console.Write($"\n{langValue.nameOfCity}\n>");
            string city = null;
            while (String.IsNullOrEmpty(city)){ city = Console.ReadLine(); }

            //make API call
            string json = Inputs.APICall(city, langPreferenceShort, unitPreference, config.apiKey);

            //deserialize Json response
            JsonResponseDeserializer.root wetterDaten = JsonSerializer.Deserialize<JsonResponseDeserializer.root>(json);
            string[] content = Outputs.WeatherOutput(wetterDaten, unitPreference, langValue);

            //mail option
            Helper.MailOption(langValue, config, content, cfgLoc, devData);
        }
    }
}