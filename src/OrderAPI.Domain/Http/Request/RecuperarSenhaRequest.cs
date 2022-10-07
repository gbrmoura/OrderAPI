using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Request
{
    
    public class RecuperarSenhaRequest
    {
        [Required(ErrorMessage = "E-Mail deve ser informado.")]
        [EmailAddress(ErrorMessage = "E-Mail inválido.")]
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Email { get; set; }
    }

    public class ConfirmarSenhaRequest
    {
        [Required(ErrorMessage = "E-Mail deve ser informado.")]
        [EmailAddress(ErrorMessage = "E-Mail inválido.")]
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token de validação deve ser informado.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Nova senha deve ser informada.")]
        public string NovaSenha { get; set; }
    }


}