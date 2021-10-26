using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrderAPI.API.Services
{
    public static class IdentityService
    {

        public static string getActor(IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault((x) => x.Type == ClaimTypes.Actor.ToString()).Value;
        }

        public static string getRole(IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault((x) => x.Type == ClaimTypes.Role.ToString()).Value;
        }
        
    }
}