using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.Request {

    public class CriarProdutoRequest {
        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45,  ErrorMessage = "O limite de 45 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Valor deve ser informado.")]
        public float Valor { get; set; }
        public int CategoriaCodigo { get; set; }
    }
}
