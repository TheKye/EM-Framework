using Eco.Core.Utils.Logging;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Logging
{
    public class LoggingUtils
    {
        private static string emLogger = "EM-Framework";

        private static NLogWriter Logging = NLogManager.GetLogWriter(emLogger);
        private static Dictionary<string, NLogWriter> loged = new();

        public static NLogWriter Logger => GetLogger();

        public static void RegisterNewLogger(string log) {
            NLogWriter logger = NLogManager.GetLogWriter(log);
            var assembly = Assembly.GetCallingAssembly();
            if (loged.ContainsKey(assembly.FullName))
            {
                Log.WriteError(Localizer.DoStr("Duplicate Key Entry: This Assembly has already registered a logger please use that logger instead."));
                return;
            }
            loged.Add(assembly.FullName, logger);
        }

        private static NLogWriter GetLogger()
        {
            var assembly = Assembly.GetCallingAssembly();
            foreach(var l in loged)
            {
                if (l.Key == assembly.FullName)
                    return l.Value;
            }
            return Logging;
        }

        public static void Write(string s) => Logger.Write(s);

        public static void Debug(string s) => Logger.Debug(s);

        public static void Warning(string s) => Logger.WriteWarning(s);

        public static void Error(string s) => Logger.WriteError(s);

        public static void LogTypeSelect(string message, LogType logType)
        {
            message = Localizer.DoStr(message);

            switch (logType)
            {
                case LogType.Info:
                    Logger.Write(message);
                    return;
                case LogType.Error:
                    Logger.WriteError(message);
                    return;
                case LogType.Warn:
                    Logger.WriteWarning(message);
                    return;
                case LogType.Debug:
                    Logger.Debug(message);
                    return;
                case LogType.Important:
                    Logger.WriteError(message);
                    Log.WriteError(Localizer.DoStr(message));
                    return;
                default:
                    Logger.Write(message);
                    return;
            }
        }

        public enum LogType
        {
            Info,
            Error,
            Debug,
            Warn,
            Important
        }
    }
}
