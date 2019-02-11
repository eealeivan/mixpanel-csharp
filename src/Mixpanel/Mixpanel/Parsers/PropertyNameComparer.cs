using System;
using System.Collections.Generic;

namespace Mixpanel.Parsers
{
    internal class PropertyNameComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(x, y);
        }

        public int GetHashCode(string obj)
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj);
        }
    }
}