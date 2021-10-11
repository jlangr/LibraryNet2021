using System.Linq;
using System.Collections.Generic;

namespace LibraryNet2020.Util
{
    public class NameNormalizer
	{
        public string Normalize(string unnormalizedName)
        {
            var parts = Parts(unnormalizedName.Trim());
            if (IsMononym(parts))
                return unnormalizedName;
            if (IsDuonym(parts))
                return Last(parts) + ", " + First(parts);
            return $"{Last(parts)}, {First(parts)} {MiddleInitials(parts)}";
        }

        private string MiddleInitials(string[] parts)
        {
            var middleNames = parts.Skip(1).Take(parts.Length - 2);
            var middleInitials = middleNames.Select(name => Initial(name));
            return string.Join(' ', middleInitials);
        }

        private string Initial(string name)
        {
            return name.Length == 1 
                ? name 
                : $"{name.Substring(0, 1)}.";
        }

        private static string MiddleName(string[] parts)
        {
            return parts[1];
        }

        private bool IsDuonym(string[] parts)
        {
            return parts.Length == 2;
        }

        private static string[] Parts(string unnormalizedName)
        {
            return unnormalizedName.Split(' ');
        }

        private static bool IsMononym(string[] parts)
        {
            return parts.Length == 1;
        }

        private static string First(string[] parts)
        {
            return parts[0];
        }

        private static string Last(string[] parts)
        {
            return parts[parts.Length - 1];
        }
    }
}