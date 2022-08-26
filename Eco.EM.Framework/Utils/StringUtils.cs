using System;

namespace Eco.EM.Framework.Utils
{
    public static class StringUtils
    {
        public static string GetAssemblyNameFromAssemblyString(string qualifiedname)
        {
            var splits = qualifiedname.Split(",", StringSplitOptions.TrimEntries);
            return splits[1];
        }

        //Sanitizing Methods

        /// <summary>
        /// Clean Just trims starting and ending whitespaces of the text parsed to it without affecting case
        /// </summary>
        /// <param name="dirty"></param>
        /// <returns></returns>
        public static string Clean(string dirty)
        {
            return dirty.Trim();
        }

        /// <summary>
        /// Sanitize will remove all Whitespaces at the start and end and return the string in lowercase
        /// </summary>
        /// <param name="dirty"></param>
        /// <returns></returns>
        public static string Sanitize(string dirty)
        {
            char[] whitespace = { ' ' };
            return dirty.TrimStart(whitespace).TrimEnd(whitespace).ToLower();
        }

        /// <summary>
        /// CleanSanitize Will remove every whitespace from a string forming a whole word from multiple does not affect case
        /// </summary>
        /// <param name="dirty"></param>
        /// <returns></returns>
        public static string CleanSanitize(string dirty)
        {
            char[] whitespace = { ' ' };
            return dirty.Replace(" ", "").TrimStart(whitespace).TrimEnd(whitespace);
        }


        /// <summary>
        /// FullClean will remove every whitespace from a string and return a lowercase string
        /// </summary>
        /// <param name="dirty"></param>
        /// <returns></returns>
        public static string FullClean(string dirty)
        {
            char[] whitespace = { ' ' };
            return dirty.Replace(" ", "").TrimStart(whitespace).TrimEnd(whitespace).ToLower();
        }
    }
}
