using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using OrderAPI.Data.Models;
using OrderAPI.API.Configurations;

namespace OrderAPI.API.Services
{

    public class TokenService 
    {
        private readonly AuthenticationConfig _configuration;
        public TokenService(AuthenticationConfig configuration) 
        {
            _configuration = configuration;
        }

        public string GenerateToken(MUsuario dados) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            Console.WriteLine("Passou por aqui");

            var key = Encoding.ASCII.GetBytes(_configuration.AccessTokenSecret);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Actor, dados.Codigo.ToString()),
                    new Claim(ClaimTypes.Name, dados.Email.ToString()),
                    new Claim(ClaimTypes.Role, "USUARIO"),
                }),
                Expires = DateTime.UtcNow.AddMinutes(Int32.Parse(_configuration.AccessTokenExpirantionMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration.Audience,
                Issuer = _configuration.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }

        public string GenerateToken(MFuncionario dados) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.AccessTokenSecret);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Actor, dados.Codigo.ToString()),
                    new Claim(ClaimTypes.Name, dados.Login.ToString()),
                    new Claim(ClaimTypes.Role, dados.Previlegio.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(Int32.Parse(_configuration.AccessTokenExpirantionMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration.Audience,
                Issuer = _configuration.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
