using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Models 
{

    public class CategoriaModel 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 145 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }
        public List<ProdutoModel> Produtos { get; set; }

        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

    }
}
