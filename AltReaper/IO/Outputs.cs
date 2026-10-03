using System.Net;
using System.Net.Mail;
using static System.Console;

namespace Reaper.IO
{
    internal class Outputs
    {
        public static string[] WeatherOutput(WeatherResponse.Root weatherData, string unitPreference)
        {
            //time & timezones, units
            char unitSymbol;
            DateTime localSystemTime = DateTime.Now;
            int timeZoneShiftFromUTC = weatherData.Timezone / 3600;
            string timezoneUTC;
            DateTime locTime = DateTime.UtcNow.AddHours(timeZoneShiftFromUTC);
            timezoneUTC = timeZoneShiftFromUTC >= 0 ? $"UTC+{timeZoneShiftFromUTC}" : $"UTC{timeZoneShiftFromUTC}" ;
            unitSymbol = unitPreference == "metric" ? 'c' : 'f';

            //main output
            List<string> content = [];
            string spacer = "\n-------------------------------------\n";
            content.Add(spacer);
            content.Add($"{Properties.Resources.theWeatherIn}: {weatherData.Name}, {weatherData.Sys.Country}");
            content.Add($"{Properties.Resources.localSystemTime}: {localSystemTime}");
            content.Add($"{Properties.Resources.timeAtDestination}: : {locTime} {timezoneUTC} ");
            content.Add($"{Properties.Resources.temp}: {weatherData.Main.Temp:0.#}°{unitSymbol}");
            content.Add($"{Properties.Resources.lowestTemp}: {weatherData.Main.TempMin:0.#}°{unitSymbol}");
            content.Add($"{Properties.Resources.highestTemp}: {weatherData.Main.TempMax:0.#}°{unitSymbol}");
            content.Add($"{Properties.Resources.description}: {weatherData.Weather[0].Description}");
            content.Add(spacer);
            string[] cArray = [.. content];
            WriteLine(string.Join("\r\n", cArray));
            WriteLine(Properties.Resources.pressEnterContinue);
            while (ReadKey(true).Key != ConsoleKey.Enter) { continue; }
            return cArray;
        }
        public static bool MailOutput(string recipient, string subjectLine, string[] content, JsonHandling.Config config)
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
            var smtpClient = new SmtpClient(config.HostDomain, int.Parse(config.PortNumber))
            {
                Credentials = new NetworkCredential(config.SenderMail, config.SenderMailPassword),
                EnableSsl = true,
            };

            //Set smtp content
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(config.SenderMail),
                Priority = MailPriority.Low,
                Subject = subjectLine,
                IsBodyHtml = true,
                Body = HtmlBody.GetBody(content, GlobalVars.easterEgg, name, config)
            };

            //Set recipient
            mailMessage.To.Add(recipient);

            //Set bcc for analysation/archivating usage
            //empty bcc means no BCC mail. Older configs stored the localized "no" instead, so accept that of every app language
            bool noBcc = string.IsNullOrWhiteSpace(config.Bcc) || GlobalVars.appToCulture.Values.Any(c =>
                config.Bcc == Properties.Resources.ResourceManager.GetString(nameof(Properties.Resources.NoOption), new System.Globalization.CultureInfo(c)));
            if (noBcc) { }
            else { mailMessage.Bcc.Add(config.Bcc); }

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
