using System.ComponentModel.DataAnnotations;

namespace OrderAPI.API.HTTP.Request 
{
    public class CriarMetodoPagtoRequest 
    {

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
        public string Titulo { get; set; }
        
    }
}
