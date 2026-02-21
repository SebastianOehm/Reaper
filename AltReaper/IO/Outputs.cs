using System.Net;
using System.Net.Mail;
using static System.Console;

namespace Reaper.IO
{
    internal class Outputs
    {
        public static string[] WeatherOutput(WeatherResponse.root weatherData, string unitPreference)
        {
            //time & timezones, units
            char unitSymbol;
            DateTime localSystemTime = DateTime.Now;
            int timeZoneShiftFromUTC = weatherData.timezone / 3600;
            string timezoneUTC;
            DateTime locTime = DateTime.UtcNow.AddHours(timeZoneShiftFromUTC);
            timezoneUTC = timeZoneShiftFromUTC >= 0 ? $"UTC+{timeZoneShiftFromUTC}" : $"UTC{timeZoneShiftFromUTC}" ;
            unitSymbol = unitPreference == "metric" ? 'c' : 'f';

            //main output
            List<string> content = new();
            string spacer = "\n-------------------------------------\n";
            content.Add(spacer);
            content.Add($"{Properties.Resources.theWeatherIn}: {weatherData.name}, {weatherData.sys.country}");
            content.Add($"{Properties.Resources.localSystemTime}: {localSystemTime}");
            content.Add($"{Properties.Resources.timeAtDestination}: : {locTime} {timezoneUTC} ");
            content.Add($"{Properties.Resources.temp}: {weatherData.main.temp:0.#}°{unitSymbol}");
            content.Add($"{Properties.Resources.lowestTemp}: {weatherData.main.temp_min:0.#}°{unitSymbol}");
            content.Add($"{Properties.Resources.highestTemp}: {weatherData.main.temp_max:0.#}°{unitSymbol}");
            content.Add($"{Properties.Resources.description}: {weatherData.weather[0].description}");
            content.Add(spacer);
            string[] cArray = content.ToArray();
            WriteLine(string.Join("\r\n", cArray));
            WriteLine(Properties.Resources.pressEnterContinue);
            while (ReadKey(true).Key != ConsoleKey.Enter) { continue; }
            return cArray;
        }
        public static bool MailOutput(string recipient, string subjectLine, string[] content, JsonHandling.config config)
        {
            //Set salutation
            Write($"\n{Properties.Resources.nameOr}\n>");
            CursorVisible = true;
            ForegroundColor = ConsoleColor.White;
            string? name = ReadLine();
            if (name == Properties.Resources.NoOption || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)) { name = ""; }
            ForegroundColor = ConsoleColor.Green;
            CursorVisible = false;
            //Set smtp config
            var smtpClient = new SmtpClient(config.hostDomain, int.Parse(config.portNumber))
            {
                Credentials = new NetworkCredential(config.senderMail, config.senderMailPassword),
                EnableSsl = true,
            };

            //Set smtp content
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(config.senderMail),
                Priority = MailPriority.Low,
                Subject = subjectLine,
                IsBodyHtml = true,
                Body = HtmlBody.getBody(content,globalVars.easterEgg,name,config)
            };

            //Set recipient
            mailMessage.To.Add(recipient);

            //Set bcc for analysation/archivating usage
            if (config.bcc == Properties.Resources.NoOption) { }
            else { mailMessage.Bcc.Add(config.bcc); }

            //sending
            try { smtpClient.Send(mailMessage); }
            catch (Exception mail)
            {
                WriteLine(mail.Message);
                return false;
            }
            return true;
        }
    }
}
