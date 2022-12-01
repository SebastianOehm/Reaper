using Renci.SshNet;
using System.Text.RegularExpressions;

namespace Reaper
{
    
    internal class Helper
    {
        public static void MailOption(String[] langValue, String[] config, String[] content, String cfgLoc)
        {
            int cfgOptCount = 6;
            Console.Write($"\n{langValue[18]} ({langValue[16]},{langValue[17]})\n>");
            string answer ="";
            while (answer != langValue[16] | answer != langValue[17])
            { 
                answer = Console.ReadLine().ToLower();
                if (answer == langValue[16])
                {
                    bool partSuccess = false;
                    while (!partSuccess)
                    {
                        if (File.ReadAllLines(cfgLoc).Length != cfgOptCount)
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
        public static void SupervisorMode(String supervisorPwd, String appName, string directoryLoc)
        {
            //setting login credentials
            SftpClient sftp = new SftpClient("ssh.strato.de", $"sftp_{appName}@wettersense.de", supervisorPwd);
            sftp.Connect();

            //getting config
            Stream configLoc = File.Create($"{directoryLoc}\\config.cfg");
            sftp.DownloadFile(@"/config.cfg", configLoc);
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
            Regex myRegex = new Regex(@"^[a-z]+Text\.txt$");
            List<string> downloadList = files.Where(f => myRegex.IsMatch(f)).ToList();
            foreach (string str in downloadList)
            {
                Stream langFileLoc = File.Create($"{directoryLoc}\\langFiles\\{str}");
                sftp.DownloadFile($"/langFiles/{str}", langFileLoc);
                langFileLoc.Close();
            }
            sftp.Disconnect();
        }
    }
}
