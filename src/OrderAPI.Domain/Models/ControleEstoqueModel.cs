using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderAPI.Domain.Enums;

namespace OrderAPI.Domain.Models 
{
    public class ControleEstoqueModel 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public EstoqueCrontoleEnum Tipo { get; set; }
        
        [ForeignKey("Produto")]
        public int ProdutoCodigo { get; set; }
        public ProdutoModel Produto { get; set; }

        [ForeignKey("Funcionario")]
        public int FuncionarioCodigo { get; set; }
        public FuncionarioModel Funcionario { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
