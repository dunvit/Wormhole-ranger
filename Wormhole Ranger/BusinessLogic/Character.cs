using log4net;
using Wormhole_Ranger.Tools;

namespace Wormhole_Ranger.BusinessLogic
{
    public class Character
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(Character));

        public string Name { get; set; }

        public string Id { get; set; }

        public string Token { get; set; }

        public void ReInitialization()
        {
            Log.DebugFormat("Starting for id = {0} refreshToken = {1}", Id, Token);

            EsiApi EsiData = new EsiApi(ApplicationSettings.Authorization.ClientId, ApplicationSettings.Authorization.ClientSecret);

            EsiData.Refresh(Token);

            dynamic data = EsiData.ObtainingCharacterData();

            Id = data.CharacterID;
            Name = data.CharacterName;

            //LoadLocationInfo();

            //LoadCharacterInfo();

            //_lastTokenUpdate = DateTime.Now;
        }
    }
}
