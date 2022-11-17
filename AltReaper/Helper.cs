using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reaper
{
    internal class Helper
    {
        public static void MailOption (String[] langValue, String[] config, String[] content, String cfgLoc)
        {
            int cfgOptCount = 5;
            Console.Write($"\n{langValue[18]} ({langValue[16]},{langValue[17]})\n>");
            string answer = Console.ReadLine().ToLower();
            if (answer == langValue[16])
            {
                bool partSuccess = false;
                while (!partSuccess)
                {
                    if (File.ReadAllLines(cfgLoc).Length != 6)
                    {
                        Inputs.configGen(cfgLoc,cfgOptCount,langValue);
                        config = File.ReadAllLines(cfgLoc);
                        continue;
                    }
                    Console.Write($"\n{langValue[19]}\n>");
                    string recipient = Console.ReadLine();
                    if (Outputs.MailOutput(recipient, langValue[15], content, langValue, config[1], config[2], config[3], int.Parse(config[4]), config[5]) == true)
                    {
                        Console.WriteLine($"{langValue[21]}");
                        Console.WriteLine("Weather data powered by openweathermap.org");
                        Console.WriteLine("Mail powered by htmlemail.io & WetterSense.de");
                    }
                    else { throw new Exception(); }
                    partSuccess = true;
                }
            }
            else
            {
                if (answer == langValue[17])
                {
                    Console.WriteLine("Goodbye");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
        }
    }
}
