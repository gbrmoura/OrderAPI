using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Models 
{
    public class UsuarioModel 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(115, ErrorMessage = "O limite de 115 caractéres foi atingido.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Sobrenome deve ser informado.")]
        [MaxLength(145, ErrorMessage = "O limite de 145 caractéres foi atingido.")]
        public string Sobrenome { get; set; }

        [MaxLength(14, ErrorMessage = "O limite de 14 caractéres foi atigido.")]
        public string Prontuario { get; set; }

        [Required(ErrorMessage = "Senha deve ser informada.")]
        [MinLength(5, ErrorMessage = "O limite minimo de 5 caracéres não foi atigido.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "E-Mail deve ser informado.")]
        [EmailAddress(ErrorMessage = "E-Mail inválido.")]
        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Email { get; set; }
        public Guid Token { get; set; }
        public List<FavoritoModel> Favoritos { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
