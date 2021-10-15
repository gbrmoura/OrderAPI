﻿using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Services
{

    public class TokenService {

        private IConfiguration _configuration { get; }

        public TokenService(IConfiguration configuration) {
            _configuration = configuration;
        }

        public string GenerateToken(MUsuario dados) {
            var tokenHandler = new JwtSecurityTokenHandler();
            Console.WriteLine("Passou por aqui");

            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Actor, dados.Codigo.ToString()),
                    new Claim(ClaimTypes.Email, dados.Email.ToString()),
                    new Claim(ClaimTypes.Role, "USUARIO"),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }

        public string GenerateToken(MFuncionario dados) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Actor, dados.Codigo.ToString()),
                    new Claim(ClaimTypes.Name, dados.Login.ToString()),
                    new Claim(ClaimTypes.Role, dados.Previlegio.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}