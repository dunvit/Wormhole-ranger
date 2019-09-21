using log4net;
using System;
using System.Threading.Tasks;
using System.Timers;
using Wormhole_Ranger.Tools;

namespace Wormhole_Ranger.BusinessLogic
{
    public class Character
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(Character));

        const double interval = 10000;

        public string Name { get; set; }

        public string Id { get; set; }

        public string Token { get; set; }

        public EsiApi EsiData { get; set; }

        public Character(string id, string refreshToken)
        {
            Id = id;
            Token = refreshToken;

            ReInitialization();

            Scheduler.IntervalInSeconds(11, 10, 15,
            () => {
                RefreshPilotInfo();
            });

        }

        public void ReInitialization()
        {
            Log.DebugFormat("Starting for id = {0} refreshToken = {1}", Id, Token);

            EsiData = new EsiApi(ApplicationSettings.Authorization.ClientId, ApplicationSettings.Authorization.ClientSecret);

            EsiData.Refresh(Token);


            //LoadLocationInfo();

            //LoadCharacterInfo();

            //_lastTokenUpdate = DateTime.Now;
        }

        private void Event_Refresh(object sender, ElapsedEventArgs e)
        {
            RefreshPilotInfo();
        }

        private void RefreshPilotInfo()
        {
            Task.Run(() =>
            {
                RefreshInfo();
            });
        }

        private void RefreshInfo()
        {
            Log.DebugFormat("[Pilot.RefreshInfo] starting for Id = {0}", Id);

            //var span = DateTime.Now - _lastTokenUpdate;
            //var ms = (int)span.TotalMilliseconds;

            //if (ms > EsiData.ExpiresIn * 1000 - 20000)
            //{
            //    EsiData.Refresh();

            //    _lastTokenUpdate = DateTime.Now;

            //    Log.DebugFormat("[Pilot.RefreshInfo] set LastTokenUpdate for Id = {0}", Id);
            //}

            //if (_isBusy == false)
            //{
            //    Log.DebugFormat("[Pilot '{0}'] Load location info.", Name);
            //    LoadLocationInfo();
            //}
        }
    }
}
