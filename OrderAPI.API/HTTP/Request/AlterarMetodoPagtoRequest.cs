using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.API.HTTP.Request {
    public class AlterarMetodoPagtoRequest {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
        public string Nome { get; set; }
    }
}
