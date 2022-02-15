using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.Models
{
    public class RecoverPasswordModel
    { 
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Email deve ser informado.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Token de validação deve ser informado.")]
        [MaxLength(8, ErrorMessage = "O limite de 8 caractéres foi atingido.")]
        public string Token { get; set; }
    }
}