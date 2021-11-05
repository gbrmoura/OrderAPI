using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.API.Services
{
    public static class ImageService
    {
        private static readonly string _caminho = Environment.CurrentDirectory + "\\Imagens\\";

        public static string SaveImage(string base64Image, string name)
        {
            string path = _caminho + name;
            System.IO.File.WriteAllBytes(path, Convert.FromBase64String(base64Image));
            return path;
        }
    }
}