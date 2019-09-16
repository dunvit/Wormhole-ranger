using System;
using System.IO;
using Newtonsoft.Json;

namespace Wormhole_Ranger
{
    public static class ApplicationSettings
    {
        /// <summary>
        /// <para>0 - English</para>
        /// <para>1 - Russian</para>
        /// <para>For get values from resx in folder Localization</para>
        /// </summary>
        public static int LanguageId { get; set; } = 0;

        public static State.WindowCurrentStatus Window = new State.WindowCurrentStatus();

        public static BusinessLogic.Characters Characters = new BusinessLogic.Characters();

        public static Settings.AuthorizationSettings Authorization = new Settings.AuthorizationSettings();

        public static Settings.SignaturePatterns SignaturePatterns = new Settings.SignaturePatterns();

        public static Settings.Common Common = new Settings.Common();

        static ApplicationSettings()
        {
            Load();
        }

        public static void Load()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EveJima", "Settings.dat");

            if (File.Exists(path))
            {
                LoadFromFile(path);
            }
            else
            {
                LoadFromDefaultValues();
            }

         }

        private static void LoadFromFile(string path)
        {
            var settingsContent = File.ReadAllText(path);

            dynamic jsonResponse = JsonConvert.DeserializeObject(settingsContent);

            LanguageId = jsonResponse.LanguageId == null ? 0 : jsonResponse.LanguageId;

            Authorization.Authorization_ClientId = jsonResponse.Authorization_ClientId;
            Authorization.Authorization_ClientSecret = jsonResponse.Authorization_ClientSecret;
            Authorization.Authorization_ClientState = jsonResponse.Authorization_ClientState;
            Authorization.Authorization_Port = jsonResponse.Authorization_Port;
            Authorization.Authorization_Scopes = jsonResponse.Authorization_Scopes;

            SignaturePatterns.Gas = jsonResponse.SignaturePatternGas == null ? "Gas %ABC-%123 %NAME (%ET)" : jsonResponse.SignaturePatternGas;
            SignaturePatterns.Data = jsonResponse.SignaturePatternData == null ? "Data %ABC-%123 %NAME (%ET)" : jsonResponse.SignaturePatternData;
            SignaturePatterns.Relic = jsonResponse.SignaturePatternRelic == null ? "Relic %ABC-%123 %NAME (%ET)" : jsonResponse.SignaturePatternRelic;
            SignaturePatterns.Wormhole = jsonResponse.SignaturePatternWormhole == null ? "WH %ABC-%123 %NAME (%ET)" : jsonResponse.SignaturePatternWormhole;
            SignaturePatterns.Unknown = jsonResponse.SignaturePatternUnknown == null ? "Unknown %ABC-%123 %NAME (%ET)" : jsonResponse.SignaturePatternUnknown;

            var ccPilots = jsonResponse.Pilots;

            foreach (var ccPilot in ccPilots)
            {
                Characters.Add(new BusinessLogic.Character
                {
                    Id = ccPilot.Item2.ToString(),
                    Name = ccPilot.Item1.ToString(),
                    Token = ccPilot.Item3.ToString()
                });
            }
        }

        private static void LoadFromDefaultValues()
        {
            Authorization.Authorization_ClientId = "e136434f8a0c484ab802666f378cac09";
            Authorization.Authorization_ClientSecret = "bqbIMfDvaFfI9EPOGYmrVDeih9wPkDFnH3eW7GZY";
            Authorization.Authorization_ClientState = "bqbIMfDvaFfI9EPOGYmrVDeih9wPkDFnH3eW7GZY";
            Authorization.Authorization_Port = "8080";
            Authorization.Authorization_Scopes = "esi-location.read_location.v1 esi-location.read_ship_type.v1 esi-bookmarks.read_character_bookmarks.v1 esi-fleets.read_fleet.v1 esi-ui.open_window.v1 esi-ui.write_waypoint.v1";

            SignaturePatterns.Gas = "Gas %ABC-%123 %NAME (%ET)";
            SignaturePatterns.Data = "Data %ABC-%123 %NAME (%ET)";
            SignaturePatterns.Relic = "Relic %ABC-%123 %NAME (%ET)";
            SignaturePatterns.Wormhole = "WH %ABC-%123 %NAME (%ET)";
            SignaturePatterns.Unknown = "Unknown %ABC-%123 %NAME (%ET)";
        }
    }
}
