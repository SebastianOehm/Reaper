
namespace Reaper
{
    internal class Helper
    {
        public static void MailOption(String[] langValue, String[] config, String[] content, String cfgLoc)
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
                        Inputs.configGen(cfgLoc, cfgOptCount, langValue, config);
                        config = File.ReadAllLines(cfgLoc);
                        continue;
                    }
                    Console.Write($"\n{langValue[19]}\n>");
                    string recipient = Console.ReadLine();
                    if (Outputs.MailOutput(recipient, langValue[15], content, langValue, config))
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
        public static string PasswordMaker()
        {
            string password = null;
            while (true)
            {
                ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                if (keyPressed.Key == ConsoleKey.Enter) { break; }

                else if (keyPressed.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (keyPressed.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    password.Append(keyPressed.KeyChar);
                    Console.Write("*");
                }
            }
            return password;
        }
    }
}
