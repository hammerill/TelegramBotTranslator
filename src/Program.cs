using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBotCSharp
{
    static class Program
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

            sql = new("..\\..\\..\\userparams.db", bs.DebugLog);
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
            bot.OnMessage += GotMessage;
            bot.StartReceiving();

            if (bs.DebugLog)
            {
                Console.WriteLine("Debug log enabled.");
            }
            Logger.Started();
            Console.ReadKey();

            bot.StopReceiving();

            return 0;
        }

        static async void GotMessage(object sender, MessageEventArgs e)
        {
            string fromEnglish = "С английского на русский", fromRussian = "С русского на английский";
            ReplyKeyboardMarkup rkm = new()
            {
                Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton(fromEnglish)
                    },
                    new[]
                    {
                        new KeyboardButton(fromRussian)
                    }
                }
            };

            if (e.Message.Text != "/start" && e.Message.Text != fromRussian && e.Message.Text != fromEnglish)
            {
                Logger.Got(e);

                UserParams user = sql.GetUserInList(users, e.Message.From.Id);
                if (user == null)
                {
                    user = sql.GetUserInDB(e.Message.From.Id);
                    if (user == null)
                    {
                        sql.AddUpdateUserInDB(e.Message.From.Id, true);
                        user = sql.GetUserInDB(e.Message.From.Id);
                        users.Add(user);
                    }
                }

                string Translated = Translator.Translate(e.Message.Text, user.IsFromEnglish, bs.DebugLog);
                await bot.SendTextMessageAsync(e.Message.Chat, Translated);

                Logger.Sent(e, Translated);
            }
            else if (e.Message.Text == fromRussian || e.Message.Text == fromEnglish)
            {
                if (e.Message.Text == fromRussian)
                {
                    ChangeLang(e, false);

                    string MsgText = "Готово! Теперь бот переводит русский текст в английский.";
                    await bot.SendTextMessageAsync(e.Message.Chat, MsgText);

                    Logger.Sent(e, MsgText);
                }
                else
                {
                    ChangeLang(e, true);

                    string MsgText = "Готово! Теперь бот переводит английский текст в русский.";
                    await bot.SendTextMessageAsync(e.Message.Chat, MsgText);

                    Logger.Sent(e, MsgText);
                }
            }
            else
            {
                Logger.Got(e);

                UserParams user = sql.GetUserInList(users, e.Message.From.Id);
                if (user == null)
                {
                    user = sql.GetUserInDB(e.Message.From.Id);
                    if (user == null)
                    {
                        sql.AddUpdateUserInDB(e.Message.From.Id, true);
                        user = sql.GetUserInDB(e.Message.From.Id);
                        users.Add(user);
                    }
                }

                string MsgText = "Выберите язык и бот будет переводить введённый вами текст.";
                await bot.SendTextMessageAsync(e.Message.Chat, MsgText, replyMarkup: rkm);

                Logger.Sent(e, MsgText);
            }
        }

        static public void ChangeLang(MessageEventArgs e, bool isFromEnglish)
        {
            Logger.LangChange(e, isFromEnglish);

            UserParams user = sql.GetUserInList(users, e.Message.From.Id);
            if (user == null)
            {
                user = sql.GetUserInDB(e.Message.From.Id);
                if (user == null)
                {
                    sql.AddUpdateUserInDB(e.Message.From.Id, isFromEnglish);
                    user = sql.GetUserInDB(e.Message.From.Id);
                    users.Add(user);
                }
                else
                {
                    sql.AddUpdateUserInDB(e.Message.From.Id, isFromEnglish);
                }
            }
            else
            {
                sql.ReplaceUserInList
                (
                    users,
                    e.Message.From.Id,
                    new UserParams()
                    {
                        UserId = e.Message.From.Id,
                        IsFromEnglish = isFromEnglish
                    }
                );
                sql.AddUpdateUserInDB(e.Message.From.Id, isFromEnglish);
            }
        }
    }
}