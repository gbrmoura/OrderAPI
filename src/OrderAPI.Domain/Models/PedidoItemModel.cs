using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Domain.Models
{
    public class PedidoItemModel 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Valor deve ser informado.")]
        public float Valor { get; set; }

        [ForeignKey("Produto")]
        public int ProdutoCodigo { get; set; }

        public ProdutoModel Produto { get; set; }
        
        [ForeignKey("Pedido")]
        public int PedidoCodigo { get; set; }
        public PedidoModel Pedido { get; set; }
         private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
