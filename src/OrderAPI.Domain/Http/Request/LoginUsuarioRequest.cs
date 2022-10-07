using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Request 
{
    public class LoginUsuarioRequest 
    {

        [Required(ErrorMessage = "E-Mail deve ser informado.")]
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Senha deve ser informada.")]
        [MinLength(5, ErrorMessage = "O limite minimo de 5 caractéres não foi atigido.")]
        public string Senha { get; set; }

    }
}
