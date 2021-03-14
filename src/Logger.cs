using System;
using Telegram.Bot.Args;

namespace TGBotCSharp
{
    static class Logger
    {
        static public void Got(MessageEventArgs e)
        {
            Console.WriteLine($"{e.Message.From.Id}:{e.Message.From.FirstName}:\t\t\"{e.Message.Text}\".");
        }
        static public void Sent(MessageEventArgs e, string msgText)
        {
            Console.WriteLine($"Bot to {e.Message.From.Id}:{e.Message.From.FirstName}:\t\"{msgText}\".\n\n");
        }
        static public void Requesting(string url)
        {
            Console.WriteLine($"Sending request to {{\n{url}\n}}.");
        }
        static public void GotResponse(string result)
        {
            Console.WriteLine($"Got {{\n{result}\n}}. Parsing this...");
        }
        static public void Err(int errType = 0, int userId = 0, string info = "")
        {
            switch (errType)
            {
                case 0:
                    Console.WriteLine("Error occured while parsing, returning error string.");
                    break;
                case 1:
                    Console.WriteLine($"Error ({info}) occured while reading user {userId} at DB, returning null.");
                    break;
                case 2:
                    Console.WriteLine($"Cannot find user {userId} in database, returning null.");
                    break;
                case 3:
                    Console.WriteLine($"Cannot find user {userId} in UserParams list, returning null.");
                    break;
                default:
                    break;
            }
        }
        static public void AddUpdateUser(int userId, bool isFromEnglish, bool isAdding)
        {
            if (isAdding)
            {
                Console.WriteLine($"{userId} - new user, adding him to DB with parameter isFromEnglish = {isFromEnglish}...");
            }
            else
            {
                Console.WriteLine($"{userId} - existing user, updating isFromEnglish to {isFromEnglish} at the DB...");
            }
        }
        static public void Started()
        {
            Console.WriteLine("Awaiting messages, press any key to stop.\n\n");
        }
        static public void LangChange(MessageEventArgs e, bool isFromEnglish)
        {
            if (isFromEnglish)
            {
                Console.WriteLine($"{e.Message.From.Id}:{e.Message.From.FirstName} switching language to \"English\" -> \"Russian\".");
            }
            else
            {
                Console.WriteLine($"{e.Message.From.Id}:{e.Message.From.FirstName} switching language to \"Russian\" -> \"English\".");
            }
        }
        static public void FoundUser(int userId, bool isFromDb)
        {
            if (isFromDb)
            {
                Console.WriteLine($"Found a {userId} in database.\n");
            }
            else
            {
                Console.WriteLine($"Found a {userId} in UserParams list.");
            }
        }
        static public void Replacing(int userId, bool isFromEnglish)
        {
            Console.WriteLine($"Replacing {userId} in UserParams list with new element, where isFromEnglish = {isFromEnglish}.");
        }
    }
}
