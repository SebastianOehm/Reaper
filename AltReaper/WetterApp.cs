using System.Text.Json;


/* LineByLineTranslationFileInstructions W = Word  number=line index
0   ShortLanguageCode (not a query)
1   Enter unit preference Query
2   W metric
3   W imperial
4   Enter name of place for which you want weather data
5   invalidInput
6   pressEnterContinue
7   errorFound
8   theWeatherIn
9   W temperature
10  W lowestTemperature
11  W highestTemperature
12  W description
13  LocalSystemTime
14  Time at destination
15  Your weather Info (formal)
16  W yes
17  W no
18  mailWanted
19  mailAddressQuery
20  nameOr
21  mailSuccessMessage
*/

namespace Reaper
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            //ConsoleWindowTitle
            double versionNumber = 0.6;
            Console.Title = $"Reaper v{String.Format("0.##", versionNumber.ToString())}";

            //generate default language file
            string baseLoc = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper";
            string tree = $"{baseLoc}\\langFiles\\";
            string cfgLoc = $"{baseLoc}\\config.cfg";
            if (!Directory.Exists(tree)) { Directory.CreateDirectory(tree); }
            TranslationMaker.defaultFileMaker();
            string[] config = Inputs.configReader(cfgLoc);

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
                    try
                    {
                        langValue = Inputs.langHandler(langPreferenceLong);
                    }
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
                catch (Exception unitException)
                {
                    Console.WriteLine(langValue[7]);
                    continue;
                }
            }

            //Build data for APICall bool APISuccess = false;
            if (unitPreference == langValue[2]) { unitPreference = "metric"; } else { unitPreference = "imperial"; }
            Console.Write($"\n{langValue[4]}\n>");
            string city = null;
            while (String.IsNullOrEmpty(city))
            {
                city = Console.ReadLine();
            }
            string json = Inputs.APICall(city, langPreferenceShort, unitPreference, config[0]);
            //deserialize Json response
            JsonResponseDeserializer.root wetterDaten = JsonSerializer.Deserialize<JsonResponseDeserializer.root>(json);
            string[] content = Outputs.WeatherOutput(wetterDaten, unitPreference, langValue);

            //mail option
            Console.Write($"\n{langValue[19]} ({langValue[16]},{langValue[17]})\n>");
            string answer = Console.ReadLine().ToLower();
            if (answer == langValue[16])
            {
                Console.Write($"\n{langValue[20]}\n>");
                string recipient = Console.ReadLine();
                if (Outputs.MailOutput(recipient, langValue[15], content, langValue, config[1], config[2], config[3], int.Parse(config[4])) == true)
                {
                    Console.WriteLine($"{langValue[21]}");
                    Console.WriteLine("Weather data powered by openweathermap.org");
                    Console.WriteLine("Mail powered by htmlemail.io & WetterSense.de");
                }
                else { throw new Exception(); }
            }
            else
            {
                if (answer == langValue[17])
                {
                    Console.WriteLine("Goodbye");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
        }
    }
}
