using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using OrderAPI.Data.Helpers;

namespace OrderAPI.API.HTTP.Request
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