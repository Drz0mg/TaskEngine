using System;
using System.Collections.Generic;

namespace TaskEngine.Helpers
{

    /// <summary>
    /// This class is an helper class the implements Functions used often.
    /// Created 20-12-2009 by cromon.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Ensures the angle is in the range 0 to 2PI
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static float EnsureAngle(float angle)
        {
            if (angle < 0) { angle += (float)(Math.PI * 2); }
            if (angle > Math.PI * 2) { angle -= (float)Math.PI * 2; }
            return angle;
        }

        /// <summary>
        /// Gets the task file path.
        /// </summary>
        /// <returns></returns>
        public static string GetTaskFilePath()
        {
            string fullpath = Settings.TaskFile;
            string[] parts = fullpath.Split('\\');
            string taskname = parts[parts.Length - 1];
            fullpath = fullpath.Substring(0, fullpath.Length - taskname.Length);
            return fullpath;
        }
    }
}
