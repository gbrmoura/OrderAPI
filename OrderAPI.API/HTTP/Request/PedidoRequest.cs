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
        [Range(1, int.MaxValue, ErrorMessage = "Codigo não deve ser menor que zero.")]
        public int MetodoPagamentoCodigo { get; set; }

        public List<PedidoItemRequest> Items { get; set; }
        
    }
}