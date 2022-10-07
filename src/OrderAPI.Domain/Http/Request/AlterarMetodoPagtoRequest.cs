using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Request
{
    public class AlterarMetodoPagtoRequest 
    {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
        public string Titulo { get; set; }
    }
}
