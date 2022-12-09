using Renci.SshNet;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Net;
using System;
using System.Globalization;
using static Reaper.JsonHandling;

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
                        bool problem = cfgChecker(config);
                        if (problem == true)
                        {
                            Inputs.configGen(cfgLoc, problem, langValue, config);
                            config = JsonSerializer.Deserialize<JsonHandling.config>(File.ReadAllText(cfgLoc));
                        }
                        Console.Write($"\n{langValue.mailAddressQuery}\n>");
                        string recipient = Console.ReadLine();
                        if (Outputs.MailOutput(recipient, langValue.yourWeatherInfo, content, langValue, config))
                        {
                            Closer(true, devData, config, langValue);
                        }
                        else { throw new Exception(); }
                        partSuccess = true;
                    }
                }
                else
                {
                    if (answer == langValue.no)
                    {
                        Closer(false, devData);
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
        public static bool cfgChecker(JsonHandling.config config)
        {       
            bool[] bools = { String.IsNullOrEmpty(config.apiKey), String.IsNullOrEmpty(config.senderMail), String.IsNullOrEmpty(config.senderMailPassword), String.IsNullOrEmpty(config.hostDomain), String.IsNullOrEmpty(config.portNumber), false, String.IsNullOrEmpty(config.bcc) };
            try { if (int.Parse(config.portNumber) < 0 && int.Parse(config.portNumber) > 65535) { bools[5] = true; } }
            catch { return true; }
            bool problem = false;
            // if all values of bools are true
            if (bools.Any(x => x) == true) { problem = true; }
            return problem;
        }
        public static bool DeviceIsOnline()
        {
            bool isOnline = false;
            Ping ping1 = new();
            Ping ping2 = new();
            PingReply reply1 = ping1.Send("1.1.1.1");
            PingReply reply2 = ping2.Send("8.8.8.8");
            if ( reply1.Status == IPStatus.Success | reply2.Status == IPStatus.Success)
            {
                isOnline = true;
            }
            
            return isOnline;
        }
        public static bool APIsOnline()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            bool isOnline = false;
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                // Creates an HttpWebRequest for the specified URL. 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?q=London");
                // Sends the HttpWebRequest and waits for a response.
                request.Proxy = defaultWebProxy;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();

            }
            catch (WebException e)
            {
                if( e.Message.Equals("The remote server returned an error: (401) Unauthorized.", StringComparison.InvariantCultureIgnoreCase)) { isOnline = true; }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nThe following Exception was raised : {0}", e.Message);
            }
            return isOnline;
        }
        public static void Closer (bool mailSuccess, String[] devData)
        {
            Console.WriteLine($"\nThank you for using {devData[0]}!");
            Console.WriteLine("Weather data powered by openweathermap.org");
            Console.WriteLine($"{devData[0]} by {devData[1]}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
        public static void Closer(bool mailSuccess, String[] devData, JsonHandling.config config, JsonHandling.langVal langValue)
        {
            Console.WriteLine($"{langValue.mailSuccessMessage}");
            Console.WriteLine($"\nThank you for using {devData[0]}!");
            Console.WriteLine("Weather data powered by openweathermap.org");
            Console.WriteLine($"Mail powered by htmlemail.io & {config.senderMail.Split('@')[1]}");
            Console.WriteLine($"{devData[0]} by {devData[1]}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
