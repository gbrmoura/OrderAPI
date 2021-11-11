using System;

namespace OrderAPI.API.Services
{
    public static class UtilsService
    {   

        public static bool CompareStrings(string str, string[] strArray)
        {
            foreach (string s in strArray)
            {
                if (str.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
