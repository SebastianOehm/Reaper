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
                string.IsNullOrEmpty(config.apiKey),
                string.IsNullOrEmpty(config.senderMail),
                string.IsNullOrEmpty(config.senderMailPassword),
                string.IsNullOrEmpty(config.hostDomain),
                string.IsNullOrEmpty(config.portNumber),
                false,
                string.IsNullOrEmpty(config.bcc)
            };
            try
            {
                if (int.Parse(config.portNumber) < 0 || int.Parse(config.portNumber) > 65535)
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
                WriteLine(Properties.Resources.DeviceOnline);
            } else
            {
                WriteLine(Properties.Resources.DeviceOfflineDetails);
                Helper.Closer();
            }
        }
        public static void APIisOnline()
        {
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                var handler = new HttpClientHandler
                {
                    Proxy = defaultWebProxy,
                    UseProxy = true,
                    DefaultProxyCredentials = CredentialCache.DefaultCredentials
                };

                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(10);

                HttpResponseMessage response = client.GetAsync("https://api.openweathermap.org/data/2.5/weather?q=London").GetAwaiter().GetResult();

                // Treat HTTP 401 (Unauthorized) as online (API reachable but invalid key)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    WriteLine(Properties.Resources.ApiOnline);
                    return;
                }

                // Any other HTTP response (including successful 200) is treated as offline for this check
                WriteLine(Properties.Resources.ApiOffline);
                Helper.Closer();
            }
            catch (HttpRequestException e)
            {
                // network-level error (DNS, timeout, connection failure, etc.)
                WriteLine(string.Format(Properties.Resources.NetworkErrorFormat, e.Message));
                Helper.Closer();
            }
            catch (Exception e)
            {
                WriteLine(string.Format(Properties.Resources.ExceptionRaisedFormat, e.Message));
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
                    BackgroundColor = ConsoleColor.White;
                    ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    prefix = " ";
                    BackgroundColor = ConsoleColor.Black;
                    ForegroundColor = ConsoleColor.White;
                }
                WriteLine($"{prefix} {currentOption}");
            }
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.Green;
        }
        public int IRExecute()
        {
            ConsoleKey pressedKey;
            do
            {
                Clear();
                DisplayAvailableOptions();
                ConsoleKeyInfo keyInfo = ReadKey(true);
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
        public string SRExecute()
        {
            ConsoleKey pressedKey;
            do
            {
                Clear();
                DisplayAvailableOptions();
                ConsoleKeyInfo keyInfo = ReadKey(true);
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
