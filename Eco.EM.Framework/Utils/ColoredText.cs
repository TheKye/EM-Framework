﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Eco.EM.Framework.Utils
{
    public static class ColoredText
    {
        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string RedText(string msg)
        {
            return $"<color=red>{msg}</color>";
        }

        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string BlueText(string msg)
        {
            return $"<color=blue>{msg}</color>";
        }

        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string YellowText(string msg)
        {
            return $"<color=yellow>{msg}</color>";
        }

        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string GreenText(string msg)
        {
            return $"<color=green>{msg}</color>";
        }

        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string CustomText(string color, string msg)
        {
            return $"<color={color}>{msg}</color>";
        }

        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string CustomMultiText(string color, string msg, string color2, string msg2)
        {
            return $"<color={color}>{msg}</color> <color={color2}>{msg2}</color>";
        }

        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string CustomMultiText(string color, string msg, string color2, string msg2, string color3, string msg3 )
        {
            return $"<color={color}>{msg}</color> <color={color2}>{msg2}</color> <color={color3}>{msg3}</color>";
        }

        [Obsolete("Please use Eco.EM.Framework.Text, this will be removed in a later release")]
        public static string CustomMultiText(string color, string msg, string color2, string msg2, string color3, string msg3, string color4, string msg4)
        {
            return $"<color={color}>{msg}</color> <color={color2}>{msg2}</color> <color={color3}>{msg3}</color> <color={color4}>{msg4}</color>";
        }

    }
}