using Wormhole_Ranger.Tools;

namespace Wormhole_Ranger.Localization
{
    public class Messages
    {

        public static string Get(string key, string defaultValue = "None")
        {
            if (DebugTools.IsInDesignMode() == false )
            {
                return GetMessageByLanguageKey(key, ApplicationSettings.LanguageId);
            }

            return defaultValue != "None" ? defaultValue : key;
        }

        private static string GetMessageByLanguageKey(string key, int language)
        {

            switch (language)
            {
                case 0:
                    return English.ResourceManager.GetString(key);

                case 1:
                    return Russian.ResourceManager.GetString(key);

                default:
                    return English.ResourceManager.GetString(key);
            }
        }
    }
}
