using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderAPI.Models;

namespace OrderAPI.Data.Response {
    public class ConsultarCardapioProdutoResponse {
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

        public ConsultarCategoriaResponse Categoria { get; set; }
    
    }
}