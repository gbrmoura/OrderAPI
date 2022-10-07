using System;
using System.Security.Claims;
using System.Security.Principal;

namespace OrderAPI.API.EntensionMethods
{
    public static class IdentityService
    {

        public static string GetUsuarioCodigo(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("codigo");
            return claim?.Value;
        }

        public static string GetUsuarioPrivilegio(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Role);
            return claim?.Value;
        }

        public static string GetUsuarioNome(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("nome");
            return claim?.Value;
        }

        public static string GetUsuarioLogin(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("login");
            return claim?.Value;
        }

        public static string GetUsuarioToken(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("token");
            return claim?.Value;
        }
        
    }
}
