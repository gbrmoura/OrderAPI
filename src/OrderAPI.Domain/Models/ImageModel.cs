
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Domain.Models
{
    public class ImageModel
    {
        [Key]
        public int Codigo { get; set; }
        
        [Required(ErrorMessage = "Nome do arquivo deve ser informado.")]
        [MaxLength(60, ErrorMessage = "Nome do arquivo deve ter no máximo 60 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Tipo do arquivo deve ser informado.")]
        [MaxLength(5, ErrorMessage = "Tipo do arquivo deve ter no máximo 5 caracteres.")]
        public string Extensao { get; set; }

        [Required(ErrorMessage = "Caminho do arquivo deve ser informado.")]
        public string Caminho { get; set; }
        
        [ForeignKey("Produto")]
        public int ProductCodigo { get; set; }
        public ProdutoModel Produto { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}