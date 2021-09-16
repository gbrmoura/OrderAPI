using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.Request {
    public class AlterarCategoriaRequest {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 145 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }
    }
}