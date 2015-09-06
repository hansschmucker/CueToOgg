using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CueToOgg
{
    public static class StringExtensions
    {
        public static String TrimTo(this String str, Char trimEndChar)
        {
            return (new Regex("^[^\\"+ trimEndChar+"]*\\"+ trimEndChar +"|\\"+ trimEndChar+"[^\\"+ trimEndChar+"]*$")).Replace(str, "");
        }
    }
}
