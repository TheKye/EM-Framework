using Eco.Core.Utils.Logging;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Logging
{
    public class LoggingUtils
    {
        private static string Logger = "EMDefaultDebugging";

        private static NLogWriter Logging = NLogManager.GetLogWriter(Logger);

        public static NLogWriter RegisterNewLogger(string log) { return Logging = NLogManager.GetLogWriter(log); }

        public static void Write(string s) => Logging.Write(s);

        public static void Debug(string s) => Logging.Debug(s);

        public static void Warning(string s) => Logging.WriteWarning(s);

        public static void Error(string s) => Logging.WriteError(s);

        public static void LogTypeSelect(string message, LogType logType)
        {
            message = Localizer.DoStr(message);
            if (logType == LogType.Info)
            {
                Logging.Write(message);
            }
            if (logType == LogType.Error)
            {
                Logging.WriteError(message);
            }
            if (logType == LogType.Warn)
            {
                Logging.WriteWarning(message);
            }
            if (logType == LogType.Debug)
            {
                Logging.Debug(message);
            }
        }

        public enum LogType
        {
            Info,
            Error,
            Debug,
            Warn
        }
    }
}
