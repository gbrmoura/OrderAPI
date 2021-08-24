using OrderAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.DTO {
    public class DTOConsultarUsuario {
        [Key]
        public int Codigo { get; set; }
        [Required(ErrorMessage = "O nome deve ser preenchido!")]
        [MaxLength(80, ErrorMessage = "O limite de Caractéres do Nome foi atingido")]
        public String Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome deve ser preenchido!")]
        [MaxLength(110, ErrorMessage = "O limite de Caractéres do Sobrenome foi atingido")]
        public String Sobrenome { get; set; }

        [Required(ErrorMessage = "O email deve ser preenchido!")]
        [EmailAddress(ErrorMessage = "Endereço de E-mail inválido")]
        [MaxLength(245, ErrorMessage = "O limite de Caractéres do email foi atingido")]
        public String Email { get; set; }
    }
}
