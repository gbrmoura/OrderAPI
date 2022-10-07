using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Domain.Http.Request
{
    public class PedidoItemRequest
    {
        [ForeignKey("Pedido")]
        [Required(ErrorMessage = "Produto Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int ProdutoCodigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }
}