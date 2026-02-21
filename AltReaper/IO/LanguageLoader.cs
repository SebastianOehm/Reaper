using System.Globalization;

namespace Reaper.IO
{
    internal static class LanguageLoader
    {
        // Load language values from RESX resources for a given selected language display name
        public static (string apiCode, CultureInfo culture) Load(string selectedLanguageDisplay)
        {
            // Normalize selected display name
            string selectedDisplay = string.IsNullOrEmpty(selectedLanguageDisplay) ? "English" : selectedLanguageDisplay;

            // Determine culture name and API code using app-level mappings
            string cultureName = "en";
            if (Reaper.globalVars.appToCulture != null && Reaper.globalVars.appToCulture.ContainsKey(selectedDisplay))
            {
                cultureName = Reaper.globalVars.appToCulture[selectedDisplay];
            }

            string apiCode = "en";
            if (Reaper.globalVars.appToApiCode != null && Reaper.globalVars.appToApiCode.ContainsKey(selectedDisplay))
            {
                apiCode = Reaper.globalVars.appToApiCode[selectedDisplay];
            }

            CultureInfo culture = CultureInfo.InvariantCulture;
            try { if (!string.IsNullOrEmpty(cultureName)) culture = new CultureInfo(cultureName); }
            catch { culture = CultureInfo.InvariantCulture; }

            return (apiCode, culture);
        }
    }
}
