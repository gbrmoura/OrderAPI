using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Domain.Http.Request
{
    public class PedidoRequest
    {
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }
        
        [ForeignKey("MetodoPagamento")]
        [Required(ErrorMessage = "Metodo de Pagamento Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int MetodoPagamentoCodigo { get; set; }    

        [Required(ErrorMessage = "Produtos necessarios.")]
        public List<PedidoItemRequest> Items { get; set; }
        
    }
}