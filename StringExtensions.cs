using System;
using System.Text.RegularExpressions;

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
