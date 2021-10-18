using System;
using LibraryNet2020.Migrations;
using Microsoft.VisualBasic;

namespace LibraryNet2020.Util
{
    public class NameNormalizer
	{
        public string Normalize(string unnormalizedName)
        {
            var parts = Parts(unnormalizedName);
            if (IsMononym(parts))
                return First(parts);
            if (IsDuonym(parts))
                return Last(parts) + ", " + First(parts);
            return $"{Last(parts)}, {First(parts)} {MiddleInitial(parts)}}}";
        }

        private string MiddleInitial(string[] parts)
        {
            return Initial(Middle(parts));
        }

        private string Initial(string[] parts)
        {
            return "";
        }
        
        private string Middle(string[] parts)
        {
            return parts[1];
        }

        private bool IsDuonym(string[] parts)
        {
            return parts.Length == 2;
        }

        private string[] Parts(string name)
        {
            return name.Trim().Split(' ');
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
            return parts[1]; // hmm.
        }
    }
}