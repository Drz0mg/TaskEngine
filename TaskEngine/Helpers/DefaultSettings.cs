namespace TaskEngine.Helpers
{
    /// <summary>
    /// This class contains default configs, if they are not included in this class they will return 0, false etc when not included in a task file.
    /// </summary>
    internal class DefaultSettings
    {
        public static string GetConfigString(string toGet)
        {
            if (toGet.Equals("Distance"))
            {
                return "30";
            }
            if (toGet.Equals("SkipMobsWithAdds"))
            {
                return "false";
            }
            if (toGet.Equals("AddsDistance"))
            {
                return "15";
            }
            if (toGet.Equals("AddsCount"))
            {
                return "3";
            }
            if (toGet.Equals("MinLevel"))
            {
                return "0";
            }
            if (toGet.Equals("MaxLevel"))
            {
                return "100";
            }
            if (toGet.Equals("PullDistance"))
            {
                return "20";
            }
            return "";
        }
    }
}
