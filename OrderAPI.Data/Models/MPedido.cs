using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderAPI.Data.Helpers;

namespace OrderAPI.Data.Models 
{
    public class MPedido 
    {
        [Key]
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "Data deve ser informada.")]
        public DateTime Data { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }

        [ForeignKey("Usuario")]
        public Guid UsuarioCodigo { get; set; }

        public MUsuario Usuario { get; set; }

        [ForeignKey("MetodoPagamento")]
        public Guid MetodoPagamentoCodigo { get; set; }

        public MMetodoPagamento MetodoPagamento { get; set; }

        public List<MPedidoItem> Items { get; set; }

        public PedidoStatusEnum Status { get; set; }
    }
}
