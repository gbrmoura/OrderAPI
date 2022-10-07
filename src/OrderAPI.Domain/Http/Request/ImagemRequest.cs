using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Domain.Http.Request
{
    public class ImagemRequest
    {
        [Required(ErrorMessage = "Codigo do produto deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo do produto deve ser maior que zero.")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Token deve ser informado.")]
        public string Token { get; set; }
    }
}