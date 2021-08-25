using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Data.Request {

    public class LoginFuncionarioRequest {
        [Required(ErrorMessage = "Login deve ser informado.")]
        [MaxLength(50, ErrorMessage = "O limite de 50 caractéres foi atingido.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Senha deve ser informada.")]
        [MinLength(5, ErrorMessage = "O limite minimo de 5 caractéres não foi atigido.")]
        [MaxLength(40, ErrorMessage = "O limite de 40 caractéres foi atingido.")]
        public string Senha { get; set; }
    }

}
