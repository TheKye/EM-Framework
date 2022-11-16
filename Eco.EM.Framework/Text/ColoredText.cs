using System;
using System.Collections.Generic;
using System.Text;

namespace Eco.EM.Framework.Text
{
    /// <summary>
    /// String Extension to add colors directly to the string without needing to use the methods 
    /// Works for single color only and does not work in the console
    /// </summary>
    public static class @string
    {
        public static string Red(this string msg) => $"<color=red>{msg}</color>";

        public static string Blue(this string msg) => $"<color=blue>{msg}</color>";

        public static string Yellow(this string msg) => $"<color=yellow>{msg}</color>";

        public static string Green(this string msg) => $"<color=green>{msg}</color>";
        public static string Orange(this string msg) => $"<color=#ffa31a>{msg}</color>";

        public static string White(this string msg) => $"<color=#ffffff>{msg}</color>";

        public static string Success(this string msg) => Green(msg);

        public static string Error(this string msg) => Red(msg);

        public static string Warning(this string msg) => Yellow(msg);
    }

    public static class ColoredText
    {
        public static string RedText(string msg) => $"<color=red>{msg}</color>";

        public static string BlueText(string msg) => $"<color=blue>{msg}</color>";

        public static string YellowText(string msg) => $"<color=yellow>{msg}</color>";

        public static string GreenText(string msg) => $"<color=green>{msg}</color>";

        public static string CustomText(string color, string msg) => $"<color={color}>{msg}</color>";

        public static string CustomMultiText(string color, string msg, string color2, string msg2) => $"<color={color}>{msg}</color> <color={color2}>{msg2}</color>";

        public static string CustomMultiText(string color, string msg, string color2, string msg2, string color3, string msg3) => $"<color={color}>{msg}</color> <color={color2}>{msg2}</color> <color={color3}>{msg3}</color>";

        public static string CustomMultiText(string color, string msg, string color2, string msg2, string color3, string msg3, string color4, string msg4) => $"<color={color}>{msg}</color> <color={color2}>{msg2}</color> <color={color3}>{msg3}</color> <color={color4}>{msg4}</color>";

        public static string MultiText(Dictionary<string, string> colorsAndValues)
        {
            StringBuilder sb = new();

            foreach (var cv in colorsAndValues)
            {
                sb.Append($"<color={cv.Key}>{cv.Value}</color> ");
            }

            return sb.ToString();
        }
    }
}