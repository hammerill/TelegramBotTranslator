using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBotCSharp
{
    static class MsgHandler
    {
        static public ITelegramBotClient bot;
        static public SQLiter sql;
        static public List<UserParams> users;
        static public BotSettings bs;

        static public async void GotMessage(object sender, MessageEventArgs m)
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

            var command = CmdHandler.GetCommand(m);
            if (command != null)
            {
                Logger.Got(m);

                string cmdText = CmdHandler.GetEntityText(command, m);
                if (cmdText == "/start")
                {
                    GetUser(m.Message.From.Id);

                    string MsgText = "Выберите язык и бот будет переводить введённый вами текст.";
                    await bot.SendTextMessageAsync(m.Message.Chat, MsgText, replyMarkup: rkm);

                    Logger.Sent(m, "{Start message}");
                }
                else
                {
                    Logger.Err(4, info: cmdText);
                    await bot.SendStickerAsync(m.Message.Chat, bs.ErrorStickerId);
                    Logger.Sent(m, "{Error sticker}");
                }
            }
            else if (m.Message.Text != fromRussian && m.Message.Text != fromEnglish)
            {
                Logger.Got(m);

                UserParams user = GetUser(m.Message.From.Id);

                string Translated = Translator.Translate(m.Message.Text, user.IsFromEnglish);
                if (Translated == null)
                {
                    await bot.SendStickerAsync(m.Message.Chat, bs.ErrorStickerId);
                    Logger.Sent(m, "{Error sticker}");
                }
                else
                {
                    await bot.SendTextMessageAsync(m.Message.Chat, Translated);
                    Logger.Sent(m, Translated);
                }

            }
            else if (m.Message.Text == fromRussian)
            {
                ChangeLang(m, false);

                string MsgText = "Готово! Теперь бот переводит русский текст в английский.";
                await bot.SendTextMessageAsync(m.Message.Chat, MsgText);

                Logger.Sent(m, MsgText);
            }
            else
            {
                ChangeLang(m, true);

                string MsgText = "Готово! Теперь бот переводит английский текст в русский.";
                await bot.SendTextMessageAsync(m.Message.Chat, MsgText);

                Logger.Sent(m, MsgText);
            }
        }

        static UserParams GetUser(int id)
        {
            UserParams user = SQLiter.GetUserInList(users, id);
            if (user == null)
            {
                user = sql.GetUserInDB(id);
                if (user == null)
                {
                    sql.AddUpdateUserInDB(id, true);
                    user = sql.GetUserInDB(id);
                    users.Add(user);
                }
            }

            return user;
        }

        static void ChangeLang(MessageEventArgs e, bool isFromEnglish)
        {
            Logger.LangChange(e, isFromEnglish);

            UserParams user = SQLiter.GetUserInList(users, e.Message.From.Id);
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
                SQLiter.ReplaceUserInList
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
