using System.Net;
using System.Net.Mail;

namespace Reaper
{
    internal class Outputs
    {
        public static String[] WeatherOutput(JsonResponseDeserializer.root wetterDaten, String unitPreference, String[] langValue)
        {
            //declare vars for cleaner output messages
            double tempValue = wetterDaten.main.temp;
            double temp_minValue = wetterDaten.main.temp_min;
            double temp_maxValue = wetterDaten.main.temp_max;
            string city = wetterDaten.name;
            string countryShort = wetterDaten.sys.country;
            char unitSymbol;
            string weatherDescription = wetterDaten.weather[0].description;

            //time & timezones
            string? relativeTimeWord = null;
            DateTime localSystemTime = DateTime.Now;
            int timeZoneShiftFromUTC = wetterDaten.timezone / 3600;
            string timezoneUTC = null;
            DateTime locTime = DateTime.UtcNow.AddHours(timeZoneShiftFromUTC);
            //var lastMeasured = DateTimeOffset.FromUnixTimeSeconds(wetterDaten.dt).DateTime.AddHours(timeZoneShiftFromUTC);
            int? timeDifference = localSystemTime.Hour - locTime.Hour;
            if (timeZoneShiftFromUTC >= 0) { timezoneUTC = $"UTC+{timeZoneShiftFromUTC}"; } else { timezoneUTC = $"UTC{timeZoneShiftFromUTC}"; }
            if (localSystemTime > locTime) { relativeTimeWord = "behind"; } else { relativeTimeWord = "ahead"; timeDifference = timeDifference * -1; } // replace with langValue index later

            //metric/imperial output
            if (unitPreference == "metric") { unitSymbol = 'C'; } else { unitSymbol = 'F'; }

            //main output
            string cs = "\n-------------------------------------\n";
            string c1 = $"{langValue[8]}: {wetterDaten.name}, {countryShort}";
            string c2 = $"{langValue[13]}: {localSystemTime}";
            string c3 = $"{langValue[14]}: : {locTime} {timezoneUTC} ";
            //Console.WriteLine($"Last measured: {lastMeasured}");
            string c4 = $"{wetterDaten.name} is {timeDifference}h {relativeTimeWord}.";
            string c5 = $"{langValue[9]}: {tempValue:0.#}°{unitSymbol}";
            string c6 = $"{langValue[10]}: {temp_minValue:0.#}°{unitSymbol}";
            string c7 = $"{langValue[11]}: {temp_maxValue:0.#}°{unitSymbol}";
            string c8 = $"{langValue[12]}: {weatherDescription}";
            string[] cArray = { cs, c1, c2, c3, c4, c5, c6, c7, c8, cs };
            Console.WriteLine(String.Join("\r\n", cArray));
            return cArray;
        }
        public static bool MailOutput(String recipient, String subjectLine, String[] content, String[] langValue, String sender, String pwd, String host, int port, String archiveBcc)
        {
            //salutation
            Console.Write($"\n{langValue[20]}\n>");
            string name = Console.ReadLine();
            if (name == langValue[17]) { name = ""; }

            //Smtp config
            var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(sender, pwd),
                EnableSsl = true,
            };

            //Set Smtp content
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(sender),
                Subject = subjectLine,
                IsBodyHtml = true,
                Body = $"<!doctype html>\r\n<html>\r\n<head>\r\n<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n<title>Simple Weather Information Email</title>\r\n<style>\r\n@media only screen and (max-width: 620px) {{\r\n  table.body h1 {{\r\nfont-size: 28px !important;\r\nmargin-bottom: 10px !important;\r\n  }}\r\n\r\ntable.body p,\r\ntable.body ul,\r\ntable.body ol,\r\ntable.body td,\r\ntable.body span,\r\ntable.body a {{\r\n font-size: 16px !important;\r\n  }}\r\n\r\n  table.body .wrapper,\r\ntable.body .article }}{{\r\n    padding: 10px !important;\r\n  }}\r\n\r\n  table.body .content {{\r\n    padding: 0 !important;\r\n  }}\r\n\r\n  table.body .container {{\r\n    padding: 0 !important;\r\n    width: 100% !important;\r\n  }}\r\n\r\n  table.body .main {{\r\n    border-left-width: 0 !important;\r\n    border-radius: 0 !important;\r\n    border-right-width: 0 !important;\r\n  }}\r\n\r\n  table.body .btn table {{\r\n    width: 100% !important;\r\n  }}\r\n\r\n  table.body .btn a {{\r\n    width: 100% !important;\r\n  }}\r\n\r\n  table.body .img-responsive {{\r\n    height: auto !important;\r\n    max-width: 100% !important;\r\n    width: auto !important;\r\n}}\r\n}}\r\n@media all{{\r\n.ExternalClass {{\r\nwidth: 100%;\r\n}}\r\n\r\n.ExternalClass,\r\n.ExternalClass p,\r\n.ExternalClass span,\r\n.ExternalClass font,\r\n.ExternalClass td,\r\n.ExternalClass div {{\r\n    line-height: 100%;\r\n  }}\r\n\r\n  #MessageViewBody a {{\r\n    color: inherit;\r\n    text-decoration: none;\r\n    font-size: inherit;\r\n    font-family: inherit;\r\n    font-weight: inherit;\r\n    line-height: inherit;\r\n  }}\r\n}}\r\n</style>\r\n  </head>\r\n  <body style=\"background-color: #f6f6f6; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">\r\n<table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"body\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f6f6f6; width: 100%;\" width=\"100%\" bgcolor=\"#f6f6f6\">\r\n<tr>\r\n<td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top;\" valign=\"top\">&nbsp;</td>\r\n<td class=\"container\" style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; max-width: 580px; padding-top: 10px; width: 580px; margin: 0 auto;\" width=\"580\" valign=\"top\">\r\n<div class=\"content\" style=\"box-sizing: border-box; display: block; margin: 0 auto; max-width: 580px; padding-top: 10px;\">\r\n\r\n <!-- START CENTERED WHITE CONTAINER -->\r\n<table role=\"presentation\" class=\"main\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background: #ffffff; border-radius: 3px; width: 100%;\" width=\"100%\">\r\n\r\n<!-- START MAIN CONTENT AREA -->\r\n<tr>\r\n<td class=\"wrapper\" style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box; padding: 20px;\" valign=\"top\">\r\n<table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;\" width=\"100%\">\r\n<tr>\r\n<td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top;\" valign=\"top\">\r\n<p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Hello {name},</p>\r\n\n  <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Here is your weather info:<br>\n{String.Join("\n<br>", content)}</p>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">This is an automated mail, please no do not reply.</p>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n\r\n<!-- END MAIN CONTENT AREA -->\r\n</table>\r\n<!-- END CENTERED WHITE CONTAINER -->\r\n\r\n<!-- START FOOTER -->\r\n<div class=\"footer\" style=\"clear: both; text-align: center; width: 100%;\">\r\n<table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;\" width=\"100%\">\r\n<tr>\r\n<td class=\"content-block\" style=\"font-family: sans-serif; vertical-align: color: #999999; font-size: 12px; text-align: center;\" valign=\"top\" align=\"center\">\r\n<br> Don't like these emails? <a href=\"https://www.youtube.com/watch?v=8ybW48rKBME\" style=\"text-decoration: underline; color: #999999; font-size: 12px; text-align: center;\">Unsubscribe</a>.\r\n </td>\r\n  </tr>\r\n <tr>\r\n  <td class=\"content-block powered-by\" style=\"font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; color: #999999; font-size: 12px; text-align: center;\" valign=\"top\" align=\"center\">\r\n  Powered by <a href=\"http://htmlemail.io \" style=\"color: #999999; font-size: 12px; text-align: center; text-decoration: none;\">HTMLemail</a> & <a href=\"http://WetterSense.de \" style=\"color: #999999; font-size: 12px; text-align: center; text-decoration: none;\">WetterSense</a>.\r\n </td>\r\n  </tr>\r\n  </table>\r\n  </div>\r\n <!-- END FOOTER -->\r\n\r\n</div>\r\n</td>\r\n <td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top;\" valign=\"top\">&nbsp;</td>\r\n</tr>\r\n</table>\r\n</body>\r\n</html>",            };
            
            //Set Recipient
            mailMessage.To.Add(recipient);
            //Set bcc for analysis
            if(archiveBcc == langValue[17]) { } 
            else 
            {
                mailMessage.Bcc.Add(archiveBcc); 
            }

            //sending
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch
            {
                Exception mail = new Exception();
                Console.WriteLine(mail);
                return false;
            }
            return true;
        }
    }
}
