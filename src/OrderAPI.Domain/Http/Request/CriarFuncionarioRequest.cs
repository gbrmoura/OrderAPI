using System.ComponentModel.DataAnnotations;
using OrderAPI.Domain.Enums;

namespace OrderAPI.Domain.Http.Request 
{
    public class CriarFuncionarioRequest 
    {
        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(115, ErrorMessage = "O limite de 115 caractéres foi atingido.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "E-Mail deve ser informado.")]
        [EmailAddress(ErrorMessage = "E-Mail inválido.")]
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Login deve ser informado.")]
        [MaxLength(50, ErrorMessage = "O limite de 50 caractéres foi atingido.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Senha deve ser informada.")]
        [MinLength(5, ErrorMessage = "O limite minimo de 5 caractéres não foi atigido.")]
        [MaxLength(40, ErrorMessage = "O limite de 40 caractéres foi atingido.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Previlégio deve ser informado.")]
        [Range(1, 3, ErrorMessage = "Privilegio deve estar entre MASTER, GERENTE, FUNCIONARIO.")]
        public PrevilegioEnum Previlegio { get; set; }
    }
}
