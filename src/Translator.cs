using System.Web;

namespace TGBotCSharp
{
    static class Translator
    {
        static public string Translate(string toTranslate, bool fromEnglish = true, bool debug = false)
        {
            string text = HttpUtility.UrlEncode(toTranslate);
            string urlEnglish = string.Format($"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=ru&dt=t&q={text}");
            string urlRussian = string.Format($"https://translate.googleapis.com/translate_a/single?client=gtx&sl=ru&tl=en&dt=t&q={text}");

            string url = fromEnglish ? urlEnglish : urlRussian;

            if (debug) { Logger.Requesting(url); }
            string result = Requester.Request(url);
            if (debug) { Logger.GotResponse(result); }

            try
            {
                result = Parser.Parse(result);
                if (debug) { Logger.Returning(result); }
                return result;
            }
            catch
            {
                Logger.Err();
                return "Error";
            }
        }
    }
}
