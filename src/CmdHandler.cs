using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace TGBotCSharp
{
    class CmdHandler
    {
        static public MessageEntity GetCommand(MessageEventArgs m)
        {
            if (m.Message.Entities != null)
            {
                foreach (var item in m.Message.Entities)
                {
                    if (item.Type == Telegram.Bot.Types.Enums.MessageEntityType.BotCommand)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        static public string GetEntityText(MessageEntity me, MessageEventArgs m)
        {
            int offset = me.Offset;
            int len = me.Length;
            string text = m.Message.Text.Substring(offset, len);

            return text;
        }
    }
}
