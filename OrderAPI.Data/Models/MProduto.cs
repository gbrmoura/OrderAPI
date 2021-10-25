﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Data.Models 
{
    public class MProduto 
    {
        [Key]
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45,  ErrorMessage = "O limite de 45 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informado.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Valor deve ser informado.")]
        public float Valor { get; set; }
        private bool _status = true;
        public bool Status {
            get { return _status; }
            set { _status = value; }
        }

        [ForeignKey("CategoriaCodigo")]
        public Guid CategoriaCodigo { get; set; }
        public MCategoria Categoria { get; set; }
    }
}
