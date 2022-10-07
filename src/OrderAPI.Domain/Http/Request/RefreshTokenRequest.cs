using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Domain.Http.Request
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Token deve ser informado.")]
        public string Token { get; set; }
        
        [Required(ErrorMessage = "Refresh Token deve ser informado.")]
        public string RefreshToken { get; set; }
    }
}