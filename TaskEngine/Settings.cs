using System;
using System.Reflection;

namespace TaskEngine
{
    public static class Settings
    {
        public static string TaskFile => $"{new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath.Replace("Internal\\ZzukBot.exe", "BotBases\\")}Task.psc";
    }
}
