﻿using System.IO;
using System.Net;
using System.Text;
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
        static public void CopyMessage(Message msg, int chatId, string token)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"https://api.telegram.org/bot{token}/copyMessage");
            req.ContentType = "application/json";
            req.Method = "POST";

            using (StreamWriter sw = new(req.GetRequestStream()))
            {
                var from_chat_id = msg.From.Id;
                var message_id = msg.MessageId;

                string json = $"{{\"chat_id\": {chatId}, \"from_chat_id\": {from_chat_id}, \"message_id\": {message_id}}}";
                sw.Write(json);
           }

            req.GetResponse();
        }
    }
}
