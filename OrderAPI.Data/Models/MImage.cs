
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Data.Models 
{
    public class MImage
    {
        [Key]
        public int Codigo { get; set; }
        
        [Required(ErrorMessage = "Nome do arquivo deve ser informado.")]
        [MaxLength(60, ErrorMessage = "Nome do arquivo deve ter no máximo 60 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Caminho do arquivo deve ser informado.")]
        public string Caminho { get; set; }
        
        [ForeignKey("Produto")]
        public int ProductCodigo { get; set; }
        public MProduto Produto { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}