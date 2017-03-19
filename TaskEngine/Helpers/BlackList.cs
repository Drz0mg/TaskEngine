using System;
using System.Collections.Generic;
using ZzukBot.Objects;

namespace TaskEngine.Helpers
{
    class BlackList
    {
        private static Dictionary<string, GTimer> blacklisted = new Dictionary<string, GTimer>();

        public static void Blacklist(string name, int howlongSeconds)
        {
            lock (blacklisted)
            {
                GTimer t = null;
                if (blacklisted.TryGetValue(name, out t))
                {
                    blacklisted.Remove(name);
                }
                t = new GTimer(howlongSeconds * 1000);
                blacklisted.Add(name, t);
                Logger.Log("Blacklisted " + name + " for " + howlongSeconds + "s");
            }
        }
        public static void Blacklist(ulong guid, int howlongSeconds)
        {
            lock (blacklisted)
            {
                Blacklist("GUID" + guid, howlongSeconds);
            }
        }

        public static void Blacklist(WoWUnit unit)
        {
            lock (blacklisted)
            {
                Blacklist(unit.Guid, 15 * 60); // 15 minutes
            }
        }
        public static void Blacklist(WoWUnit unit, int howlongSeconds)
        {
            lock (blacklisted)
            {
                Blacklist(unit.Guid, howlongSeconds);
            }
        }
        public static void Blacklist(String name)
        {
            lock (blacklisted)
            {
                Blacklist(name, 15 * 60); // 15 minutes
            }
        }

        public static void UnBlacklist(string name)
        {
            lock (blacklisted)
            {
                blacklisted.Remove(name);
                Logger.Log("Un-Blacklisted " + name);
            }
        }
        public static void UnBlacklist(ulong guid)
        {
            lock (blacklisted)
            {
                UnBlacklist("GUID" + guid);
            }
        }

        public static void UnBlacklist(WoWUnit u)
        {
            lock (blacklisted)
            {
                UnBlacklist(u.Guid);
            }
        }

        public static bool IsBlacklisted(string name)
        {
            lock (blacklisted)
            {
                GTimer t = null;
                if (!blacklisted.TryGetValue(name, out t))
                    return false;

                return !t.IsReady();
            }
        }

        public static bool IsBlacklisted(ulong guid)
        {
            lock (blacklisted)
            {
                return IsBlacklisted("GUID" + guid);
            }
        }

        public static bool IsBlacklisted(WoWUnit unit)
        {
            lock (blacklisted)
            {
                return IsBlacklisted(unit.Guid);
            }
        }
    }
}
