using System.Text.Json;

namespace Reaper
{
    internal class TranslationMaker
    {
        public static bool newFileMaker(string langName)
        {
            string[] supportedShortCodes = { "af", "al", "ar", "az", "bg", "ca", "cz", "da", "de", "el", "en", "eu", "fa", "fa", "fi", "fr", "gl", "he", "hi", "hr", "hu", "id", "it", "ja", "kr", "la", "lt", "mk", "no", "nl", "pl", "pt", "pt_br", "ro", "ru", "se", "sk", "sl", "sr", "th", "tr", "ua", "vi", "zh_cn", "zh_tw", "zu" };
            string[] fullySupportedLanguages = { "afrikaans", "albanian", "arabic", "azerbaijani", "bulgarian", "catalan", "czech", "danish", "german", "greek", "english", "basque", "persian", "farsi", "finnish", "french", "galician", "Hebrew", "hindi", "croatian", "hungarian", "indonesian", "italian", "japanese", "korean", "latvian", "lithuanian", "macedonian", "norwegian", "dutch", "polish", "portuguese", "romanian", "russian", "swedish", "slovak", "slovenian", "spanish", "serbian", "thai", "turkish", "ukrainian", "vietnamese", "chinese simplified", "chinese traditional", "zulu" };
            string currentUserDesktopPath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\Reaper\\langFiles\\";
            string futureFilePath = currentUserDesktopPath + langName.ToLower() + "Text.json";

            bool langCreated = false;
            bool success = false;
            if (langName == "default" | String.IsNullOrEmpty(langName)) { return false; } //ensures only files with names that arent defaultText or Text are created

            //Creates file if not existant or user confirms that they want to
            if (File.Exists(futureFilePath))
            {
                Console.Write("\nThere is already a File for the language you selected. If you want to overwrite it, type \"I know what I am doing!\" (without quotes)\n>");
                if (Console.ReadLine() == "I know what I am doing!") { } else { return false; }
            }

            //checks if language is fully supported
            string shortLanguage = null;
            if (fullySupportedLanguages.Contains(langName, StringComparer.OrdinalIgnoreCase))
            {
                int index = Array.FindIndex(fullySupportedLanguages, x => x.ToLower() == langName.ToLower());
                shortLanguage = supportedShortCodes[index];
            }
            else { Console.WriteLine("Language not fully supported. Some output may be in English"); shortLanguage = "en"; }

            //Instructions & translation items
            Console.WriteLine("Follow the instructions! \nOnly translate what is inside quotes! \nDo not put your translation in quotes unless told! \nPress Enter after translation to proceed");
            Console.Write("\nTranslate \"Enter the name of the unit system you want to use\"\n>");
            string unitQuery = Console.ReadLine();
            Console.Write("\nTranslate the word \"metric\" (the measurement/unit system)\n>");
            string metric = Console.ReadLine().ToLower();
            Console.Write("\nTranslate the word \"imperial\" (the mesarement/unit system)\n>");
            string imperial = Console.ReadLine().ToLower();
            Console.Write("\nTranslate \"Enter the name of the city which you want weather data of.\"\n>");
            string nameOfCity = Console.ReadLine();
            Console.Write("\nTranslate \"Invalid input.\"\n>");
            string invalidInput = Console.ReadLine();
            Console.Write("\nTranslate \"\"Press Enter to continue.\n>");
            string pressEnterContinue = Console.ReadLine();
            Console.Write("\nTranslate \"Error found.\"\n>");
            string errorMessage = Console.ReadLine();
            Console.Write("\nTranslate \"The weather in\" (like in )\n>");
            string theWeatherIn = Console.ReadLine();
            Console.Write("\nTranslate the word \"temperature\"\n>");
            string temp = Console.ReadLine();
            Console.Write("\nTranslate \"lowest Temperature\"\n>");
            string lowestTemp = Console.ReadLine();
            Console.Write("\nTranslate \"highest temperature\"\n>");
            string highestTemp = Console.ReadLine();
            Console.Write("\nTranslate the word \"description\"\n>");
            string description = Console.ReadLine();
            Console.Write("\nTranslate \"local system time\"\n>");
            string localSystemTime = Console.ReadLine();
            Console.Write("\nTranslate \"time at destination\"\n>");
            string timeAtDestination = Console.ReadLine();
            Console.Write("\nTranslate \"your weather info\"(formal your)\n>");
            string yourWeatherInfo = Console.ReadLine();
            Console.Write("\nTranslate the word \"yes\"\n>");
            string yes = Console.ReadLine().ToLower();
            Console.Write("\nTranslate the word \"no\"\n>");
            string no = Console.ReadLine().ToLower();
            Console.Write("\nDo you want this as a mail?\n>");
            string mailWanted = Console.ReadLine();
            Console.Write("\nEnter your mail address\n>");
            string mailAddressQuery = Console.ReadLine();
            string nameOr = "";
            while ((nameOr.Contains('"') && nameOr.Contains('(') && nameOr.Contains(')')) == false)
            {
                Console.WriteLine("Translate everything inside the next message, including \" \\ and messages inside brackets");
                Console.Write("\nEnter your name or type \"no\" to not be addressed (without quotes)\n>");
                nameOr = Console.ReadLine();
            }
            Console.Write("\nMail sent successfully\n>");
            string mailSuccessMessage = Console.ReadLine();

            var langVal = new JsonHandling.langVal
            {
                shortLanguage = shortLanguage, unitQuery = unitQuery, metric = metric, 
                imperial = imperial, nameOfCity = nameOfCity, invalidInput = invalidInput, 
                pressEnterContinue = pressEnterContinue, errorMessage = errorMessage, 
                theWeatherIn = theWeatherIn, temp = temp, lowestTemp = lowestTemp, 
                highestTemp = highestTemp, description = description, localSystemTime = localSystemTime, 
                timeAtDestination = timeAtDestination, yourWeatherInfo = yourWeatherInfo, 
                yes = yes, no = no, mailWanted = mailWanted, mailAddressQuery = mailAddressQuery,
                nameOr = nameOr, mailSuccessMessage = mailSuccessMessage,
            };

            //Writing array to file
            Console.WriteLine("Writing to file");
            string val = JsonSerializer.Serialize(langVal);
            File.WriteAllText(futureFilePath, val);
            if (File.Exists(futureFilePath)) { success = true; }
            Console.Clear();

            if (success)
            {
                langCreated = true;
                Console.WriteLine($"{char.ToUpper(langName[0]) + langName.Substring(1)} language file successfully created.");
            }
            return langCreated;
        }

        public static bool defaultFileMaker()
        {
            string[] fullySupportedLanguages = { "afrikaans", "albanian", "arabic", "azerbaijani", "bulgarian", "catalan", "czech", "danish", "german", "greek", "english", "basque", "persian", "farsi", "finnish", "french", "galician", "Hebrew", "hindi", "croatian", "hungarian", "indonesian", "italian", "japanese", "korean", "latvian", "lithuanian", "macedonian", "norwegian", "dutch", "polish", "portuguese", "romanian", "russian", "swedish", "slovak", "slovenian", "spanish", "serbian", "thai", "turkish", "ukrainian", "vietnamese", "chinese simplified", "chinese traditional", "zulu" };
            string langName = "default";
            bool langCreated = false, success = false;

            string currentUserDesktopPath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\Reaper\\langFiles\\";
            string futureFilePath = currentUserDesktopPath + langName.ToLower() + "Text.json";

            var langVal = new JsonHandling.langVal
            {
                shortLanguage = "en", unitQuery = "Enter the name of the unit system you want to use",
                metric = "metric", imperial = "imperial", errorMessage = "Error found.", no = "no",
                nameOfCity = "Enter the name of the city which you want weather data of.",
                invalidInput = "Invalid input.", pressEnterContinue = "Press Enter to continue.",
                theWeatherIn = "The weather in", temp = "temperature", lowestTemp = "lowest temperature",
                highestTemp = "highest temperature", description = "description", yes = "yes",
                timeAtDestination = "time at destination", yourWeatherInfo = "your weather info",
                mailWanted = "Do you want this as a mail?", mailAddressQuery = "Enter your mail address",
                nameOr = "Enter your name or type \"no\" to not be addressed (without quotes)",
                mailSuccessMessage = "Mail sent successfully", localSystemTime= "local system time"
            };
            //generate file output
            string val = JsonSerializer.Serialize(langVal);;
            File.WriteAllText(futureFilePath, val);
            
            if (File.Exists(futureFilePath)) { success = true; }
            if (success)
            {
                langCreated = true;
                Console.WriteLine($"{char.ToUpper(langName[0]) + langName.Substring(1)} language file successfully created.");
                Console.Clear();
                Console.WriteLine("Currently implemented languages: ");
                Console.WriteLine("-------------------------");
                foreach (string l in fullySupportedLanguages)
                {
                    if (File.Exists($"{currentUserDesktopPath}{l}Text.json")) 
                    { 
                        Console.Write(char.ToUpper(l[0]) + l.Substring(1) + ", "); 
                    }
                }
                Console.Write("default\n-------------------------\n");
            }
            return langCreated;
        }
    }
}
