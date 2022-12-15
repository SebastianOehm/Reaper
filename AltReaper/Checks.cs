using System.Globalization;
using System.Net.NetworkInformation;
using System.Net;
using Renci.SshNet.Common;
using System.Security.Cryptography.X509Certificates;

namespace Reaper
{
    internal class Checks
    {
        public static bool cfgChecker(JsonHandling.config config)
        {
            bool[] bools = {
                String.IsNullOrEmpty(config.apiKey),
                String.IsNullOrEmpty(config.senderMail),
                String.IsNullOrEmpty(config.senderMailPassword),
                String.IsNullOrEmpty(config.hostDomain),
                String.IsNullOrEmpty(config.portNumber),
                false,
                String.IsNullOrEmpty(config.bcc)
            };
            try
            {
                if (int.Parse(config.portNumber) < 0 && int.Parse(config.portNumber) > 65535)
                { bools[5] = true; }
            }
            catch { return true; }
            bool problem = false;

            // if all values of bools are true
            if (bools.Any(x => x) == true) { problem = true; }
            return problem;
        }
        public static void DeviceIsOnline(String[] devData)
        {
            bool isOnline = false;
            Ping cloudflarePing = new();
            Ping googlePing = new();
            PingReply cloudflareReply = cloudflarePing.Send("1.1.1.1");
            PingReply googleReply = googlePing.Send("8.8.8.8");
            if (cloudflareReply.Status == IPStatus.Success | googleReply.Status == IPStatus.Success)
            {
                isOnline = true;
            }
            if (isOnline == true) { Console.WriteLine("Device is online"); }
            else
            {
                Console.WriteLine(@"      
Your device is not connected to the internet.
This application needs internet access.
Please connect your device to the internet to use this application.");
                Helper.Closer(devData);
            }
        }
        public static void APIisOnline(String[] devData)
    {
            //force internal output to be english
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
                if (e.Message.Equals("The remote server returned an error: (401) Unauthorized.", StringComparison.InvariantCultureIgnoreCase)) { isOnline = true; }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nThe following Exception was raised : {0}", e.Message);
            }
            if (isOnline == true) { Console.WriteLine("API is online"); }
            else { Console.WriteLine("API is not online. Please try again later."); Helper.Closer(devData); }
        }
    }
    class Menu
    {
        private int Index;
        private String[] Options;
        private string InputPrompt;
        public Menu(string inputPrompt, string[] options)
        {
            InputPrompt = inputPrompt;
            Options = options;
            Index = 0;
        }
        public void DisplayAvailableOptions()
        {
            Console.WriteLine(InputPrompt);
            for (int counter = 0; counter < Options.Length; counter++)
            {
                string currentOption = Options[counter];
                string prefix;
                if(counter == Index)
                {
                    prefix = ">";
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    prefix = " ";
                    Console.BackgroundColor= ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($"{prefix} {currentOption}"); //fill some pretty stuff in later

            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
        }
        public int IRExcecute()
        {
            ConsoleKey pressedKey;
            do
            {
                Console.Clear();
                DisplayAvailableOptions();
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                pressedKey = keyInfo.Key;

                if (pressedKey == ConsoleKey.UpArrow ^ pressedKey == ConsoleKey.W)
                {
                    Index--;
                    if (Index == -1)
                    {
                        Index = Options.Length - 1;
                    }
                }
                else if (pressedKey == ConsoleKey.DownArrow ^ pressedKey == ConsoleKey.S)
                {
                    Index++;
                    if (Index == Options.Length)
                    {
                        Index = 0;
                    }
                }                
            } while (pressedKey != ConsoleKey.Enter);
            return Index;
        }
        public String SRExcecute()
        {
            ConsoleKey pressedKey;
            do
            {
                Console.Clear();
                DisplayAvailableOptions();
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                pressedKey = keyInfo.Key;

                if (pressedKey == ConsoleKey.UpArrow ^ pressedKey == ConsoleKey.W)
                {
                    Index--;
                    if (Index == -1)
                    {
                        Index = Options.Length - 1;
                    }
                }
                else if (pressedKey == ConsoleKey.DownArrow ^ pressedKey == ConsoleKey.S)
                {
                    Index++;
                    if (Index == Options.Length)
                    {
                        Index = 0;
                    }
                }
            } while (pressedKey != ConsoleKey.Enter);
            return Options[Index];
        }
    }
} 
