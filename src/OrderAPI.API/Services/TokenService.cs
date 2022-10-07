using System;
using System.Text;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using OrderAPI.Domain.Models;
using OrderAPI.API.Configurations;
using System.Collections.Generic;
using OrderAPI.Data;

namespace OrderAPI.API.Services
{

    public class TokenService 
    {
        private AuthenticationConfig configuration;
        private OrderAPIContext context;

        public TokenService(AuthenticationConfig configuration, OrderAPIContext context) 
        {
            this.context = context;
            this.configuration = configuration;
        }

        public string GenerateToken(IEnumerable<Claim> claims) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.configuration.AccessTokenSecret);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Int32.Parse(this.configuration.AccessTokenExpirantionMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public bool IsValidCurrentToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(this.configuration.AccessTokenSecret);
            var mySecurityKey = new SymmetricSecurityKey(key);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = mySecurityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token) 
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.configuration.AccessTokenSecret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }

        public string GenerateRefreshToken() => Guid.NewGuid().ToString();
        
        public void SaveRefreshToken(Guid actor, string refreshToken, string token)
        {
            var dbToken = new TokenModel() { 
                Actor = actor, 
                RefreshToken = refreshToken,
                Token = token
            };

            this.context.Token.Add(dbToken);
            this.context.SaveChanges();
        }

        public string GetRefreshToken(Guid actor)
        {
            var token = this.context.Token.FirstOrDefault((token) => token.Actor == actor);
            return (token != null ? token.RefreshToken : string.Empty);
        }

        public void DeleteRefreshToken(Guid actor) 
        {
            if (this.context.Token.Any((x) => x.Actor == actor))
            {
                var token = this.context.Token.FirstOrDefault((x) => x.Actor == actor);
                this.context.Token.Remove(token);
                this.context.SaveChanges();
            }
        }

        public bool IsValidRefreshToken(string refreshToken, string token) 
        {
            return this.context.Token.Any((x) => x.RefreshToken == refreshToken && x.Token == token);
        }
    }
}
