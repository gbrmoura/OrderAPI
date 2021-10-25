using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.API.HTTP.Request
{
    public class PedidoRequest
    {
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Obersavacao { get; set; }

        [ForeignKey("MetodoPagamento")]
        [Required(ErrorMessage = "Metodo de Pagamento Codigo deve ser informado.")]
        public Guid MetodoPagamentoCodigo { get; set; }

        [ForeignKey("Usuario")]
        [Required(ErrorMessage = "Usuario Codigo deve ser informado.")]
        public Guid UsuarioCodigo { get; set; }

        [Required(ErrorMessage = "Produtos necessarios.")]
        public List<PedidoItemRequest> Items { get; set; }
        
    }
}