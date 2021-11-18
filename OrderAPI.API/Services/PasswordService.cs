using System;

namespace OrderAPI.API.Services {

    public class PasswordService {

        public string EncryptPassword(string password) {
            return BCrypt.Net.BCrypt.HashPassword(password);
        } 

        public bool VerifyPassword(string password, string hashPassword) {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }

        public bool ComparePassword(string password, string confirmPassword) {
            return password.Trim().Equals(confirmPassword.Trim());
        }
    }
}
