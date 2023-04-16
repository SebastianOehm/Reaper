using System.Text.Json;
using static System.Console;

namespace Reaper
{
    internal class TranslationMaker
    {
        public static bool newFileMaker(string langName)
        {
            string currentUserDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Desktop\\Reaper\\langFiles\\";
            string futureFilePath = currentUserDesktopPath + langName.ToLower() + "Text.json";

            bool langCreated = false;
            if (langName.ToLower() == "default" | String.IsNullOrEmpty(langName) | langName.ToLower() == "english") { return false; } //ensures only files with names that arent defaultText or Text are created
            //Creates file if not existant or user confirms that they want to
            if (File.Exists(futureFilePath))
            {
                Write("\nThere is already a File for the language you selected. If you want to overwrite it, type \"I know what I am doing!\" (without quotes)\n>");
                if (ReadLine() == "I know what I am doing!") { } else { return false; }
            }

            //checks if language is fully supported
            string shortLanguage = null;
            if (globalVars.fullySupportedLanguages.Contains(langName, StringComparer.OrdinalIgnoreCase))
            {
                int index = Array.FindIndex(globalVars.fullySupportedLanguages, x => x.ToLower() == langName.ToLower());
                shortLanguage = globalVars.supportedShortCodes[index];
            }
            else { WriteLine("Language not fully supported. Some output may be in English"); shortLanguage = "en"; }

            //Instructions & translation items
            WriteLine("Follow the instructions! \nOnly translate what is inside quotes! \nDo not put your translation in quotes unless told! \nPress Enter after translation to proceed");
            Write("\nTranslate \"Enter the name of the unit system you want to use\"\n>");
            string unitQuery = ReadLine();
            Write("\nTranslate the word \"metric\" (the measurement/unit system)\n>");
            string metric = ReadLine().ToLower();
            Write("\nTranslate the word \"imperial\" (the mesarement/unit system)\n>");
            string imperial = ReadLine().ToLower();
            Write("\nTranslate \"Enter the name of the city which you want weather data of.\"\n>");
            string nameOfCity = ReadLine();
            Write("\nTranslate \"Invalid input.\"\n>");
            string invalidInput = ReadLine();
            Write("\nTranslate \"\"Press Enter to continue.\n>");
            string pressEnterContinue = ReadLine();
            Write("\nTranslate \"Error found.\"\n>");
            string errorMessage = ReadLine();
            Write("\nTranslate \"The weather in\" (like in )\n>");
            string theWeatherIn = ReadLine();
            Write("\nTranslate the word \"temperature\"\n>");
            string temp = ReadLine();
            Write("\nTranslate \"lowest Temperature\"\n>");
            string lowestTemp = ReadLine();
            Write("\nTranslate \"highest temperature\"\n>");
            string highestTemp = ReadLine();
            Write("\nTranslate the word \"description\"\n>");
            string description = ReadLine();
            Write("\nTranslate \"local system time\"\n>");
            string localSystemTime = ReadLine();
            Write("\nTranslate \"time at destination\"\n>");
            string timeAtDestination = ReadLine();
            Write("\nTranslate \"your weather info\"(formal your)\n>");
            string yourWeatherInfo = ReadLine();
            Write("\nTranslate the word \"yes\"\n>");
            string yes = ReadLine().ToLower();
            Write("\nTranslate the word \"no\"\n>");
            string no = ReadLine().ToLower();
            Write("\nDo you want this as a mail?\n>");
            string mailWanted = ReadLine();
            Write("\nEnter your mail address\n>");
            string mailAddressQuery = ReadLine();
            string nameOr = "";
            while ((nameOr.Contains('"') && nameOr.Contains('(') && nameOr.Contains(')')) == false)
            {
                WriteLine("Translate everything inside the next message, including \" \\ and messages inside brackets");
                Write("\nEnter your name or type \"no\" to not be addressed (without quotes)\n>");
                nameOr = ReadLine();
            }
            Write("\nMail sent successfully\n>");
            string mailSuccessMessage = ReadLine();

            var langVal = new JsonHandling.langVal
            {
                shortLanguage = shortLanguage,
                unitQuery = unitQuery,
                metric = metric,
                imperial = imperial,
                nameOfCity = nameOfCity,
                invalidInput = invalidInput,
                pressEnterContinue = pressEnterContinue,
                errorMessage = errorMessage,
                theWeatherIn = theWeatherIn,
                temp = temp,
                lowestTemp = lowestTemp,
                highestTemp = highestTemp,
                description = description,
                localSystemTime = localSystemTime,
                timeAtDestination = timeAtDestination,
                yourWeatherInfo = yourWeatherInfo,
                yes = yes,
                no = no,
                mailWanted = mailWanted,
                mailAddressQuery = mailAddressQuery,
                nameOr = nameOr,
                mailSuccessMessage = mailSuccessMessage,
            };

            WriteLine("Writing to file");
            File.WriteAllText(futureFilePath, JsonSerializer.Serialize(langVal));
            Clear();

            if (File.Exists(futureFilePath))
            {
                langCreated = true;
                WriteLine($"{char.ToUpper(langName[0]) + langName.Substring(1)} language file successfully created.");
            }
            return langCreated;
        }

        public static bool defaultFileMaker()
        {
            string langName = "english";
            bool langCreated = false;

            string currentUserDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Desktop\\Reaper\\langFiles\\";
            string futureFilePath = currentUserDesktopPath + langName.ToLower() + "Text.json";

            var langVal = new JsonHandling.langVal
            {
                shortLanguage = "en",
                unitQuery = "Enter the name of the unit system you want to use",
                metric = "metric",
                imperial = "imperial",
                errorMessage = "Error found.",
                no = "no",
                nameOfCity = "Enter the name of the city which you want weather data of.",
                invalidInput = "Invalid input.",
                pressEnterContinue = "Press Enter to continue.",
                theWeatherIn = "The weather in",
                temp = "temperature",
                lowestTemp = "lowest temperature",
                highestTemp = "highest temperature",
                description = "description",
                yes = "yes",
                timeAtDestination = "time at destination",
                yourWeatherInfo = "your weather info",
                mailWanted = "Do you want this as a mail?",
                mailAddressQuery = "Enter your mail address",
                nameOr = "Enter your name or type \"no\" to not be addressed (without quotes)",
                mailSuccessMessage = "Mail sent successfully",
                localSystemTime = "local system time"
            };
            File.WriteAllText(futureFilePath, JsonSerializer.Serialize(langVal));

            if (File.Exists(futureFilePath))
            {
                langCreated = true;
                WriteLine($"{char.ToUpper(langName[0]) + langName.Substring(1)} language file successfully created.");
                Clear();
            }
            return langCreated;
        }
    }
}