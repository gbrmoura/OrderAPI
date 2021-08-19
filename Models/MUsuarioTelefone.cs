using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Models {
    public class MUsuarioTelefone {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "DDD deve ser informado.")]
        [MaxLength(2, ErrorMessage = "O limite de 2 caractéres foi atingido.")]
        public string DDD { get; set; }

        [Required(ErrorMessage = "Telefone deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atingido.")]
        public string Telefone { get; set; }
    }
}
