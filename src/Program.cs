using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBotCSharp
{
    class Program
    {
        static ITelegramBotClient bot;
        static SQLiter sql;
        static List<UserParams> users;
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

            Logger.LogFileLocation = bs.LogFileLocation;
            Logger.DevLogFileLocation = bs.DevLogFileLocation;

            sql = new(bs.DBLocation);
            users = sql.GetAllUsers();


            try
            {
                bot = new TelegramBotClient(bs.TokenString);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            MsgHandler.bot = bot;
            MsgHandler.sql = sql;
            MsgHandler.users = users;
            MsgHandler.bs = bs;

            bot.OnMessage += MsgHandler.GotMessage;
            bot.StartReceiving();

            Logger.Started();
            Console.ReadKey();

            bot.StopReceiving();

            return 0;
        }
    }
}