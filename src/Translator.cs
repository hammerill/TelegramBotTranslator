using System.Web;

namespace TGBotCSharp
{
    static class Translator
    {
        static public string Translate(string toTranslate, User user)
        {
            string text = HttpUtility.UrlEncode(toTranslate);
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={user.SrcLang.LangCode}&tl={user.ToLang.LangCode}&dt=t&q={text}";

            Logger.Requesting(url);
            string result = Requester.Request(url);
            Logger.GotResponse(result);

            try
            {
                result = Parser.Parse(result);
                return result;
            }
            catch
            {
                Logger.Err();
                return null;
            }
        }
    }
}
