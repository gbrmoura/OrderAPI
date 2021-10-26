﻿using System;
using System.Text;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using OrderAPI.Data.Models;
using OrderAPI.API.Configurations;
using System.Collections.Generic;
using System.Security.Cryptography;
using OrderAPI.Data;

namespace OrderAPI.API.Services
{

    public class TokenService 
    {
        private readonly AuthenticationConfig _configuration;

        private OrderAPIContext _context;
        private List<(Guid, string)> _refreshtokens = new();

        public TokenService(AuthenticationConfig configuration, OrderAPIContext context) 
        {
            _context = context;
            _configuration = configuration;
        }

        public string GenerateToken(IEnumerable<Claim> claims) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.AccessTokenSecret);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Int32.Parse(_configuration.AccessTokenExpirantionMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                // Audience = _configuration.Audience,
                // Issuer = _configuration.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken() 
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) 
            {
                rng.GetBytes(randomNumber);
            };
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token) 
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.AccessTokenSecret)),
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
    
        public void SaveRefreshToken(Guid actor, string refreshToken)
        {
            _context.Token.Add(new MToken()
            {
                Actor = actor,
                RefreshToken = refreshToken
            });
            _context.SaveChanges();
        }

        public string GetRefreshToken(Guid actor)
        {
            return _context.Token.FirstOrDefault((x) => x.Actor == actor).RefreshToken;
        }

        public void DeleteRefreshToken(Guid actor, string refreshToken) 
        {
            var token = _context.Token.FirstOrDefault((x) => x.Actor == actor && x.RefreshToken == refreshToken);
            _context.Token.Remove(token);
        }
    }
}
