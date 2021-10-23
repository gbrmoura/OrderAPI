using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.API.HTTP.Request
{
    public class PedidoItemRequest
    {
        [ForeignKey("Pedido")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo não deve ser menor que zero.")]
        public int ProdutoCodigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo não deve ser menor que zero.")]
        public int Quantidade { get; set; }
    }
}