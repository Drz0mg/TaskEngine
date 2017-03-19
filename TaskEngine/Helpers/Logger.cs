using ZzukBot.Game.Statics;

namespace TaskEngine.Helpers
{
    public static class Logger
    {
        public static void Log(string message, params object[] p)
        {
            message = string.Format(message, p);
            ZzukBot.ExtensionMethods.StringExtensions.Log(string.Format(message), "TaskEngine", true);
            Lua.Instance.Execute($"DEFAULT_CHAT_FRAME:AddMessage(\"TaskEngine: {message} \");");
        }
    }
}
