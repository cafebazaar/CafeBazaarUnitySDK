namespace CafeBazaar.Games.OurUtils
{
    using System;
    using UnityEngine;

    public class Logger
    {
        private static bool debugLogEnabled = false;

        public static bool DebugLogEnabled
        {
            get { return debugLogEnabled; }

            set { debugLogEnabled = value; }
        }

        private static bool warningLogEnabled = true;

        public static bool WarningLogEnabled
        {
            get { return warningLogEnabled; }

            set { warningLogEnabled = value; }
        }

        public static void d(string msg)
        {
            if (debugLogEnabled)
            {
                Debug.Log(ToLogMessage(string.Empty, "DEBUG", msg));
            }
        }

        public static void w(string msg)
        {
            if (warningLogEnabled)
            {
                Debug.LogWarning(ToLogMessage("!!!", "WARNING", msg));
            }
        }

        public static void e(string msg)
        {
            if (warningLogEnabled)
            {
                Debug.LogWarning(ToLogMessage("***", "ERROR", msg));
            }
        }

        public static string describe(byte[] b)
        {
            return b == null ? "(null)" : "byte[" + b.Length + "]";
        }

        private static string ToLogMessage(string prefix, string logType, string msg)
        {
            string timeString = null;
            try
            {
                timeString = DateTime.Now.ToString("MM/dd/yy H:mm:ss zzz");
            }
            catch (Exception)
            {
                Debug.LogWarning("*** [CafeBazaar Games Plugin " + PluginVersion.VersionString + "] ERROR: Failed to format DateTime.Now");
                timeString = string.Empty;
            }

            return string.Format("{0} [CafeBazaar Games Plugin " + PluginVersion.VersionString + "] {1} {2}: {3}",
                prefix, timeString, logType, msg);
        }
    }
}
