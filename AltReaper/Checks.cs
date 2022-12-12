using System.Globalization;
using System.Net.NetworkInformation;
using System.Net;

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
        public static bool DeviceIsOnline()
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

            return isOnline;
        }
        public static bool APIsOnline()
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
            return isOnline;
        }
    }
}
