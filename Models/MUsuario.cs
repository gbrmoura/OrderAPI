using OrderAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Models {
    public class MUsuario {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O nome deve ser preenchido!")]
        [MaxLength(80, ErrorMessage = "O limite de Caractéres do Nome foi atingido")]
        public String Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome deve ser preenchido!")]
        [MaxLength(110, ErrorMessage = "O limite de Caractéres do Sobrenome foi atingido")]
        public String Sobrenome { get; set; }

        [Required(ErrorMessage = "O email deve ser preenchido!")]
        [MaxLength(245, ErrorMessage = "O limite de Caractéres do email foi atingido")]
        [EmailAddress(ErrorMessage = "Endereço de E-mail inválido")]
        public String Email { get; set; }

        [Required(ErrorMessage = "A senha deve ser preenchida!")]
        [MinLength(5, ErrorMessage = "A senha deve possuir no minimo 5 dígitos!")]
        public String Senha { get; set; }
        public Guid Token { get; set; }
        public EPrevilegios NivelAcesso { get; set; }
        public DateTime DataCadastro { get; set; }
        public EStatusRegistro Status { get; set; }
    }
}
