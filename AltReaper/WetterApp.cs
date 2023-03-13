using System.Text.Json;
using static Reaper.JsonHandling;
using static Reaper.WeatherResponse;
using static System.Console;

namespace Reaper
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            
            Title = $"{globalVars.appName} v{globalVars.versionNumber}";
            ForegroundColor = ConsoleColor.Green;
            OutputEncoding = System.Text.Encoding.UTF8;
            CursorVisible = false;
            Clear();

            Checks.DeviceIsOnline();
            Checks.APIisOnline();

 
            if (!Directory.Exists(globalVars.tree)) { Directory.CreateDirectory(globalVars.tree); }

            string[] superUserOptions = { "yes", "no" };
            Menu superUser = new("Do you want to enter superuser mode?", superUserOptions);

            if (superUser.IRExcecute() == 0)
            {
                Write("\nEnter the superuser password. Password won't be shown, type each char individually, backspace to correct, enter to continue\n>");
                while (true)
                {
                    try
                    {
                        Helper.SuperUserMode(Helper.PasswordMaker(), globalVars.appName, globalVars.baseLoc);
                        break;
                    }
                    catch { Write("\nError. Retype password\n>"); continue; }
                }
            }
            else { Write("Continuing in standard mode"); }
            TranslationMaker.defaultFileMaker();


            string chosenLanguage = Inputs.langPreference();
            langVal langValue = JsonSerializer.Deserialize<langVal>(Inputs.langHandler(chosenLanguage));

            Inputs.ConfigGetter();
            config config = JsonSerializer.Deserialize<config>(File.ReadAllText(globalVars.cfgLoc));


            string unitPreference = Inputs.UnitPreference(langValue);

            string city = null;
            int check = 0;
            CursorVisible = true;
            while (true)
            {
                Write($"\n{langValue.nameOfCity}\n>");
                ForegroundColor = ConsoleColor.White;
                city = ReadLine();
                if (!String.IsNullOrEmpty(city)) { break; }
                if (check >= 1)
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine(langValue.invalidInput);
                    continue;
                }
                check++;
            }
            ForegroundColor = ConsoleColor.Green;
            CursorVisible = false;

            root weatherData = Inputs.APICall(city, langValue.shortLanguage, unitPreference, config.apiKey).Result;

            var content = Outputs.WeatherOutput(weatherData, unitPreference, langValue);

            Helper.MailOption(langValue, config, content);
        }
    }
}