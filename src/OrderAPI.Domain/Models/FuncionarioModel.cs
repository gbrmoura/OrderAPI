using OrderAPI.Domain.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Models 
{
    public class FuncionarioModel 
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

        [Required(ErrorMessage = "Senha deve ser informada.")]
        [MinLength(5, ErrorMessage = "O limite minimo de 5 caractéres não foi atigido.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Previlégio deve ser informado.")]
        public PrevilegioEnum Previlegio { get; set; }
        public Guid Token { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

    }
}
