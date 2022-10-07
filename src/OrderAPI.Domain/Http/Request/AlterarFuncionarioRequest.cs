using System.ComponentModel.DataAnnotations;
using OrderAPI.Domain.Enums;

namespace OrderAPI.Domain.Http.Request 
{
    public class AlterarFuncionarioRequest 
    {
        [Key]
        public int Codigo { get; set; }
        
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

        [Required(ErrorMessage = "Previlégio deve ser informado.")]
        [Range(0, 2, ErrorMessage = "Privilegio deve estar entre MASTER, GERENTE, FUNCIONARIO.")]
        public PrevilegioEnum Previlegio { get; set; }
    }
}
