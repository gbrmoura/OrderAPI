using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.API.HTTP.Response {

    public class ConsultarCategoriaResponse {
        [Key]
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 145 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }
    }
}