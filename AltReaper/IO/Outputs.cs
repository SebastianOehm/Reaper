using System.Globalization;
using System.Net;
using System.Net.Mail;
using static System.Console;

namespace Reaper
{
    internal class Outputs
    {
        public static String[] WeatherOutput(WeatherResponse.root weatherData, String unitPreference, JsonHandling.langVal langValue)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
            //time & timezones, units
            char unitSymbol;
            DateTime localSystemTime = DateTime.Now;
            int timeZoneShiftFromUTC = weatherData.timezone / 3600;
            string timezoneUTC;
            DateTime locTime = DateTime.UtcNow.AddHours(timeZoneShiftFromUTC);
            timezoneUTC = timeZoneShiftFromUTC >= 0 ? $"UTC+{timeZoneShiftFromUTC}" : $"UTC{timeZoneShiftFromUTC}" ;
            unitSymbol = unitPreference == "metric" ? 'c' : 'f';

            //main output
            List<String> content = new();
            string spacer = "\n-------------------------------------\n";
            content.Add(spacer);
            content.Add($"{langValue.theWeatherIn}: {weatherData.name}, {weatherData.sys.country}");
            content.Add($"{langValue.localSystemTime}: {localSystemTime}");
            content.Add($"{langValue.timeAtDestination}: : {locTime} {timezoneUTC} ");
            content.Add($"{langValue.temp}: {weatherData.main.temp:0.#}°{unitSymbol}");
            content.Add($"{langValue.lowestTemp}: {weatherData.main.temp_min:0.#}°{unitSymbol}");
            content.Add($"{langValue.highestTemp}: {weatherData.main.temp_max:0.#}°{unitSymbol}");
            content.Add($"{langValue.description}: {weatherData.weather[0].description}");
            content.Add(spacer);
            string[] cArray = content.ToArray();
            WriteLine(String.Join("\r\n", cArray));
            WriteLine(langValue.pressEnterContinue);
            while (ReadKey(true).Key != ConsoleKey.Enter) { continue; }
            return cArray;
        }
        public static bool MailOutput(String recipient, String subjectLine, String[] content, JsonHandling.langVal langValue, JsonHandling.config config)
        {
            //Set salutation
            Write($"\n{langValue.nameOr}\n>");
            CursorVisible = true;
            ForegroundColor = ConsoleColor.White;
            string name = ReadLine();
            if (name == langValue.no) { name = ""; }
            ForegroundColor = ConsoleColor.Green;
            CursorVisible = false;
            //Set smtp config
            var smtpClient = new SmtpClient(config.hostDomain, int.Parse(config.portNumber))
            {
                Credentials = new NetworkCredential(config.senderMail, config.senderMailPassword),
                EnableSsl = true,
            };

            //Set smtp content
            string easterEgg = "https://bit.ly/3Gpgiyh";
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(config.senderMail),
                Priority = MailPriority.Low,
                Subject = subjectLine,
                IsBodyHtml = true,
                Body = HtmlBody.getBody(content,easterEgg,name,config)
            };

            //Set recipient
            mailMessage.To.Add(recipient);

            //Set bcc for analysation/archivating usage
            if (config.bcc == langValue.no) { }
            else { mailMessage.Bcc.Add(config.bcc); }

            //sending
            try { smtpClient.Send(mailMessage); }
            catch
            {
                Exception mail = new Exception();
                WriteLine(mail.Message);
                return false;
            }
            return true;
        }
    }
}
