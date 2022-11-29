using Renci.SshNet;
using System.Security;
using System.Net;
using System.Text.RegularExpressions;

namespace Reaper
{
    internal class Helper
    {
        public static void MailOption(String[] langValue, String[] config, String[] content, String cfgLoc)
        {
            int cfgOptCount = 5;
            Console.Write($"\n{langValue[18]} ({langValue[16]},{langValue[17]})\n>");
            string answer = Console.ReadLine().ToLower();
            if (answer == langValue[16])
            {
                bool partSuccess = false;
                while (!partSuccess)
                {
                    if (File.ReadAllLines(cfgLoc).Length != 6)
                    {
                        Inputs.configGen(cfgLoc, cfgOptCount, langValue, config);
                        config = File.ReadAllLines(cfgLoc);
                        continue;
                    }
                    Console.Write($"\n{langValue[19]}\n>");
                    string recipient = Console.ReadLine();
                    if (Outputs.MailOutput(recipient, langValue[15], content, langValue, config))
                    {
                        Console.WriteLine($"{langValue[21]}");
                        Console.WriteLine("Weather data powered by openweathermap.org");
                        Console.WriteLine("Mail powered by htmlemail.io & WetterSense.de");
                        Console.WriteLine("Reaper by WetterSenseDev");
                    }
                    else { throw new Exception(); }
                    partSuccess = true;
                }
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
                else if (keyPressed.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    password += keyPressed.KeyChar;
                    Console.Write("*");
                }
            }
            return password;
        }
        public static void SupervisorMode(String supervisorPwd, String appName, string directoryLoc)
        {
            string[] fullySupportedLanguages = { "afrikaans", "albanian", "arabic", "azerbaijani", "bulgarian", "catalan", "czech", "danish", "german", "greek", "english", "basque", "persian", "farsi", "finnish", "french", "galician", "Hebrew", "hindi", "croatian", "hungarian", "indonesian", "italian", "japanese", "korean", "latvian", "lithuanian", "macedonian", "norwegian", "dutch", "polish", "portuguese", "romanian", "russian", "swedish", "slovak", "slovenian", "spanish", "serbian", "thai", "turkish", "ukrainian", "vietnamese", "chinese simplified", "chinese traditional", "zulu" };
            SftpClient sftp = new SftpClient("ssh.strato.de", $"sftp_{appName}@wettersense.de", supervisorPwd);
            sftp.Connect();
            Stream configLoc = File.Create($"{directoryLoc}\\config.cfg");
            sftp.DownloadFile(@"/config.cfg", configLoc);
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
            
            Regex myRegex = new Regex(@"^[a-z]+Text\.txt$");
            List<string> downloadList = files.Where(f => myRegex.IsMatch(f)).ToList();
            foreach (string str in downloadList)
            {
                Stream langFileLoc = File.Create($"{directoryLoc}\\langFiles\\{str}");
                sftp.DownloadFile($"/langFiles/{str}", langFileLoc);
            }
            sftp.Disconnect();

        }
    }
}
