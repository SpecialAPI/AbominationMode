using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AbominationMode.Tools
{
    public class PassiveWithCountInfo(string baseID, bool includePrefixes, bool includeBasicPassive)
    {
        public readonly Regex BasicPassiveRegex = includeBasicPassive ? PassiveTools.GeneratePassiveRegex(baseID, includePrefixes) : null;
        public readonly Regex CountRegex = PassiveTools.GeneratePassiveCountRegex(baseID, includePrefixes);

        public bool TryGetCount(BasePassiveAbilitySO pa, out int rank)
        {
            rank = 0;
            if (pa == null)
                return false;

            var id = pa.name;
            if(string.IsNullOrEmpty(id))
                return false;

            if (CountRegex != null && CountRegex.Match(id) is Match m && m.Success && int.TryParse(m.Groups["count"].Value, out var r))
            {
                rank = r;
                return true;
            }

            if (BasicPassiveRegex != null && BasicPassiveRegex.IsMatch(pa.name))
            {
                rank = 1;
                return true;
            }

            rank = 0;
            return false;
        }
    }
}
