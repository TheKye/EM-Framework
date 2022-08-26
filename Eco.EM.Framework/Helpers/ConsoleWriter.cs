

namespace Eco.EM.Framework.Console
{
    using System;
    using Eco.Shared.Localization;
    using Eco.Shared.Utils;
    using System.Collections.Generic;
    using System.Text;
    using Eco.Core.Utils;
    using System.Security.Cryptography;
    using Eco.Gameplay.Players;
    using System.Threading.Tasks;
    using System.Reflection;
    using Eco.Core.Utils.Logging;

    public class ConsoleWriter
    {
        static object log => typeof(Log).GetField("writer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(typeof(Log));

        static object logger => typeof(NLogWriter).GetField("logger", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(log);

        private static void PrintString(string value)
        {
            Console.Write("{0}", value);
        }

        private static void PrintColoredString(string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            PrintString(value);
            Console.ResetColor();
        }

        private static void EndString()
        {
            PrintString("\n");
        }

        public static void TextWriter(ConsoleColor color, string firstText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintColoredString(firstText, color);
                EndString();
            }
        }

        public static void TextWriter(ConsoleColor color, string firstText, string secondText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintColoredString(firstText, color);
                PrintString(secondText);
                EndString();
            }
        }

        public static void TextWriter(ConsoleColor color, string firstText, ConsoleColor color2, string secondText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintColoredString(firstText, color);
                PrintColoredString(secondText, color2);
                EndString();
            }
        }

        public static void TextWriter(ConsoleColor color, string firstText, ConsoleColor color2, string secondText, ConsoleColor color3, string thirdText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintColoredString(firstText, color);
                PrintColoredString(secondText, color2);
                PrintColoredString(thirdText, color3);
                EndString();
            }
        }

        public static void TextWriter(ConsoleColor color, string firstText, string secondText, string thirdText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintColoredString(firstText, color);
                PrintString(secondText);
                PrintString(thirdText);
                EndString();
            }
        }

        public static void TextWriter(ConsoleColor color, string firstText, string secondText, ConsoleColor color2, string thirdText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintColoredString(firstText, color);
                PrintString(secondText);
                PrintColoredString(thirdText, color2);
                EndString();
            }
        }

        public static void TextWriter(string firstText, ConsoleColor color, string secondText, ConsoleColor color2, string thirdText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintString(firstText);
                PrintColoredString(secondText, color);
                PrintColoredString(thirdText, color2);
                EndString();
            }
        }

        public static void TextWriter(string firstText, ConsoleColor color, string secondText, string thirdText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintString(firstText);
                PrintColoredString(secondText, color);
                PrintString(thirdText);
                EndString();
            }
        }

        public static void TextWriter(string firstText, string secondText, ConsoleColor color, string thirdText)
        {
            Log.Write(Localizer.DoStr($""));
            lock (logger)
            {
                PrintString(firstText);
                PrintString(secondText);
                PrintColoredString(thirdText, color);
                EndString();
            }
        }
    }
}
