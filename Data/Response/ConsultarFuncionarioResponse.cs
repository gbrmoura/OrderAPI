using OrderAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.Response {

    public class ConsultarFuncionarioResponse {
        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(115, ErrorMessage = "O limite de 115 caractéres foi atingido.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Login deve ser informado.")]
        [MaxLength(50, ErrorMessage = "O limite de 50 caractéres foi atingido.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Senha deve ser informada.")]
        [MinLength(5, ErrorMessage = "O limite minimo de 5 caractéres não foi atigido.")]
        [MaxLength(40, ErrorMessage = "O limite de 40 caractéres foi atingido.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Previlégio deve ser informado.")]
        public EPrevilegio Previlegio { get; set; }
    }
}
