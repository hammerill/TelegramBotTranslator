using System;
using System.Collections.Generic;
using Telegram.Bot;

namespace TGBotCSharp
{
    class Program
    {
        static ITelegramBotClient bot;
        static DatabaseController dc;
        static List<User> users;
        static BotSettings bs;

        static int Main()
        {
            try
            {
                bs = Parser.ReadFile("..\\..\\..\\botsettings.json");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            Logger.LogFileLocation      = bs.LogFileLocation;
            Logger.DevLogFileLocation   = bs.DevLogFileLocation;

            dc = new(bs.DBLocation, bs.LangsDBLocation);
            users = dc.GetAllUsers();
            foreach (User user in users) //I unsuccesfully tried to fix bug with entity objects normally, so I just wrote that s**tcode to get bot work
            {
                user.SrcLang = dc.GetLangById(user.SrcLangId);
                user.ToLang = dc.GetLangById(user.ToLangId);
            }

            try
            {
                bot = new TelegramBotClient(bs.TokenString);
                var botMe = bot.GetMeAsync().Result;
                Logger.Started(botMe.FirstName);
            }
            catch
            {
                Console.WriteLine("Failed to initialize bot.\nMaybe you inserted an invalid token?\nAlso check your internet connection.");
                return -1;
            }

            MsgHandler.bot      = bot;
            MsgHandler.dc       = dc;
            MsgHandler.users    = users;
            MsgHandler.bs       = bs;

            bot.OnMessage += MsgHandler.HandleMessage;
            bot.StartReceiving();

            Console.ReadKey();

            bot.StopReceiving();

            return 0;
        }
    }
}