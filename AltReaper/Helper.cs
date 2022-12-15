using Renci.SshNet;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace Reaper
{
    internal class Helper
    {
        public static void MailOption(JsonHandling.langVal langValue, JsonHandling.config config, String[] content, String cfgLoc, String[] devData)
        {
            Console.Write($"\n{langValue.mailWanted} ({langValue.yes},{langValue.no})\n>");
            string answer = null;
            while (answer != langValue.yes && answer != langValue.no )
            { 
                answer = Console.ReadLine().ToLower();
                if (answer == langValue.yes)
                {
                    bool partSuccess = false;
                    while (!partSuccess)
                    {
                        bool problem = Checks.cfgChecker(config);
                        if (problem == true)
                        {
                            Inputs.configGen(cfgLoc, problem, langValue, config);
                            config = JsonSerializer.Deserialize<JsonHandling.config>(File.ReadAllText(cfgLoc));
                        }
                        Console.Write($"\n{langValue.mailAddressQuery}\n>");
                        string recipient = Console.ReadLine();
                        if (Outputs.MailOutput(recipient, langValue.yourWeatherInfo, content, langValue, config))
                        {
                            Closer(devData, config, langValue);
                        }
                        else { throw new Exception(); }
                        partSuccess = true;
                    }
                }
                else
                {
                    if (answer == langValue.no)
                    {
                        Closer(devData);
                    }
                }
            }
        }
        public static string PasswordMaker()
        {
            string password = "";
            while (true)
            {
                ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                if (keyPressed.Key == ConsoleKey.Enter) { break; }
                else if (keyPressed.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }

                // make non-unicode chars (like function keys) invalid
                else if (keyPressed.KeyChar != '\u0000')
                {
                    password += keyPressed.KeyChar;
                    Console.Write("*");
                }
            }
            return password;
        }
        public static void SuperUserMode(String superUserPwd, String appName, string directoryLoc)
        {
            //setting login credentials
            SftpClient sftp = new SftpClient("ssh.strato.de", $"sftp_{appName}@wettersense.de", superUserPwd);
            sftp.Connect();

            //getting config
            Stream configLoc = File.Create($"{directoryLoc}\\config.json");
            sftp.DownloadFile(@"/config.json", configLoc);
            configLoc.Close();
            
            //search for available language files
            var langFileList = sftp.ListDirectory("/langFiles/").ToList();
            System.Collections.IEnumerable list = sftp.ListDirectory("/langFiles/", null);
            System.Collections.IEnumerator enumerator = list.GetEnumerator();
            Renci.SshNet.Sftp.SftpFile sftpFile;
            string name;
            List<string> files = new List<string>();
            while (enumerator.MoveNext())
            {
                sftpFile = (Renci.SshNet.Sftp.SftpFile)enumerator.Current;
                name = sftpFile.Name;
                files.Add(name);
            }

            //getting available language files
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
        
        public static void Closer (String[] devData)
        {
            Uninstaller(devData);
            Console.WriteLine($"\nThank you for using {devData[0]}!");
            Console.WriteLine("Weather data powered by openweathermap.org");
            Console.WriteLine($"{devData[0]} by {devData[1]}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
            Environment.Exit(0);
        }
        public static void Closer(String[] devData, JsonHandling.config config, JsonHandling.langVal langValue)
        {
            Console.WriteLine($"{langValue.mailSuccessMessage}");
            Uninstaller(devData);
            Console.WriteLine($"\nThank you for using {devData[0]}!");
            Console.WriteLine("Weather data powered by openweathermap.org");
            Console.WriteLine($"Mail powered by htmlemail.io & {config.senderMail.Split('@')[1]}");
            Console.WriteLine($"{devData[0]} by {devData[1]}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
            Environment.Exit(0);
        }
        public static void Uninstaller(String[] devData)
        {
            string baseLoc = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Desktop\\Reaper";
            Console.Write($"\nDo you want to unistall {devData[0]} (yes,no)\n>");
            string choice = Console.ReadLine();
            if (choice == "yes")
            {
                Directory.Delete(baseLoc, true);
            }
        }
    }
}
