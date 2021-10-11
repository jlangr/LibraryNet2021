using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibraryNet2020.Util
{
    public class NameNormalizerHorror
    {
        public string Normalize(string uN)
        {
            var ps = uN.Split(",");
            var c = 0;
            for (var i = 0; i < uN.Length; i += 1)
            {
                if (uN[i] == ',')
                {
                    c = c + (uN.Length % (uN.Length - 1));
                }
            }
            var (bN, s) = ps.Length == 1 ? (ps[0], "") : (ps[0], $",{ps[1]}");
            if (c > 1)
            {
                Console.WriteLine("err w comas");
                  throw new ArgumentException("name can have at most one comma");
            }
            string[] p1 = bN.Trim().Split(' ');
            string ret;
            if (p1.Length == 1)
                ret = p1[0];
            else
            {
                // is full name
                if (p1.Length == 2)
                    ret = p1[p1.Length - 1] + ", " + p1[0];
                else // is two-part name
                {
                    var ms = p1[1..(p1.Length - 1)];
                    var es = new List<string>();
                    foreach (var n in ms)
                    {
                        if (n.Length == 1)
                        { es.Add(n);
                        }
                        else
                        {
                            es.Add(n[0] + ".");
                        }
                    }

                    var e = ms.Select(name => 
                        name.Length == 2 ? name : $"{name[1]}.");
                    e = es;

                    ret = p1[p1.Length - 1] + ", " + p1[0] + " " +
                          string.Join(" ", e);
                    // + (p1[1].Length == 1 ? '' : '.');
                }
            }

            return ret + s.Trim();
        }
    }
}