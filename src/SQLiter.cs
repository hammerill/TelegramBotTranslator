using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace TGBotCSharp
{
    class SQLiter
    {
        public string Filename { get; set; }
        public bool Debug { get; set; }
        public SQLiteConnection Connection;

        public SQLiter(string filename, bool debug = false)
        {
            Filename = filename;
            Debug = debug;

            if (!File.Exists(Filename))
            {
                SQLiteConnection.CreateFile(Filename);
            }

            Connection = new SQLiteConnection
            {
                ConnectionString = $"Data Source = {Filename}"
            };
            Connection.Open();

            CreateTables();
        }
        ~SQLiter()
        {
            Connection.Close();
        }

        public void CreateTables()
        {
            SQLiteCommand command = Connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS Languages (userId INTEGER, isFromEnglish INTEGER)";

            command.ExecuteNonQuery();
        }

        public UserParams GetUserInDB(int userId)
        {
            UserParams toReturn;

            SQLiteCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM Languages WHERE userId = $id";
            command.Parameters.AddWithValue("$id", userId);

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                reader.Read();

                bool isFromEnglish;
                try
                {
                    isFromEnglish = (reader.GetInt32(1) == 1);
                }
                catch (Exception e)
                {
                    if (Debug) { Logger.Err(1, userId, e.Message); }
                    return null;
                }
                toReturn = new UserParams() { UserId = userId, IsFromEnglish = isFromEnglish };
            }

            if (Debug) { Logger.FoundUser(userId, true); }

            return toReturn;
        }

        public void AddUpdateUserInDB(int userId, bool isFromEnglish)
        {
            SQLiteCommand command = Connection.CreateCommand();

            if (IsUserExistsInDB(userId))
            {
                if (Debug) { Logger.AddUpdateUser(userId, isFromEnglish, false); }
                command.CommandText = "UPDATE Languages SET isFromEnglish = $isFromEnglish WHERE userId = $userId";
                command.Parameters.AddWithValue("$userId", userId);
                command.Parameters.AddWithValue("$isFromEnglish", isFromEnglish ? 1 : 0);
            }
            else
            {
                if (Debug) { Logger.AddUpdateUser(userId, isFromEnglish, true); }
                command.CommandText = "INSERT INTO Languages VALUES ($userId, $isFromEnglish)";
                command.Parameters.AddWithValue("$userId", userId);
                command.Parameters.AddWithValue("$isFromEnglish", isFromEnglish ? 1 : 0);
            }

            command.ExecuteNonQuery();
        }

        public bool IsUserExistsInDB(int userId)
        {
            SQLiteCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM Languages WHERE userId = $id";
            command.Parameters.AddWithValue("$id", userId);

            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();

            try
            {
                reader.GetInt32(0);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public List<UserParams> GetAllUsers()
        {
            List<UserParams> toReturn = new();

            SQLiteCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM Languages";

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                toReturn.Add(new UserParams() { UserId = reader.GetInt32(0), IsFromEnglish = (reader.GetInt32(1) == 1) });
            }

            return toReturn;
        }

        public UserParams GetUserInList(List<UserParams> ups, int id)
        {
            foreach (var item in ups)
            {
                if (item.UserId == id)
                {
                    if (Debug) { Logger.FoundUser(id, false); }
                    return item;
                }
            }

            if (Debug) { Logger.Err(3, id); }
            return null;
        }
        public void ReplaceUserInList(List<UserParams> ups, int id, UserParams up)
        {
            for (int i = 0; i < ups.Count; i++)
            {
                if (ups[i].UserId == id)
                {
                    if (Debug) { Logger.Replacing(id, up.IsFromEnglish); }
                    ups[i] = up;
                    return;
                }
            }

            throw new ArgumentException("Cannot find a user with given id.");
        }
    }

    class UserParams
    {
        public int UserId { get; set; }
        public bool IsFromEnglish { get; set; }

        public UserParams() {}
    }
}
