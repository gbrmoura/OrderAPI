using System;

namespace OrderAPI.Services {

    public static class PasswordService {

        public static String EncryptPassword(String password) {
            return BCrypt.Net.BCrypt.HashPassword(password);
        } 

        public static bool VerifyPassword(String password, String verifyPassword) {
            return BCrypt.Net.BCrypt.Verify(password, verifyPassword);
        }

        public static Boolean ComparePassword(String password, string confirmPassword) {
            return password.Trim().Equals(confirmPassword.Trim());
        }

    }
}
