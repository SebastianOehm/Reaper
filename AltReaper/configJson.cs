
namespace Reaper
{
    internal class configJson
    {
        public class root
        {
            public string apiKey { get; set; }
            public string senderMail { get; set; }

            public string senderMailPassword { get; set; }
            public string hostDomain { get; set; }
            public int portNumber { get; set; }
            public string bcc { get; set; }
        }
    }
}
