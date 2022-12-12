using System.Runtime.CompilerServices;
using System.Text.Json;
/* LineByLineTranslationFileInstructions | W = Word | number=line index
0       ShortLanguageCode (not a query)
1       Enter   preference
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
            string appName = "Reaper", devName = "WetterSenseDev", versionNumber = "0.9";
            string[] devData = { appName, devName };
            Console.Title = $"{appName} v{versionNumber}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();

            if (Checks.DeviceIsOnline() == true) { Console.WriteLine("Device is online"); }
            else
            {
                Console.WriteLine(@"      
Your device is not connected to the internet.
This application needs internet access.
Please connect your device to the internet to use this application.");
                Helper.Closer(devData);
            }
            if (Checks.APIsOnline() == true) { Console.WriteLine("API is online"); }
            else { Console.WriteLine("API is not online. Please try again later."); Helper.Closer(devData); }

            //generate default directory structure and langFile
            string baseLoc = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper";
            string tree = $"{baseLoc}\\langFiles\\", cfgLoc = $"{baseLoc}\\config.json";
            if (!Directory.Exists(tree)) { Directory.CreateDirectory(tree); }

            // ask if user wants to use superuser mode
            Console.Write("\nDo you want to enter superuser mode? (yes, no)\n>");
            if (Console.ReadLine() == "yes")
            {
                Console.Write("\nEnter the superuser password. Password won't be shown, type each char individually, backspace to correct, enter to continue\n>");
                while (true) 
                { 
                    try 
                    { 
                        string superUserPwd = Helper.PasswordMaker();
                        Helper.SuperUserMode(superUserPwd, appName, baseLoc);
                        break;
                    }
                    catch { Console.Write("\nError. Retype password\n>"); continue; }
                }
            }
            else { Console.Write("Continuing in standard mode"); }
            TranslationMaker.defaultFileMaker();

            //select language or implement new language
            Console.Write("\nPlease either input your desired language or input \"new\" to implement another one. (without quotes)\n>");
            string langPreferenceLong = Console.ReadLine().ToLower(), spacer = "-------------------------";
            JsonHandling.langVal langValue = null;
            if (langPreferenceLong == "new")
            {
                Console.WriteLine("Please input the name of the language you want to implement");
                string langName = Console.ReadLine();
                if (TranslationMaker.newFileMaker(langName) == true)
                {
                    langValue = JsonSerializer.Deserialize<JsonHandling.langVal>(Inputs.langHandler(langName));
                }
            }
            else
            {
                if (langPreferenceLong.ToLower() == "english")
                {
                    langValue = JsonSerializer.Deserialize<JsonHandling.langVal>(Inputs.langHandler("default"));
                    Console.WriteLine($"\n{spacer}\n");
                    Console.WriteLine($"using {char.ToUpper(langPreferenceLong[0]) + langPreferenceLong.Substring(1)}");
                    Console.WriteLine($"\n{spacer}");
                }
                else
                {
                    try 
                    { 
                        langValue = JsonSerializer.Deserialize<JsonHandling.langVal>(Inputs.langHandler(langPreferenceLong));
                        Console.WriteLine($"\n{spacer}\n");
                        Console.WriteLine($"using {char.ToUpper(langPreferenceLong[0]) + langPreferenceLong.Substring(1)}");
                        Console.WriteLine($"\n{spacer}");
                    }
                    catch
                    {
                        //resort to default on fail
                        langValue = JsonSerializer.Deserialize<JsonHandling.langVal>(Inputs.langHandler("default"));
                        Console.WriteLine($"\n{spacer}");
                        Console.WriteLine($"{langValue.invalidInput} Using default language (English)");
                        Console.WriteLine($"{spacer}");
                    }
                }
            }
            string langPreferenceShort = langValue.shortLanguage;

            //cfg getter
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
            JsonHandling.config config = JsonSerializer.Deserialize<JsonHandling.config>(File.ReadAllText(cfgLoc));

            //gets unit preference
            string unitPreference = Inputs.UnitPreference(langValue);

            //Build data for API call
            string city = null;
            int check = 0; 
            while (true)
            {
                
                Console.Write($"\n{langValue.nameOfCity}\n>");
                city = Console.ReadLine();
                if(!String.IsNullOrEmpty(city)) { break; }
                if (check >= 1) 
                {
                    Console.WriteLine(langValue.invalidInput);
                    continue;
                }
                check++;
            }

            //make API call
            WeatherResponse.root weatherData = Inputs.APICall(city, langPreferenceShort, unitPreference, config.apiKey).Result;

            string[] content = Outputs.WeatherOutput(weatherData, unitPreference, langValue);

            //mail option
            Helper.MailOption (langValue, config, content, cfgLoc, devData);
        }
    }
}