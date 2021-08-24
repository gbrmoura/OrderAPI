using OrderAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Models {
    public class MPedido {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Data deve ser informada.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Status deve ser informado.")]
        public EStatus Status { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }

        public List<MPedidoItem> Items { get; set; }

        public MMetodoPagamento MetodoPagamento { get; set; }
    }
}
