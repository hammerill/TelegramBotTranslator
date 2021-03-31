using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBotCSharp
{
    static class MsgHandler
    {
        static public ITelegramBotClient bot;
        static public DatabaseController dc;
        static public List<User> users;
        static public BotSettings bs;

        static public async void HandleMessage(object sender, MessageEventArgs m)
        {
            List<string> buttons = new()
            {
                "С русского на английский",
                "С английского на русский"
            };

            List<List<KeyboardButton>> kbs = new();

            for (int i = 0; i < buttons.Count; i++)
            {
                kbs.Add(new List<KeyboardButton>() { new KeyboardButton(buttons[i]) });
            }

            ReplyKeyboardMarkup rkm = new()
            {
                Keyboard = kbs
            };

            var command = CmdHandler.GetCommand(m);
            if (command != null)
            {
                Logger.Got(m);

                string cmdText = CmdHandler.GetEntityText(command, m);
                if (cmdText == "/start")
                {
                    GetUser(m.Message.From.Id);

                    string MsgText = "Выберите язык и бот будет переводить введённый вами текст";
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
            else if (!buttons.Exists(a => a == m.Message.Text))
            {
                Logger.Got(m);

                User user = GetUser(m.Message.From.Id);

                string Translated = Translator.Translate(m.Message.Text, user);
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
            else if (m.Message.Text == buttons[0])
            {
                ChangeLang(m, true, dc.GetLangByCode("ru"));
                ChangeLang(m, false, dc.GetLangByCode("en"));

                string MsgText = "Готово! Теперь бот переводит русский текст в английский";
                await bot.SendTextMessageAsync(m.Message.Chat, MsgText);

                Logger.Sent(m, MsgText);
            }
            else
            {
                ChangeLang(m, true, dc.GetLangByCode("en"));
                ChangeLang(m, false, dc.GetLangByCode("ru"));

                string MsgText = "Готово! Теперь бот переводит английский текст в русский";
                await bot.SendTextMessageAsync(m.Message.Chat, MsgText);

                Logger.Sent(m, MsgText);
            }
        }

        static User GetUser(int id)
        {
            User user = DatabaseController.GetUserInList(users, id);
            if (user == null)
            {
                user = dc.GetUserInDB(id);
                if (user == null)
                {
                    dc.AddUpdateUserInDB(new User() { Id = id, SrcLang = dc.GetLangByCode("en"), ToLang = dc.GetLangByCode("ru") });
                    user = dc.GetUserInDB(id);
                    users.Add(user);
                }
            }

            return user;
        }

        static void ChangeLang(MessageEventArgs e, bool isSrcLangChanges, Lang toChange)
        {
            Logger.LangChange(e, isSrcLangChanges, toChange.FriendlyTitle);

            User user = DatabaseController.GetUserInList(users, e.Message.From.Id);
            if (user == null)
            {
                user = dc.GetUserInDB(e.Message.From.Id);
                if (user == null)
                {
                    throw new System.ArgumentException("Cannot find a user with given id, method can't handle this.");
                }
                else
                {
                    if (isSrcLangChanges)
                    {
                        user.SrcLang = toChange;
                    }
                    else
                    {
                        user.ToLang = toChange;
                    }

                    dc.AddUpdateUserInDB(user);
                }
            }
            else
            {
                if (isSrcLangChanges)
                {
                    user.SrcLang = toChange;
                }
                else
                {
                    user.ToLang = toChange;
                }

                DatabaseController.ReplaceUserInList
                (
                    users,
                    e.Message.From.Id,
                    user
                );

                dc.AddUpdateUserInDB(user);
            }
        }
    }
}
