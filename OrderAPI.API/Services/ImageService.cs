using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderAPI.API.Services
{
    public static class ImageService
    {
        private static readonly string _caminho = Environment.CurrentDirectory + "\\Imagens\\";
        private static readonly string regExBase64 = "/^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$/";

        public static bool ValidarBase64(string base64)
        {
            if (String.IsNullOrEmpty(base64))
                return false;

            if (base64.Contains("base64,"))
                return true;

            return false;
        }

        public static string SaveImage(string base64Image, string name)
        {

            if (!Directory.Exists(_caminho))
            {
                Directory.CreateDirectory(_caminho);
            }

            var path = _caminho + name;
            var imageDataBytes = Convert.FromBase64String(base64Image.Split(',')[1]);
            System.IO.File.WriteAllBytes(path, imageDataBytes);
            return path;
        }
    }
}