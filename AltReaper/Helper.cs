using Renci.SshNet;
using System.Text.Json;
using static System.Console;
using Reaper.IO;

namespace Reaper
{
    public static class globalVars
    {
        // Load language lists and application metadata from resources (null-safe)
        public static string[] fullySupportedLanguages = (Properties.Resources.ResourceManager.GetString("FullySupportedLanguages") ?? string.Empty)
            .Split('|', StringSplitOptions.RemoveEmptyEntries);
        public static string[] supportedShortCodes = (Properties.Resources.ResourceManager.GetString("SupportedShortCodes") ?? string.Empty)
            .Split('|', StringSplitOptions.RemoveEmptyEntries);
        public static string[] devData = { Properties.Resources.AppName, Properties.Resources.DevName };
        public static string baseLoc = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\Reaper";
        public static string cfgLoc = $"{baseLoc}\\config.json";

        // App visible languages and mapping to OpenWeatherMap API codes / culture names.
        // Keep small for now: English and German only.
        public static string[] appLanguages = { "English", "German" };
        public static Dictionary<string, string> appToApiCode = new()
        {
            { "English", "en" },
            { "German", "de" }
        };
        // Map app language display -> culture name used for ResourceManager lookups (use BCP-47 style)
        public static Dictionary<string, string> appToCulture = new()
        {
            { "English", "en" },
            { "German", "de" }
        };
    }
    internal class Helper
    {
        public static void MailOption(JsonHandling.config config, String[] content)
        {
            string[] mailOptions = { Properties.Resources.YesOption, Properties.Resources.NoOption };
            Menu mailMenu = new(Properties.Resources.mailWanted, mailOptions);

            if (mailMenu.IRExecute() == 0)
            {
                bool partSuccess = false;
                while (!partSuccess)
                {
                    if (Checks.cfgChecker(config))
                    {
                        Inputs.configGen(config);
                        config = JsonSerializer.Deserialize<JsonHandling.config>(File.ReadAllText(globalVars.cfgLoc));
                    }
                    Write($"\n{Properties.Resources.mailAddressQuery}\n>");
                    CursorVisible = true;
                    ForegroundColor = ConsoleColor.White;
                    ForegroundColor = ConsoleColor.Green;
                    CursorVisible = false;
                    if (Outputs.MailOutput(ReadLine(), Properties.Resources.yourWeatherInfo, content, config))
                    {
                        Closer(config);
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
                        password = password[..^1];
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
            SftpClient sftp = new("ssh.strato.de", 22, $"sftp_{appName}@wettersense.de", superUserPwd);
            sftp.Connect();

            Stream configLoc = File.Create($"{directoryLoc}\\config.json");
            sftp.DownloadFile(@"/config.json", configLoc);
            configLoc.Close();

            sftp.Disconnect();
        }
        public static void Closer()
        {
            Uninstaller();
            WriteLine(string.Format(Properties.Resources.ThankYouFormat, globalVars.devData[0]));
            WriteLine(Properties.Resources.WeatherPoweredBy);
            WriteLine(string.Format(Properties.Resources.BylineFormat, globalVars.devData[0], globalVars.devData[1]));
            WriteLine(Properties.Resources.PressAnyKeyExit);
            ReadKey(true);
            Environment.Exit(0);
        }
        public static void Closer(JsonHandling.config config)
        {
            WriteLine($"{Properties.Resources.mailSuccessMessage}");
            Uninstaller();
            WriteLine(string.Format(Properties.Resources.ThankYouFormat, globalVars.devData[0]));
            WriteLine(Properties.Resources.WeatherPoweredBy);
            WriteLine(string.Format(Properties.Resources.MailPoweredByFormat, config.senderMail.Split('@')[1]));
            WriteLine(string.Format(Properties.Resources.BylineFormat, globalVars.devData[0], globalVars.devData[1]));
            WriteLine(Properties.Resources.PressAnyKeyExit);
            ReadKey(true);
            Environment.Exit(0);
        }
        public static void Uninstaller()
        {
            string uninstallPrompt = string.Format(Properties.Resources.UninstallPromptFormat, globalVars.devData[0]);
            string[] uninstallOptions = { Properties.Resources.YesOption, Properties.Resources.NoOption };
            Menu uninstallMenu = new(uninstallPrompt, uninstallOptions);
            int uninstallChoice = uninstallMenu.IRExecute();
            if (uninstallChoice == 0)
            {
                Directory.Delete(globalVars.baseLoc, true);
            }
        }
    }
}
