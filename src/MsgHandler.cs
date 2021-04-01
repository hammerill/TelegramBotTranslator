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

        static private readonly List<string> mainMenu = new()
        {
            "Сменить исходный язык",
            "Сменить выводимый язык",
            "Поменять языки местами",
            "Переключить обратный перевод",
            "Просмотреть текущие настройки"
        };
        static private readonly List<string> usualLangs = new()
        {
            "Автоопределение",
            "Русский",
            "Английский",
            "Украинский",
            "Все языки"
        };
        static private readonly List<string> allLangs = new();

        static public async void HandleMessage(object sender, MessageEventArgs m)
        {
            User user = GetUser(m.Message.From.Id);

            switch (user.State)
            {
                case 0: //main state
                    var command = CmdHandler.GetCommand(m);
                    if (command != null) //Command handled
                    {
                        Logger.Got(m);

                        string cmdText = CmdHandler.GetEntityText(command, m);
                        if (cmdText == "/start")
                        {
                            ReplyKeyboardMarkup rkm = GetRkm(mainMenu);

                            string msgText = "Выберите язык и бот будет переводить введённый вами текст";
                            await bot.SendTextMessageAsync(m.Message.Chat, msgText, replyMarkup: rkm);

                            Logger.Sent(m, "{Start message}");
                        }
                        else
                        {
                            Logger.Err(4, info: cmdText);
                            await bot.SendStickerAsync(m.Message.Chat, bs.ErrorStickerId);
                            Logger.Sent(m, "{Error sticker}");
                        }
                    }
                    else if (!mainMenu.Exists(a => a == m.Message.Text)) //Text translating
                    {
                        Logger.Got(m);

                        string translated;
                        if (user.ReverseMode == 1)
                        {
                            translated = Translator.Translate(m.Message.Text, user);
                            translated = Translator.Translate(translated, user, true);
                        }
                        else
                        {
                            translated = Translator.Translate(m.Message.Text, user);
                        }

                        if (translated != null)
                        {
                            await bot.SendTextMessageAsync(m.Message.Chat, translated);
                            Logger.Sent(m, translated);
                        }
                        else
                        {
                            await bot.SendStickerAsync(m.Message.Chat, bs.ErrorStickerId);
                            Logger.Sent(m, "{Error sticker}");
                        }

                    }
                    else if (m.Message.Text == mainMenu[0]) //SrcLang change
                    {
                        Logger.Menu(m, 0);

                        string MsgText = "Выберите исходный язык";

                        ReplyKeyboardMarkup rkm = GetRkm(usualLangs);

                        dc.UpdateUserState(user, 1, true); //setting state to usual SrcLang changing

                        await bot.SendTextMessageAsync(m.Message.Chat, MsgText, replyMarkup: rkm);
                        Logger.Sent(m, MsgText);
                    }
                    else if (m.Message.Text == mainMenu[1]) //ToLang change
                    {
                        Logger.Menu(m, 1);

                        string MsgText = "Выберите выводимый язык";

                        ReplyKeyboardMarkup rkm = GetRkm(usualLangs);

                        dc.UpdateUserState(user, 1, false); //setting state to usual ToLang changing

                        await bot.SendTextMessageAsync(m.Message.Chat, MsgText, replyMarkup: rkm);
                        Logger.Sent(m, MsgText);
                    }
                    else if (m.Message.Text == mainMenu[2]) //Langs switching
                    {
                        Logger.Menu(m, 4, user);

                        Lang OldSrc = user.SrcLang, OldTo = user.ToLang;

                        string MsgText = $"Вы поменяли местами {OldSrc.FriendlyTitle} и {OldTo.FriendlyTitle} языки";

                        ChangeLang(m, true, OldTo);
                        ChangeLang(m, false, OldSrc);

                        await bot.SendTextMessageAsync(m.Message.Chat, MsgText);
                        Logger.Sent(m, MsgText);
                    }
                    else if (m.Message.Text == mainMenu[3]) //Reverse mode switching
                    {
                        Logger.Menu(m, 5, user);

                        string msgText;

                        if (user.ReverseMode == 1)
                        {
                            msgText = "Вы выключили режим обратного перевода";
                            dc.UpdateReverseMode(user, false);
                        }
                        else
                        {
                            msgText = "Вы включили режим обратного перевода";
                            dc.UpdateReverseMode(user, true);
                        }

                        await bot.SendTextMessageAsync(m.Message.Chat, msgText);
                        Logger.Sent(m, msgText);
                    }
                    else //Get status
                    {
                        Logger.Menu(m, 2);

                        string msgText;
                        if (user.ReverseMode == 1)
                        {
                            msgText = $"Исходным языком является {user.SrcLang.FriendlyTitle}\nВыводимым языком является {user.ToLang.FriendlyTitle}\nРежим обратного перевода включен";
                        }
                        else
                        {
                            msgText = $"Исходным языком является {user.SrcLang.FriendlyTitle}\nВыводимым языком является {user.ToLang.FriendlyTitle}\nРежим обратного перевода выключен";
                        }

                        await bot.SendTextMessageAsync(m.Message.Chat, msgText);
                        Logger.Sent(m, $"{{SrcLang: \"{user.SrcLang.LangCode}\", ToLang: \"{user.ToLang.LangCode}\", RevMode: {user.ReverseMode}}}");
                    }
                    break;
                case 1: //usual langs changing
                    if (usualLangs.Exists(a => a == m.Message.Text))
                    {
                        string msgText;
                        if (user.IsSrcLangChanges == 1)
                        {
                            msgText = "Теперь ваш исходный язык ";
                        }
                        else
                        {
                            msgText = "Теперь ваш выводимый язык ";
                        }

                        int selectedButton = usualLangs.FindIndex(a => a == m.Message.Text);
                        string selectedLangCode = "";
                        switch (selectedButton)
                        {
                            case 0:
                                selectedLangCode = "auto";
                                break;
                            case 1:
                                selectedLangCode = "ru";
                                break;
                            case 2:
                                selectedLangCode = "en";
                                break;
                            case 3:
                                selectedLangCode = "uk";
                                break;
                            case 4:
                                if (allLangs.Count == 0)
                                {
                                    foreach (Lang lang in dc.GetAllLangs())
                                    {
                                        allLangs.Add(lang.FriendlyTitle);
                                    }
                                }

                                ReplyKeyboardMarkup rkm = GetRkm(allLangs);

                                dc.UpdateUserState(user, 2, user.IsSrcLangChanges == 1); //setting state to all langs changing

                                await bot.SendTextMessageAsync(m.Message.Chat, "Выберите между всеми языками", replyMarkup: rkm);
                                Logger.Menu(m, 3);
                                return;
                        }
                        Lang selectedLang = dc.GetLangByCode(selectedLangCode);
                        ChangeLang(m, user.IsSrcLangChanges == 1, selectedLang);

                        msgText += selectedLang.FriendlyTitle;

                        dc.UpdateUserState(user); //resetting state

                        await bot.SendTextMessageAsync(m.Message.Chat, msgText, replyMarkup: GetRkm(mainMenu));
                        Logger.Sent(m, "{Lang changed}");
                    }
                    else
                    {
                        Logger.WrongText(m);
                        await bot.SendStickerAsync(m.Message.Chat, bs.ErrorStickerId);
                        Logger.Sent(m, "{Error sticker}");
                    }
                    break;
                case 2: //all langs changing
                    if (allLangs.Count == 0)
                    {
                        foreach (Lang lang in dc.GetAllLangs())
                        {
                            allLangs.Add(lang.FriendlyTitle);
                        }
                    }

                    if (allLangs.Exists(a => a == m.Message.Text))
                    {
                        string msgText;
                        if (user.IsSrcLangChanges == 1)
                        {
                            msgText = "Теперь ваш исходный язык ";
                        }
                        else
                        {
                            msgText = "Теперь ваш выводимый язык ";
                        }

                        int selectedButton = allLangs.FindIndex(a => a == m.Message.Text);
                        Lang selectedLang = dc.GetLangById(selectedButton + 1); //In list counting starts with 0, in DB with 1, so adding 1 to get synced

                        ChangeLang(m, user.IsSrcLangChanges == 1, selectedLang);
                        
                        msgText += selectedLang.FriendlyTitle;

                        dc.UpdateUserState(user); //resetting state

                        await bot.SendTextMessageAsync(m.Message.Chat, msgText, replyMarkup: GetRkm(mainMenu));
                        Logger.Sent(m, "{Lang changed}");
                    }
                    else
                    {
                        Logger.WrongText(m);
                        await bot.SendStickerAsync(m.Message.Chat, bs.ErrorStickerId);
                        Logger.Sent(m, "{Error sticker}");
                    }
                    break;
                default:
                    Logger.UnknownState(user.State);

                    dc.UpdateUserState(user, 0, true);

                    await bot.SendStickerAsync(m.Message.Chat, bs.ErrorStickerId, replyMarkup: GetRkm(mainMenu));
                    Logger.Sent(m, "{Error sticker}");
                    break;
            }
        }

        static ReplyKeyboardMarkup GetRkm(List<string> buttons)
        {
            List<List<KeyboardButton>> kbs = new();

            for (int i = 0; i < buttons.Count; i++)
            {
                kbs.Add(new List<KeyboardButton>() { new KeyboardButton(buttons[i]) });
            }

            ReplyKeyboardMarkup toReturn = new()
            {
                Keyboard = kbs
            };

            return toReturn;
        }
        static User GetUser(int id)
        {
            User user = DatabaseController.GetUserInList(users, id);
            if (user == null)
            {
                user = dc.GetUserInDB(id);
                if (user == null)
                {
                    dc.AddUpdateUserInDB(
                        new User() 
                        { 
                            Id = id,
                            SrcLang = dc.GetLangByCode("en"),
                            ToLang = dc.GetLangByCode("ru"),
                            State = 0 ,
                            IsSrcLangChanges = 1,
                            ReverseMode = 0
                        });
                    user = dc.GetUserInDB(id);
                    users.Add(user);
                }
            }
            return user;
        }

        static void ChangeLang(MessageEventArgs m, bool isSrcLangChanges, Lang toChange)
        {
            Logger.ChangeLang(m, isSrcLangChanges, toChange.FriendlyTitle);

            User user = GetUser(m.Message.From.Id);

            if (isSrcLangChanges)
            {
                user.SrcLang = toChange;
            }
            else
            {
                user.ToLang = toChange;
            }
            dc.AddUpdateUserInDB(user);

            DatabaseController.ReplaceUserInList
            (
                users,
                m.Message.From.Id,
                user
            );
        }
    }
}
