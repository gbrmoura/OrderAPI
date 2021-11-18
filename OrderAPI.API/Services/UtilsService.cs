using System;

namespace OrderAPI.API.Services
{
    public class UtilsService
    {   

        public bool CompareStrings(string str, string[] strArray)
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
