using System.Net;
using System.Text;

namespace TGBotCSharp
{
    static class Requester
    {
        static public string Request(string url)
        {
            WebClient webClient = new()
            {
                Encoding = Encoding.UTF8
            };

            return webClient.DownloadString(url);
        }
    }
}
