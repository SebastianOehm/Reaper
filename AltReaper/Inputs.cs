using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*cfg 
 * APIKey
 * senderAddress
 * password
 * host
 * port
 * 
*/
namespace Reaper
{
    internal class Inputs
    {
        public static string[] configReader (string cfgLoc) 
        {
            string[] config = File.ReadAllLines(cfgLoc);
            return config;
        }
    }
}
