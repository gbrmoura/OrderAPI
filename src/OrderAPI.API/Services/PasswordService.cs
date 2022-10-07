using System;
using System.Linq;
using System.Text;
using OrderAPI.Data;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Services
{

    public class PasswordService
    {
        private readonly OrderAPIContext _context;

        public PasswordService(OrderAPIContext context)
        {
            this._context = context;
        }

        public string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }

        public bool ComparePassword(string password, string confirmPassword)
        {
            return password.Trim().Equals(confirmPassword.Trim());
        }

        
        public string GenerateRecoverToken(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            StringBuilder strBuilder = new StringBuilder();  
            Random random = new Random();  

            char letter;  

            for (int i = 0; i < 8; i++)
            {
                int shift = Convert.ToInt32(Math.Floor(25 * random.NextDouble()));
                letter = Convert.ToChar(shift + 65);
                strBuilder.Append(letter);  
            }

            var recoverToken = new RecoverPasswordModel
            {
                Email = email,
                Token = strBuilder.ToString(),
                ExpirationDate = DateTime.Now.AddDays(1)
            };

            _context.RecoverPassword.Add(recoverToken);
            _context.SaveChanges();

            return recoverToken.Token;
        }

        public bool VerifyRecoverToken(string token, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token");
            }

            var recoverToken = _context.RecoverPassword.FirstOrDefault(x => x.Token == token && x.Email == email);
            var date = DateTime.Now;

            if (recoverToken == null)
            {
                return false;
            }

            if (recoverToken.ExpirationDate < date.AddMinutes(-5) && recoverToken.ExpirationDate > date.AddMinutes(5))
            {
                _context.RecoverPassword.Remove(recoverToken);
                _context.SaveChanges();

                return false;
            }

            _context.RecoverPassword.Remove(recoverToken);
            _context.SaveChanges();

            return true;
        }

        
    }
}
