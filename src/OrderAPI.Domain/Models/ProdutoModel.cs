using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Domain.Models
{
    public class ProdutoModel 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45,  ErrorMessage = "O limite de 45 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informado.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Valor deve ser informado.")]
        public float Valor { get; set; }

        [ForeignKey("CategoriaCodigo")]
        public int CategoriaCodigo { get; set; }
        public CategoriaModel Categoria { get; set; }

        [ForeignKey("ImageCodigo")]
        public int ImageCodigo { get; set; }
        public ImageModel Imagem { get; set; }
        public List<FavoritoModel> Favoritos { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
