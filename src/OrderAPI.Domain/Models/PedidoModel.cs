using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderAPI.Domain.Enums;

namespace OrderAPI.Domain.Models
{
    public class PedidoModel 
    {
        [Key]
        [Required(ErrorMessage = "Numero deve ser informado")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int Codigo { get; set; }
        
        [Required(ErrorMessage = "Data deve ser informada.")]
        public DateTime Data { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioCodigo { get; set; }

        public UsuarioModel Usuario { get; set; }

        [ForeignKey("MetodoPagamento")]
        public int MetodoPagamentoCodigo { get; set; }

        public MetodoPagamentoModel MetodoPagamento { get; set; }

        public List<PedidoItemModel> Items { get; set; }

        public PedidoStatusEnum Status { get; set; }
    }
}
