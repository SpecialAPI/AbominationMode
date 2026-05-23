using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AbominationMode.Tools
{
    public class PassiveTools
    {
        private const string ModPrefixPattern = $@"(?:(?<{PrefixRegexGroup}>^[^_]+)_)?";

        public const string PrefixRegexGroup = "prefix";
        public const string CountRegexGroup = "count";

        public static Regex GeneratePassiveCountRegex(string passiveBaseName, bool includePrefixes)
        {
            var prefixInsert = string.Empty;
            if (includePrefixes)
                prefixInsert = ModPrefixPattern;

            return new Regex($@"^{prefixInsert}(?<baseName>{passiveBaseName})_(?<{CountRegexGroup}>[0-9]+)_PA$");
        }

        public static Regex GeneratePassiveRegex(string passiveBaseName, bool includePrefixes)
        {
            var prefixInsert = string.Empty;
            if (includePrefixes)
                prefixInsert = ModPrefixPattern;

            return new Regex($@"^{prefixInsert}(?<baseName>{passiveBaseName})_PA$");
        }
    }
}
