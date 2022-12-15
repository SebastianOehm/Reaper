﻿using System.Text.Json;
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
            string[] fullySupportedLanguages = { "afrikaans", "albanian", "arabic", "azerbaijani", "bulgarian", "catalan", "czech", "danish", "german", "greek", "english", "basque", "persian", "farsi", "finnish", "french", "galician", "Hebrew", "hindi", "croatian", "hungarian", "indonesian", "italian", "japanese", "korean", "latvian", "lithuanian", "macedonian", "norwegian", "dutch", "polish", "portuguese", "romanian", "russian", "swedish", "slovak", "slovenian", "spanish", "serbian", "thai", "turkish", "ukrainian", "vietnamese", "chinese simplified", "chinese traditional", "zulu" };

            //Set window title to Reaper.versionName
            string appName = "Reaper", devName = "WetterSenseDev", versionNumber = "0.9";
            string[] devData = { appName, devName };
            Console.Title = $"{appName} v{versionNumber}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();

            //check connection and API status
            Checks.DeviceIsOnline(devData);
            Checks.APIisOnline(devData);

            //generate default directory structure and langFile
            string baseLoc = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper";
            string tree = $"{baseLoc}\\langFiles\\", cfgLoc = $"{baseLoc}\\config.json";
            if (!Directory.Exists(tree)) { Directory.CreateDirectory(tree); }
            string[] superUserOptions = { "yes", "no" };
            string superUserPrompt = "Do you want to enter superuser mode?";
            Menu superUser = new(superUserPrompt, superUserOptions);
            int superUserChoice = superUser.IRExcecute();
            // ask if user wants to use superuser mode
            if (superUserChoice == 0)
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

            //setting language
            string chosenLanguage = Inputs.langPreference(fullySupportedLanguages, tree);
            JsonHandling.langVal langValue = JsonSerializer.Deserialize<JsonHandling.langVal>(Inputs.langHandler(chosenLanguage));

            //cfg getter
            Inputs.ConfigGetter(cfgLoc);
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
            WeatherResponse.root weatherData = Inputs.APICall(city, langValue.shortLanguage, unitPreference, config.apiKey).Result;

            string[] content = Outputs.WeatherOutput(weatherData, unitPreference, langValue);

            //mail option
            Helper.MailOption (langValue, config, content, cfgLoc, devData);
            
        }
    }
}