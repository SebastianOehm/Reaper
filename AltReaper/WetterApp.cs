using System.Text.Json;
using System.Security;
using System.Net;


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
            string appName = "Reaper";
            double versionNumber = 0.7;
            string title = $"{appName} v{versionNumber.ToString().Replace(',', '.')}";
            Console.Title = title;

            //generate default directory structure and langFile
            string baseLoc = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper";
            string tree = $"{baseLoc}\\langFiles\\";
            string cfgLoc = $"{baseLoc}\\config.cfg";
            if (!Directory.Exists(tree)) { Directory.CreateDirectory(tree); }
            Console.Write("\nDo you want to enter supervisor mode? (yes, no)\n>");
            string a = Console.ReadLine();
            if (a == "yes")
            {
                Console.Write("\nEnter the supervisor password. Password won't be shown, type each char individually, backspace to correct, enter to continue\n>");
                SecureString supervisorPwd = new NetworkCredential("", Helper.PasswordMaker()).SecurePassword;
                Helper.SupervisorMode(supervisorPwd, appName, baseLoc);
            }
            else { Console.Write("Continuing in standard mode"); }
            TranslationMaker.defaultFileMaker();

            //select language or implement new language
            string[] langValue = null;
            Console.Write("\nPlease either input your desired language or input \"new\" to implement another one. (without quotes)\n>");
            string langPreferenceLong = Console.ReadLine().ToLower();
            if (!String.IsNullOrEmpty(langPreferenceLong))
            {
                if (langPreferenceLong == "new")
                {
                    Console.WriteLine("Please input the name of the language you want to implement");
                    string langName = Console.ReadLine();
                    if (TranslationMaker.newFileMaker(langName) == true)
                    {
                        langValue = Inputs.langHandler(langName);
                    }
                }
                else
                {
                    try { langValue = Inputs.langHandler(langPreferenceLong); }
                    catch
                    {
                        //resort to default on fail
                        langValue = Inputs.langHandler("default");
                        Console.WriteLine($"{langValue[5]} Using default language (English)");
                    }
                }
            }
            else
            {
                langValue = Inputs.langHandler("default");
                Console.WriteLine($"{langValue[5]} Using default language (English)");
            }
            string langPreferenceShort = langValue[0];

            //cfg getter
            if (!File.Exists(cfgLoc))
            {
                Console.Write("\nEnter your APIKey\n>");
                string apiKey = Console.ReadLine();
                File.WriteAllText(cfgLoc, apiKey);
            }
            string [] config = File.ReadAllLines(cfgLoc);
            
            //gets unit preference
            string unitPreference = null;
            while (!langValue.Contains(unitPreference))
            {
                try
                {
                    Console.Write($"\n{langValue[1]} ({langValue[2]},{langValue[3]})\n>");
                    unitPreference = Console.ReadLine().ToLower();
                    if ((String.IsNullOrEmpty(unitPreference)) || (!langValue.Contains(unitPreference)))
                    {
                        Console.WriteLine(langValue[5]);
                        Console.WriteLine(langValue[6]);
                        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                    }
                }
                catch (Exception unitException){ Console.WriteLine(langValue[7]); continue; }
            }

            //Build data for API call
            if (unitPreference == langValue[2]) { unitPreference = "metric"; } else { unitPreference = "imperial"; }
            Console.Write($"\n{langValue[4]}\n>");
            string city = null;
            while (String.IsNullOrEmpty(city)){ city = Console.ReadLine(); }

            //make API call
            string json = Inputs.APICall(city, langPreferenceShort, unitPreference, config[0]);

            //deserialize Json response
            JsonResponseDeserializer.root wetterDaten = JsonSerializer.Deserialize<JsonResponseDeserializer.root>(json);
            string[] content = Outputs.WeatherOutput(wetterDaten, unitPreference, langValue);

            //mail option
            Helper.MailOption(langValue, config, content, cfgLoc);
        }
    }
}
