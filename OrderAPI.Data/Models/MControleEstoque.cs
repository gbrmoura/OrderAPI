﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Data.Models 
{
    public class MControleEstoque 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }
        public DateTime Data { get; set; }

        [ForeignKey("Produto")]
        public int ProdutoCodigo { get; set; }
        public MProduto Produto { get; set; }

        [ForeignKey("Funcionario")]
        public int FuncionarioCodigo { get; set; }
        public MFuncionario Funcionario { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
