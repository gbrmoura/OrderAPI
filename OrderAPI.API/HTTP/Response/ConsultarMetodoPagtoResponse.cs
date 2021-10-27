using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.API.HTTP.Response
{

public class ConsultarMetodoPagtoResponse
    {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Status deve ser informado.")]
        public Boolean Status { get; set; }

    }
}
