using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Response
{

    public class ConsultarMetodoPagtoResponse
    {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
        public string Titulo { get; set; }
    }
}
