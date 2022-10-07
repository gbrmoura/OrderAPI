using System;
using System.ComponentModel.DataAnnotations;
using OrderAPI.Domain.Enums;

namespace OrderAPI.Domain.Http.Request
{
    public class ListarPedidosRequest
    {
        [Required(ErrorMessage = "Tamanho de pagina deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tamanho de pagina deve ser maior que um.")]
        public int TamanhoPagina { get; set; }

        [Required(ErrorMessage = "Numero de pagina deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Numero de pagina deve ser maior que um.")]
        public int NumeroPagina { get; set; }
        public string CampoPesquisa { get; set; }
        public PedidoStatusEnum Status { get; set; }

    }
}