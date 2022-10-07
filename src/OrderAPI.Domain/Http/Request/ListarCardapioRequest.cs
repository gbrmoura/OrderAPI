using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Request
{
    public class ListarCardapioRequest
    {
        [Required(ErrorMessage = "Tamanho de pagina deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tamanho de pagina deve ser maior que zero")]
        public int TamanhoPagina { get; set; }
        
        [Required(ErrorMessage = "Numero de pagina deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Numero de pagina deve ser maior que zero.")]
        public int NumeroPagina { get; set; }
        public string CampoPesquisa { get; set; }
        public bool Favorito { get; set; }
    }
}