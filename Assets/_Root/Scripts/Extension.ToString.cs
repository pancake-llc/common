namespace Snorlax.Common
{
    using System;
    using System.Globalization;
    using UnityEngine;

    public static partial class Util
    {
        /// <summary>
        /// return string store vector2
        /// "$v2":"x:y"
        /// </summary>
        /// <param name="source"></param>
        /// <param name="numberFormat">A numeric format string.</param>
        /// <param name="formatProvider">An object that specifies culture-specific formatting.</param>
        /// <param name="resultFormat">result string format</param>
        /// <returns></returns>
        public static string ParseToString(this Vector2 source, string numberFormat, IFormatProvider formatProvider, string resultFormat = "{\"$v2\":\"{0}:{1}\"}")
        {
            if (string.IsNullOrEmpty(numberFormat)) numberFormat = "F1";

            return string.Format(resultFormat, (object) source.x.ToString(numberFormat, formatProvider), (object) source.y.ToString(numberFormat, formatProvider));
        }

        /// <summary>
        /// return string store vector2
        /// "$v2":"x:y"
        /// </summary>
        /// <param name="source"></param>
        /// <param name="numberFormat">A numeric format string.</param>
        /// <param name="resultFormat">result string format</param>
        public static string ParseToString(this Vector2 source, string numberFormat, string resultFormat) =>
            source.ParseToString(numberFormat, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, resultFormat);


        /// <summary>
        /// return string store vector2
        /// "$v2":"x:y"
        /// </summary>
        /// <param name="source"></param>
        /// <param name="numberFormat">A numeric format string.</param>
        public static string ParseToString(this Vector2 source, string numberFormat) =>
            source.ParseToString(numberFormat, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);

        /// <summary>
        /// return string store vector2
        /// "$v2":"x:y"
        /// </summary>
        /// <param name="source"></param>
        public static string ParseToString(this Vector2 source) => source.ParseToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    }
}