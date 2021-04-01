using System;
using System.Collections.Generic;
using System.Linq;

namespace TGBotCSharp
{
    class DatabaseController
    {
        private readonly TranslatorContext tc;
        public string LangsFilename { get; set; }

        public DatabaseController(string mainFilename, string langsFilename)
        {
            tc = new(mainFilename);
            LangsFilename = langsFilename;

            try
            {
                tc.Langs.Single(a => a.LangCode == "en");
            }
            catch
            {
                LoadLangs();
            }
        }

        public User GetUserInDB(int telegramId)
        {
            User toReturn;
            try
            {
                toReturn = tc.Users.Single(a => a.Id == telegramId);
            }
            catch (Exception e)
            {
                Logger.Err(1, telegramId, e.Message);
                return null;
            }
            Logger.FoundUser(telegramId, true);

            return toReturn;
        }

        public Lang GetLangByCode(string langCode)
        {
            Lang toReturn;

            try
            {
                toReturn = tc.Langs.Single(a => a.LangCode == langCode);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            return toReturn;
        }

        public Lang GetLangById(int id)
        {
            Lang toReturn;

            try
            {
                toReturn = tc.Langs.Single(a => a.Id == id);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            return toReturn;
        }

        public void AddUpdateUserInDB(User user)
        {
            bool isAdding = !IsUserExistsInDB(user.Id);
            Logger.AddUpdateUser(user, isAdding);

            if (isAdding)
            {
                tc.Users.Add(user);
            }
            else
            {
                tc.Users.Update(user);
            }
            tc.SaveChanges();
        }

        public bool IsUserExistsInDB(int userId)
        {
            try
            {
                tc.Users.Single(a => a.Id == userId);
            }
            catch { return false; }

            return true;
        }

        public List<User> GetAllUsers()
        {
            return tc.Users.ToList();
        }

        public List<Lang> GetAllLangs()
        {
            return tc.Langs.ToList();
        }

        static public User GetUserInList(List<User> users, int id)
        {
            try
            {
                foreach (var item in users)
                {
                    if (item.Id == id)
                    {
                        Logger.FoundUser(id, false);
                        return item;
                    }
                }
            }
            catch (Exception) {}

            Logger.Err(3, id);
            return null;
        }
        static public void ReplaceUserInList(List<User> ups, int id, User user)
        {
            for (int i = 0; i < ups.Count; i++)
            {
                if (ups[i].Id == id)
                {
                    Logger.Replacing(id, user);
                    ups[i] = user;
                    return;
                }
            }
            throw new ArgumentException("Cannot find a user with given id."); //This method doesn't calls where user with given id doesn't exists
        }

        public void UpdateUserState(User user, int state = 0, bool isSrcLangChanges = true)
        {
            Logger.UpdateUserState(user, state);
            user.State = state;
            user.IsSrcLangChanges = isSrcLangChanges ? 1 : 0;
            tc.SaveChanges();
        }
        public void UpdateReverseMode(User user, bool isEnablingReverseMode = false)
        {
            Logger.UpdateReverseMode(user, isEnablingReverseMode);
            user.ReverseMode = isEnablingReverseMode ? 1 : 0;
            tc.SaveChanges();
        }

        public void LoadLangs()
        {
            List<Lang> langs = new();

            using (LangsContext lc = new(LangsFilename))
            {
                for (int i = 1; ; i++)
                {
                    try
                    {
                        langs.Add(lc.Langs.Single(a => a.Id == i));
                    }
                    catch { break; }
                }
            }

            tc.Langs.Add(new Lang()
            {
                LangCode = "auto",
                FriendlyTitle = "автоматический"
            });

            for (int i = 0; i < langs.Count; i++)
            {
                tc.Langs.Add(new Lang()
                {
                    Id = i + 2,
                    LangCode = langs[i].LangCode,
                    FriendlyTitle = langs[i].FriendlyTitle
                });
            }

            tc.SaveChanges();
        }
    }
}
