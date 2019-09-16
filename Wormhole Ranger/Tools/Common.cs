using log4net;
using System.IO;
using System.Net;
using System.Text;

namespace Wormhole_Ranger.Tools
{
    public class Common
    {
        public static string ReadFile(string urlAddress, ILog log)
        {
            var request = (HttpWebRequest)WebRequest.Create(urlAddress);
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                log.ErrorFormat("[UiTools.ReadFile] Read configuration file {0} is failed. Status = {1} ", urlAddress, response.StatusCode);
                return null;
            }

            var receiveStream = response.GetResponseStream();

            var readStream = response.CharacterSet == null ? new StreamReader(receiveStream) : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

            var data = readStream.ReadToEnd();

            response.Close();
            readStream.Close();

            return data;
        }
    }
}
