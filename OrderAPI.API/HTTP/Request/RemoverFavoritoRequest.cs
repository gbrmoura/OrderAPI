using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.API.HTTP.Request
{
    public class RemoverFavoritoRequest
    {
        [Required(ErrorMessage = "Codigo de usuario deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo de usuario deve ser maior que zero.")]
        public int UsuarioCodigo { get; set; }

        [Required(ErrorMessage = "Codigo de produto deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo de produto deve ser maior que zero.")]
        public int ProdutoCodigo { get; set; }     
    }
}