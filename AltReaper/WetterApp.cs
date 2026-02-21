using Reaper.IO;
using System.Globalization;
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

            Title = string.Format(Properties.Resources.AppTitleFormat, Properties.Resources.AppName, Properties.Resources.VersionNumber);
            ForegroundColor = ConsoleColor.Green;
            OutputEncoding = System.Text.Encoding.UTF8;
            CursorVisible = false;
            Clear();

            Checks.DeviceIsOnline();
            Checks.APIisOnline();

            string[] superUserOptions = { Properties.Resources.YesOption, Properties.Resources.NoOption };
            Menu superUser = new(Properties.Resources.SuperUserQuestion, superUserOptions);

            if (superUser.IRExecute() == 0)
            {
                Write("\n" + Properties.Resources.SuperUserPasswordPrompt + "\n>");
                while (true)
                {
                    try
                    {
                        Helper.SuperUserMode(Helper.PasswordMaker(), Properties.Resources.AppName, globalVars.baseLoc);
                        break;
                    }
                    catch { Write("\n" + Properties.Resources.ErrorRetypePassword + "\n>"); continue; }
                }
            }
            else { Write(Properties.Resources.ContinuingStandardMode); }

            string chosenLanguage = Inputs.langPreference();
            (string apiCode, CultureInfo culture) = LanguageLoader.Load(chosenLanguage);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            Inputs.ConfigGetter();
            config config = JsonSerializer.Deserialize<config>(File.ReadAllText(globalVars.cfgLoc));


            string unitPreference = Inputs.UnitPreference();
            int check = 0;
            CursorVisible = true;
            string? city;
            while (true)
            {
                Write($"\n{Properties.Resources.nameOfCity}\n>");
                ForegroundColor = ConsoleColor.White;
                city = ReadLine();
                if (!string.IsNullOrEmpty(city)) { break; }
                if (check >= 1)
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine(Properties.Resources.invalidInput);
                    continue;
                }
                check++;
            }
            ForegroundColor = ConsoleColor.Green;
            CursorVisible = false;

            root weatherData = Inputs.APICall(city, apiCode, unitPreference, config.apiKey).Result;

            var content = Outputs.WeatherOutput(weatherData, unitPreference);

            Helper.MailOption(config, content);
        }
    }
}