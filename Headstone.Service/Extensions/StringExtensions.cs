using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Extention of string to make a paskal or camel case string to a logical text. 
        /// for example("ThisIsASample1Text => This Is A Sample 1 Text)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FirstCharToUpperCase(this string value)
        {
            value = value.ToLower();
            return value.First().ToString().ToUpper() + value.Substring(1);
        }

        public static string ToTitleCase(this string value)
        {
            value = value.ToLower();
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value);
        }

        public static string MaskCreditCard(this string value)
        {
            var firstDigits = value.Substring(0, 6);
            var lastDigits = value.Substring(value.Length - 4, 4);

            var requiredMask = new String('X', value.Length - firstDigits.Length - lastDigits.Length);

            var maskedString = string.Concat(firstDigits, requiredMask, lastDigits);

            return maskedString;
        }
    }
}
