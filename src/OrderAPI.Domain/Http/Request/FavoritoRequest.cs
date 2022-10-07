using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Domain.Http.Request
{
    public class FavoritoRequest
    {
        [Required(ErrorMessage = "Codigo de produto deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo de produto deve ser maior que zero.")]
        public int ProdutoCodigo { get; set; }
        
        public bool Estado { get; set; }
    }
}