using OrderAPI.Enums;
using System;

namespace OrderAPI.Utils {
    public class SystemUtils {
        private SystemUtils() { }

        public static String Log(String message) {
            Console.WriteLine($"[{DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss")}] - {message}");
            return message;
        }

        public static String Log(ETitleLog title, String message) {
            switch (title) {
                case ETitleLog.SYSTEM: {
                    Console.WriteLine($"[System - {DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss")}] - {message}");
                    break;
                }
                case ETitleLog.DATABASE: {
                    Console.WriteLine($"[Database - {DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss")}] - {message}");
                    break;
                }
            }
            return message;
        }
    }
}
