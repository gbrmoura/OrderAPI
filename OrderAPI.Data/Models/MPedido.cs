using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Data.Models 
{
    public class MPedido 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Data deve ser informada.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Status deve ser informado.")]

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }

        public List<MPedidoItem> Items { get; set; }
        
        [ForeignKey("MetodoPagamento")]
        public int MetodoPagamentoCodigo { get; set; }

        public MMetodoPagamento MetodoPagamento { get; set; }

        private bool _status = true;
        public bool Status {
            get { return _status; }
            set { _status = value; }
        }
    }
}
