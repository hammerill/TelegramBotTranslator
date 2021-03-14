using System.IO;
using System.Json;

namespace TGBotCSharp
{
    static class Parser
    {
        static public string Parse(string toParse)
        {
            JsonValue json = JsonValue.Parse(toParse);
            string result = "";

            foreach (JsonValue i in json[0])
            {
                result += i[0];
            }

            return result;
        }

        static public BotSettings ReadFile(string filename)
        {
            BotSettings toReturn = new();
            JsonValue json;

            try
            {
                StreamReader sr = new(filename);
                string toParse = sr.ReadToEnd();
                json = JsonValue.Parse(toParse);
            }
            catch (System.Exception)
            {
                throw new System.ArgumentException($"Please check that file \"{filename}\" exists and contains valid JSON code.");
            }

            try
            {
                toReturn.TokenString = json["tokenString"];
            }
            catch (System.Exception)
            {
                throw new System.ArgumentException($"Please check that file \"{filename}\" contains valid JSON code.");
            }

            return toReturn;
        }
    }

    class BotSettings
    {
        public string TokenString { get; set; }

        public BotSettings() {}
    }
}
