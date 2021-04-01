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
        static public void Menu(MessageEventArgs e, int menuButton, User user = null)
        {
            switch (menuButton)
            {
                case 0:
                    Log($"{e.Message.From.Id}:{e.Message.From.FirstName} requesting SrcLang change.", false);
                    break;
                case 1:
                    Log($"{e.Message.From.Id}:{e.Message.From.FirstName} requesting ToLang change.", false);
                    break;
                case 2:
                    Log($"{e.Message.From.Id}:{e.Message.From.FirstName} requesting status.", false);
                    break;
                case 3:
                    Log($"{e.Message.From.Id}:{e.Message.From.FirstName} requesting all langs.", false);
                    break;
                case 4:
                    Log($"{e.Message.From.Id}:{e.Message.From.FirstName} requesting switching langs ({user.SrcLang.LangCode} and {user.ToLang.LangCode}).", false);
                    break;
                case 5:
                    Log($"{e.Message.From.Id}:{e.Message.From.FirstName} requesting switching reverse mode (from {user.ReverseMode}).", false);
                    break;
            }
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
                    Log($"Cannot find user {userId} in Users list, returning null.", true);
                    break;
                case 4:
                    Log($"\"{info}\" is a unknown command, sending error sticker.", false);
                    break;
                default:
                    break;
            }
        }
        static public void AddUpdateUser(User user, bool isAdding)
        {
            if (isAdding)
            {
                Log($"{user.Id} is a new user, adding him to DB with parameters SrcLang = \"{user.SrcLang.LangCode}\", ToLang = \"{user.ToLang.LangCode}\".", true);
            }
            else
            {
                Log($"{user.Id} is a existing user, updating him at the DB with parameters SrcLang = \"{user.SrcLang.LangCode}\", ToLang = \"{user.ToLang.LangCode}\".", true);
            }
        }
        static public void Started(string name)
        {
            Log($"Bot \"{name}\" started.\n", false);
        }
        static public void ChangeLang(MessageEventArgs e, bool isSrcLangChanges, string langName)
        {
            if (isSrcLangChanges)
            {
                Log($"{e.Message.From.Id}:{e.Message.From.FirstName} switching source language to \"{langName}\".", false);
            }
            else
            {
                Log($"{e.Message.From.Id}:{e.Message.From.FirstName} switching destination language to \"{langName}\".", false);
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
                Log($"Found a {userId} in Users list.", true);
            }
        }
        static public void Replacing(int userId, User user)
        {
            Log($"Replacing {userId} in Users list with new element with parameters SrcLang = \"{user.SrcLang.LangCode}\", ToLang = \"{user.ToLang.LangCode}\".", true);
        }
        static public void UpdateUserState(User user, int state)
        {
            Log($"Updating state at user {user.Id} from {user.State} to {state}.", true);
        }
        static public void UpdateReverseMode(User user, bool isEnablingReverseMode = false)
        {
            Log($"Updating reverse mode at user {user.Id} from {user.ReverseMode == 1} to {isEnablingReverseMode}.", true);
        }
        static public void UnknownState(int state)
        {
            Log($"\"{state}\" is unknown user state, sending error sticker and resetting to \"0\".", false);
        }
        static public void WrongText(MessageEventArgs m)
        {
            Log($"\"{m.Message.Text}\" is wrong text at this state, sending error sticker.", false);
        }
    }
}
