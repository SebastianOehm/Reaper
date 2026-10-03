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
            if (!Reaper.GlobalVars.appToCulture.TryGetValue(selectedDisplay, out string? cultureName))
            {
                cultureName = "en";
            }

            if (!Reaper.GlobalVars.appToApiCode.TryGetValue(selectedDisplay, out string? apiCode))
            {
                apiCode = "en";
            }

            CultureInfo culture = CultureInfo.InvariantCulture;
            try { if (!string.IsNullOrEmpty(cultureName)) culture = new CultureInfo(cultureName); }
            catch { culture = CultureInfo.InvariantCulture; }

            return (apiCode, culture);
        }
    }
}
