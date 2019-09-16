using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;

namespace Wormhole_Ranger.Tools
{
    public class EsiApi
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EsiApi));

        private string CLIENT_ID = "";
        private string CLIENT_SECRET = "";

        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }

        public EsiApi(string clientID, string clientSecret)
        {
            Log.DebugFormat("[EsiApi.EsiApi] started for clientID = {0} and clientSecret = {1}", clientID, clientSecret);

            CLIENT_ID = clientID;
            CLIENT_SECRET = clientSecret;
        }

        public void Authorization(string token)
        {
            Log.DebugFormat("[EsiApi.Authorization] started for token = {0}", token);

            VerifyAuthorizationCode(token);

            Refresh();
        }

        private void VerifyAuthorizationCode(string token)
        {
            Log.DebugFormat("[EsiApi.VerifyAuthorizationCode] started for token = {0}", token);

            var url = "https://login.eveonline.com/oauth/token";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(CLIENT_ID + ":" + CLIENT_SECRET));

            Log.DebugFormat("[EsiApi.VerifyAuthorizationCode] encoded is {0}", encoded);

            httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Host = "login.eveonline.com";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"grant_type\":\"authorization_code\",\"code\":\"" + token + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.VerifyAuthorizationCode] result = {0}", result);

                    dynamic data = JObject.Parse(result);

                    AccessToken = data.access_token;
                    RefreshToken = data.refresh_token;
                    TokenType = data.token_type;
                    ExpiresIn = data.expires_in;

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[EsiApi.VerifyAuthorizationCode] Critical error. Exception is {0}", ex);
            }

        }

        public void Refresh(string refreshToken)
        {
            RefreshToken = refreshToken;
            Refresh();
        }

        public void Refresh()
        {
            Log.DebugFormat("[EsiApi.Refresh] started for refresh_token = {0}", RefreshToken);

            var url = "https://login.eveonline.com/oauth/token";

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(CLIENT_ID + ":" + CLIENT_SECRET));
                httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
                httpWebRequest.Host = "login.eveonline.com";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"grant_type\":\"refresh_token\",\"refresh_token\":\"" + RefreshToken + "\"}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.Refresh] result = {0}", result);

                    dynamic data = JObject.Parse(result);

                    AccessToken = data.access_token;
                    RefreshToken = data.refresh_token;
                    TokenType = data.token_type;
                    ExpiresIn = data.expires_in;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.Refresh] Exception is {0}", ex);
            }

        }

        public void SetWaypoint(string addToBeginning, string clearOtherWaypoints, string solarSystemId)
        {
            Log.DebugFormat("[EsiApi.SetWaypoint] started for refresh_token = {0}", AccessToken);

            var url = ApplicationSettings.Common.EsiAddress + "/latest/ui/autopilot/waypoint/?add_to_beginning=false&clear_other_waypoints=" + clearOtherWaypoints + "&datasource=tranquility&destination_id=" + solarSystemId;

            try
            {

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    using (HttpWebResponse objResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        // do something...
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.SetWaypoint] Exception is {0}", ex);
            }

        }

        public dynamic ObtainingCharacterData()
        {
            Log.DebugFormat("[EsiApi.ObtainingCharacterData] AccessToken = {0}", AccessToken);

            var url = "https://login.eveonline.com/oauth/verify";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
            httpWebRequest.Host = "login.eveonline.com";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                Log.DebugFormat("[EsiApi.ObtainingCharacterData] result = {0}", result);

                return JObject.Parse(result);

            }

        }

        public dynamic GetSolarSystemInfo(string systemId)
        {
            Log.DebugFormat("[EsiApi.GetSolarSystemInfo] started. systemId = {0}", systemId);

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/latest/universe/systems/" + systemId + "/";

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + " Start Get solar system. " + url);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.GetSolarSystemInfo] result = {0}", result);

                    return JObject.Parse(result);

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetSolarSystemInfo] systemId = {1} Exception is {0}", ex, systemId);
                return null;
            }
        }

        public dynamic GetConstellationInfo(string id)
        {
            Log.DebugFormat("[EsiApi.GetConstellationInfo] started. systemId = {0}", id);

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/latest/universe/constellations/" + id + "/";

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + " Start Constellation. " + url);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.GetConstellationInfo] result = {0}", result);

                    return JObject.Parse(result);

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetConstellationInfo] systemId = {1} Exception is {0}", ex, id);
                return null;
            }
        }

        public dynamic GetRegionInfo(string id)
        {
            Log.DebugFormat("[EsiApi.GetConstellGetRegionInfoationInfo] started. systemId = {0}", id);

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/latest/universe/regions/" + id + "/";

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + " Start Region. " + url);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.GetRegionInfo] result = {0}", result);

                    return JObject.Parse(result);

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetRegionInfo] systemId = {1} Exception is {0}", ex, id);
                return null;
            }
        }

        public string GetSolarSystemId(string name)
        {
            Log.DebugFormat("[EsiApi.GetSolarSystemId] started. name = {0}", name);

            var url = "";
            var solarSystemId = "";

            try
            {
                url = ApplicationSettings.Common.EsiAddress + "/latest/search/?search=" + WebUtility.UrlEncode(name) + "&categories=solar_system&language=en-us&strict=true&datasource=tranquility";

                Log.DebugFormat("[EsiApi.GetSolarSystemId] Read url {0} ", url);

                var data = Common.ReadFile(url, Log);

                solarSystemId = data.Split(new[] { "[" }, StringSplitOptions.None)[1].Split(new[] { "]" }, StringSplitOptions.None)[0];

                return solarSystemId;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("[Pilot.GetSolarSystemId] Read url {0} is failed. Exception = {1} ", url, e);
                return solarSystemId;
            }
        }

        public dynamic GetCorporationInfo(string corporationId)
        {
            Log.DebugFormat("[EsiApi.GetCorporationInfo] started. systemId = {0}", corporationId);

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/latest/corporations/names/?corporation_ids=" + corporationId + "&datasource=tranquility";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    result = result.Substring(1, result.Length - 2);

                    Log.DebugFormat("[EsiApi.GetCorporationInfo] result = {0}", result);

                    return JObject.Parse(result);

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetCorporationInfo] systemId = {1} Exception is {0}", ex, corporationId);
                return null;
            }
        }

        public dynamic GetLocation(long pilotId)
        {
            Log.DebugFormat("[EsiApi.GetLocation] started. pilotId = {0}", pilotId);

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/latest/characters/" + pilotId + "/location/";

                Trace.TraceInformation(DateTime.Now.ToLongTimeString() + " Start Get location. " + url);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
                httpWebRequest.ContentType = "application/json";
                //httpWebRequest.Host = "crest-tq.eveonline.com";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.GetLocation] result = {0}", result);

                    return JObject.Parse(result);

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetLocation] Exception is {0}", ex);
                return null;
            }


        }

        public List<string> GetStargates(int systemId)
        {
            Log.DebugFormat("[EsiApi.GetStargates] started. systemId = {0}", systemId);

            var stargates = new List<string>();

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/latest/universe/systems/" + systemId + "/";

                Log.DebugFormat(DateTime.Now.ToLongTimeString() + " Start Get solar system. " + url);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.GetStargates] result = {0}", result);

                    dynamic data = JObject.Parse(result);


                    var dynamicStargates = data.stargates;

                    foreach (var content in dynamicStargates)
                    {
                        var locationId = content.ToString();

                        stargates.Add(locationId);
                    }

                    return stargates;

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetStargates] systemId = {1} Exception is {0}", ex, systemId);
                return null;
            }
        }


        

        public dynamic GetCharacterInfo(long pilotId)
        {
            Log.DebugFormat("[CrestAuthorization.GetCharacterInfo] started. pilotId = {0}", pilotId);


            var url = ApplicationSettings.Common.EsiAddress + "/v1/characters/" + pilotId + "/portrait/";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
            httpWebRequest.ContentType = "application/json";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                Log.DebugFormat("[CrestAuthorization.GetCharacterInfo] result = {0}", result);

                return JObject.Parse(result);

            }
        }


        public string GetSolarSystemIdByStargate(string stargateId)
        {
            Log.DebugFormat("[EsiApi.GetSolarSystemIdByStargate] started. stargateId = {0}", stargateId);

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/latest/universe/stargates/" + stargateId + "/";

                Log.DebugFormat("[EsiApi.GetSolarSystemIdByStargate] Start Get solar system. " + url);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    Log.DebugFormat("[EsiApi.GetSolarSystemIdByStargate] result = {0}", result);

                    dynamic data = JObject.Parse(result);

                    var solarSystemId = data.destination.system_id;

                    return solarSystemId.ToString();

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetSolarSystemIdByStargate] stargateId = {1} Exception is {0}", ex, stargateId);
                return null;
            }
        }


        public static Tuple<string, string, string> GetSystemKills(string systemId)
        {
            Log.DebugFormat("[EsiApi.GetSystemKills] started. systemId = {0} ", systemId);

            try
            {
                var url = ApplicationSettings.Common.EsiAddress + "/v2/universe/system_kills/";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    foreach (var content in JArray.Parse(result))
                    {
                        var system_id = content.SelectToken("system_id").ToString();
                        var ship_kills = content.SelectToken("ship_kills").ToString();
                        var npc_kills = content.SelectToken("npc_kills").ToString();
                        var pod_kills = content.SelectToken("pod_kills").ToString();

                        if (systemId == system_id)
                        {
                            return new Tuple<string, string, string>(ship_kills, npc_kills, pod_kills);
                        }

                    }

                    Log.DebugFormat("[EsiApi.GetSystemKills] result = {0}", result);

                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Critical error in [EsiApi.GetSystemKills] Exception is {0}", ex);
                return null;
            }


        }

    }
}
