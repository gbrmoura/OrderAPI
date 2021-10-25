using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Data.Models 
{
    public class MPedidoItem 
    {
        [Key]
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Valor deve ser informado.")]
        public float Valor { get; set; }

        [ForeignKey("Produto")]
        public Guid ProdutoCodigo { get; set; }

        public MProduto Produto { get; set; }
        
        [ForeignKey("Pedido")]
        public Guid PedidoCodigo { get; set; }
        
        public MPedido Pedido { get; set; }

        private bool _status = true;
        public bool Status {
            get { return _status; }
            set { _status = value; }
        }
    }
}
