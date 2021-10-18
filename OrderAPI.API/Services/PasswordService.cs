using System;

namespace OrderAPI.API.Services {

    public static class PasswordService {

        public static string EncryptPassword(string password) {
            return BCrypt.Net.BCrypt.HashPassword(password);
        } 

        public static bool VerifyPassword(string password, string hashPassword) {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }

        public static bool ComparePassword(string password, string confirmPassword) {
            return password.Trim().Equals(confirmPassword.Trim());
        }

    }
}
