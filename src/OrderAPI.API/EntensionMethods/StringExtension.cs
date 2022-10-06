using System;

namespace OrderAPI.API.EntensionMethods
{
    public static class StringExtension 
    {
        public static string Capitalize(this string value)
        {
            if (value.Length == 0)
                return value;
            else if (value.Length == 1)
                return char.ToUpper(value[0]) + "";
            else
                return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}