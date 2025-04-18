using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScsUnitDecrypt.Core.Utils
{
    internal static class SiiUtils
    {
        private const string ValidLetters = "\00123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";

        // https://stackoverflow.com/a/53337761
        /// <summary>
        ///     Converts a ulong to a formatted address string, by inserting a '.' every 4th char from the end
        /// </summary>
        /// <param name="value">Memory address as ulong</param>
        /// <returns>Formatted string: e.g: _nameless.xxx.xxxx.xxxx.xxxx</returns>
        public static string FormatMemoryAddress(ulong value)
        {
            var formattedValue = Regex.Replace(value.ToString("x"), "(?=(?:.{4})+$)", ".");
            if (formattedValue.StartsWith(".")) formattedValue = formattedValue.Substring(1);
            return $"_nameless.{formattedValue}";
        }

        // https://stackoverflow.com/a/2751597 & https://stackoverflow.com/a/35450540
        /// <summary>
        ///     Turns floats to hex string unless solid number and smaller than 10000000f (otherwise it's written with scientific
        ///     notification)
        /// </summary>
        /// <param name="val">float to format</param>
        /// <returns></returns>
        public static string FloatToSiiFormat(float val)
        {
            if (Math.Abs(val % 1) <= double.Epsilon * 100 && val < 10000000f)
                return val.ToString(CultureInfo.InvariantCulture);
            var i = BitConverter.ToUInt32(BitConverter.GetBytes(val), 0);
            return $"&{i:x8}";
        }

        /// <summary>
        ///     Add quotes if string has chars not allowed
        /// </summary>
        /// <param name="val">String to format</param>
        /// <returns></returns>
        public static string StringToSiiFormat(string val)
        {
            if (val == "") return "\"\"";
            return val.Any(c => !ValidLetters.Contains(c)) ? $"\"{val}\"" : val;
        }
    }
}
