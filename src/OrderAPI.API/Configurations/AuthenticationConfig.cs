using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.API.Configurations
{
    public class AuthenticationConfig
    {
        public string AccessTokenSecret { get; set; }
        public string AccessTokenExpirantionMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}