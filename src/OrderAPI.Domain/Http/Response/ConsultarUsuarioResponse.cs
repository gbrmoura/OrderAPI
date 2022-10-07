using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Response 
{

    public class ConsultarUsuarioResponse 
    {
        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(115, ErrorMessage = "O limite de 115 caractéres foi atingido.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Sobrenome deve ser informado.")]
        [MaxLength(145, ErrorMessage = "O limite de 145 caractéres foi atingido.")]
        public string Sobrenome { get; set; }

        [MaxLength(14, ErrorMessage = "O limite de 14 caractéres foi atigido.")]
        public string Prontuario { get; set; }

        [Required(ErrorMessage = "E-Mail deve ser informado.")]
        [EmailAddress(ErrorMessage = "E-Mail inválido.")]
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Email { get; set; }
    }

}
