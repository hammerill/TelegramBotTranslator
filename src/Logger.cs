using System;
using System.IO;
using Telegram.Bot.Args;

namespace TGBotCSharp
{
    static class Logger
    {
        static public string LogFileLocation { get; set; }
        static public string DevLogFileLocation { get; set; }

        static private void Log(string toLog, bool isAdvanced)
        {
            string logString = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ff - ") + toLog;

            StreamWriter sw = new(DevLogFileLocation, true);
            sw.WriteLine(logString);

            if (!isAdvanced)
            {
                Console.WriteLine(logString);

                sw.Close();
                sw = new(LogFileLocation, true);
                sw.WriteLine(logString);
            }

            sw.Close();
        }
        static public void Got(MessageEventArgs e)
        {
            Log($"{e.Message.From.Id}:{e.Message.From.FirstName}:\t\t\"{e.Message.Text}\".", false);
        }
        static public void Sent(MessageEventArgs e, string msgText)
        {
            Log($"Bot to {e.Message.From.Id}:{e.Message.From.FirstName}:\t\"{msgText}\".\n\n", false);
        }
        static public void Requesting(string url)
        {
            Log($"Sending request to {{\n{url}\n}}.", true);
        }
        static public void GotResponse(string result)
        {
            Log($"Got {{\n{result}\n}}. Parsing this...", true);
        }
        static public void Err(int errType = 0, int userId = 0, string info = "")
        {
            switch (errType)
            {
                case 0:
                    Log("Error occured while parsing, returning null string.", false);
                    break;
                case 1:
                    Log($"Error ({info}) occured while reading user {userId} at DB, returning null.", true);
                    break;
                case 2:
                    Log($"Cannot find user {userId} in database, returning null.", true);
                    break;
                case 3:
                    Log($"Cannot find user {userId} in UserParams list, returning null.", true);
                    break;
                case 4:
                    Log($"\"{info}\" is a unknown command, sending error sticker.", false);
                    break;
                default:
                    break;
            }
        }
        static public void AddUpdateUser(int userId, bool isFromEnglish, bool isAdding)
        {
            if (isAdding)
            {
                Log($"{userId} is a new user, adding him to DB with parameter isFromEnglish = {isFromEnglish}...", true);
            }
            else
            {
                Log($"{userId} is a existing user, updating isFromEnglish to {isFromEnglish} at the DB...", true);
            }
        }
        static public void Started()
        {
            Log($"Bot started.\n", false);
        }
        static public void LangChange(MessageEventArgs e, bool isFromEnglish)
        {
            if (isFromEnglish)
            {
                Log($"{e.Message.From.Id}:{e.Message.From.FirstName} switching language to \"English\" -> \"Russian\".", false);
            }
            else
            {
                Log($"{e.Message.From.Id}:{e.Message.From.FirstName} switching language to \"Russian\" -> \"English\".", false);
            }
        }
        static public void FoundUser(int userId, bool isFromDb)
        {
            if (isFromDb)
            {
                Log($"Found a {userId} in database.", true);
            }
            else
            {
                Log($"Found a {userId} in UserParams list.", true);
            }
        }
        static public void Replacing(int userId, bool isFromEnglish)
        {
            Log($"Replacing {userId} in UserParams list with new element, where isFromEnglish = {isFromEnglish}.", true);
        }
    }
}
