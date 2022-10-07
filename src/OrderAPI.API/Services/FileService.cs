using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace OrderAPI.API.Services
{
    public class FileService
    {
        private string path;
        
        public FileService(IConfiguration config) 
        {
            this.path = config["FilePath"]; 
        }

        public bool IsValidBase64(string base64)
        {
            if (String.IsNullOrEmpty(base64))
                return false;

            if (base64.Contains("base64,"))
                return true;

            return false;
        }

        public string SaveFile(string buffer, string name, string  extension)
        {
            if (!Directory.Exists(this.path))
                Directory.CreateDirectory(this.path);

            var path = $"{ this.path }{ name }.{ extension }";
            var file = Convert.FromBase64String(buffer.Split(',')[1]);
            System.IO.File.WriteAllBytes(path, file);
            return path;
        }
    }
}