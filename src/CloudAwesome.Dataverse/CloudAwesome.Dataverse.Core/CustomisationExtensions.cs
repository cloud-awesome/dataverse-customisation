using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Core
{
    public static class CustomisationExtensions
    {
        public static string GenerateLogicalNameFromDisplayName(this string displayName, string publisherPrefix, bool isLookupAttribute = false)
        {
            var validNameChars = new Regex("[A-Z0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(publisherPrefix))
            {
                result.AppendFormat("{0}_", publisherPrefix);
            }
            foreach (var match in validNameChars.Matches(displayName))
            {
                result.Append(match);
            }

            if (isLookupAttribute && (displayName.Substring(displayName.Length - 2) != "id"))
            {
                result.Append("id");
            }

            return result.ToString().ToLower().Trim();
        }

        public static Label CreateLabelFromString(this string displayString, int languageCode = 1033)
        {
            return new Label(displayString, languageCode);
        }

        /// <summary>
        /// Does a basic form of pluralisation to catch when a plural form should have been included in the data but hasn't.
        /// Catches the basics but not the irregulars!
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        public static string Pluralise(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var returnValue = "";

            var lastChar = value.Substring(value.Length - 1);
            switch (lastChar)
            {
                case "s":
                case "x":
                    returnValue = $"{value}es";
                    break;
                case "y":
                    returnValue = $"{value.Substring(0, value.Length - 1)}ies";
                    break;
                case "f":
                    returnValue = $"{value.Substring(0, value.Length - 1)}ves";
                    break;
                default:
                    returnValue = $"{value}s";
                    break;
            }

            return returnValue;
        }
    }
}
