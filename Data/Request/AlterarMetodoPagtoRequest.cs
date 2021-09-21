using OrderAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.Request {
    public class AlterarMetodoPagtoRequest {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
        public string Nome { get; set; }
        
    }
}
