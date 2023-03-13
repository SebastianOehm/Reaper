using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using static System.Console;

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
        public static void DeviceIsOnline()
        {
            Ping cloudflarePing = new();
            Ping googlePing = new();
            PingReply cloudflareReply = cloudflarePing.Send("1.1.1.1");
            PingReply googleReply = googlePing.Send("8.8.8.8");
            if (cloudflareReply.Status == IPStatus.Success | googleReply.Status == IPStatus.Success)
            {
                WriteLine("Device is online");
            } else
            {
                WriteLine(@"      
Your device is not connected to the internet.
This application needs internet access.
Please connect your device to the internet to use this application.");
                Helper.Closer();
            }
        }
        public static void APIisOnline()
        {
            //force internal output to be english
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?q=London");
                request.Proxy = defaultWebProxy;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException e)
            {
                if (e.Message.Equals("The remote server returned an error: (401) Unauthorized.", StringComparison.InvariantCultureIgnoreCase)) 
                { WriteLine("API is online"); }
                else { WriteLine("API is not online. Please try again later."); Helper.Closer(); }
            }
            catch (Exception e)
            {
                WriteLine("\nThe following Exception was raised : {0}", e.Message);
            }
        }
    }
    class Menu
    {
        private int Index;
        private string[] Options;
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
                if (counter == Index)
                {
                    prefix = ">";
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    prefix = " ";
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"{prefix} {currentOption}");
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
