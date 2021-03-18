using System.IO;
using System.Net;
using System.Text;
<<<<<<< HEAD
=======
using Telegram.Bot;
using Telegram.Bot.Args;
>>>>>>> 425a1fe8a763399573a3c2be4e12826abbd3a692
using Telegram.Bot.Types;

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

        //Example using: Requester.CopyMessage(m.Message, {int:id to send}, bs.TokenString);
        static public void CopyMessage(Message m, int chatId, string token)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"https://api.telegram.org/bot{token}/copyMessage");
            req.ContentType = "application/json";
            req.Method = "POST";

            using (StreamWriter sw = new(req.GetRequestStream()))
            {
                var from_chat_id = m.From.Id;
                var message_id = m.MessageId;

                string json = $"{{\"chat_id\": {chatId}, \"from_chat_id\": {from_chat_id}, \"message_id\": {message_id}}}";
                sw.Write(json);
           }

            req.GetResponse();
        }
    }
}
