using Renci.SshNet;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Console;

namespace Reaper
{
    public static class globalVars
    {
        public static string[] fullySupportedLanguages = { "afrikaans", "albanian", "arabic", "azerbaijani", "bulgarian", "catalan", "czech", "danish", "german", "greek", "english", "basque", "persian", "farsi", "finnish", "french", "galician", "Hebrew", "hindi", "croatian", "hungarian", "indonesian", "italian", "japanese", "korean", "latvian", "lithuanian", "macedonian", "norwegian", "dutch", "polish", "portuguese", "romanian", "russian", "swedish", "slovak", "slovenian", "spanish", "serbian", "thai", "turkish", "ukrainian", "vietnamese", "chinese simplified", "chinese traditional", "zulu" };
        public static string[] supportedShortCodes = { "af", "al", "ar", "az", "bg", "ca", "cz", "da", "de", "el", "en", "eu", "fa", "fa", "fi", "fr", "gl", "he", "hi", "hr", "hu", "id", "it", "ja", "kr", "la", "lt", "mk", "no", "nl", "pl", "pt", "pt_br", "ro", "ru", "se", "sk", "sl", "sr", "th", "tr", "ua", "vi", "zh_cn", "zh_tw", "zu" };
        public static string appName = "Reaper", devName = "WetterSenseDev", versionNumber = "0.9.1";
        public static string[] devData = { appName, devName };
        public static string baseLoc = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\Reaper";
        public static string tree = $"{baseLoc}\\langFiles\\", cfgLoc = $"{baseLoc}\\config.json";
    }
    internal class Helper
    {
        public static void MailOption(JsonHandling.langVal langValue, JsonHandling.config config, String[] content)
        {
            string[] mailOptions = { langValue.yes, langValue.no };
            Menu mailMenu = new Menu(langValue.mailWanted, mailOptions);

            if (mailMenu.IRExcecute() == 0)
            {
                bool partSuccess = false;
                while (!partSuccess)
                {
                    if (Checks.cfgChecker(config))
                    {
                        Inputs.configGen(langValue, config);
                        config = JsonSerializer.Deserialize<JsonHandling.config>(File.ReadAllText(globalVars.cfgLoc));
                    }
                    Write($"\n{langValue.mailAddressQuery}\n>");
                    CursorVisible = true;
                    ForegroundColor = ConsoleColor.White;
                    ForegroundColor = ConsoleColor.Green;
                    CursorVisible = false;
                    if (Outputs.MailOutput(ReadLine(), langValue.yourWeatherInfo, content, langValue, config))
                    {
                        Closer(config, langValue);
                    }
                    else { throw new Exception(); }
                    partSuccess = true;
                }
            }
            else
            {
                Closer();
            }
        }
        public static string PasswordMaker()
        {
            ForegroundColor = ConsoleColor.White;
            string password = "";
            while (true)
            {
                ConsoleKeyInfo keyPressed = ReadKey(true);
                if (keyPressed.Key == ConsoleKey.Enter) { break; }
                else if (keyPressed.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                        Write("\b \b");
                    }
                }

                // make non-unicode chars (like function keys) invalid
                else if (keyPressed.KeyChar != '\u0000')
                {
                    password += keyPressed.KeyChar;
                    Write("*");
                }
            }
            ForegroundColor = ConsoleColor.Green;
            return password;
        }
        public static void SuperUserMode(String superUserPwd, String appName, string directoryLoc)
        {
            SftpClient sftp = new SftpClient("ssh.strato.de", $"sftp_{appName}@wettersense.de", superUserPwd);
            sftp.Connect();

            Stream configLoc = File.Create($"{directoryLoc}\\config.json");
            sftp.DownloadFile(@"/config.json", configLoc);
            configLoc.Close();

            System.Collections.IEnumerator enumerator = sftp.ListDirectory("/langFiles/", null).GetEnumerator();
            List<string> files = new List<string>();
            while (enumerator.MoveNext())
            {
                Renci.SshNet.Sftp.SftpFile sftpFile = (Renci.SshNet.Sftp.SftpFile)enumerator.Current;
                string name = sftpFile.Name;
                files.Add(name);
            }

            Regex myRegex = new Regex(@"^[a-z]+Text\.json$");
            List<string> downloadList = files.Where(f => myRegex.IsMatch(f)).ToList();
            foreach (string str in downloadList)
            {
                Stream langFileLoc = File.Create($"{directoryLoc}\\langFiles\\{str}");
                sftp.DownloadFile($"/langFiles/{str}", langFileLoc);
                langFileLoc.Close();
            }
            sftp.Disconnect();
        }
        public static void Closer()
        {
            Uninstaller();
            WriteLine($"\nThank you for using {globalVars.devData[0]}!");
            WriteLine("Weather data powered by openweathermap.org");
            WriteLine($"{globalVars.devData[0]} by {globalVars.devData[1]}");
            WriteLine("Press any key to exit.");
            ReadKey(true);
            Environment.Exit(0);
        }
        public static void Closer(JsonHandling.config config, JsonHandling.langVal langValue)
        {
            WriteLine($"{langValue.mailSuccessMessage}");
            Uninstaller();
            WriteLine($"\nThank you for using {globalVars.devData[0]}!");
            WriteLine("Weather data powered by openweathermap.org");
            WriteLine($"Mail powered by htmlemail.io & {config.senderMail.Split('@')[1]}");
            WriteLine($"{globalVars.devData[0]} by {globalVars.devData[1]}");
            WriteLine("Press any key to exit.");
            ReadKey(true);
            Environment.Exit(0);
        }
        public static void Uninstaller()
        {
            string baseLoc = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper";
            string uninstallPrompt = $"\nDo you want to unistall {globalVars.devData[0]}?";
            string[] uninstallOptions = { "yes", "no" };
            Menu uninstallMenu = new(uninstallPrompt, uninstallOptions);
            int uninstallChoice = uninstallMenu.IRExcecute();
            if (uninstallChoice == 0)
            {
                Directory.Delete(baseLoc, true);
            }
        }
    }
}
