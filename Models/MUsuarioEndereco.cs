using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Models {
    public class MUsuarioEndereco {
        [Key]
        public int Codigo { get; set; }
        
        [MaxLength(8, ErrorMessage = "O limite de 8 caractéres foi atigido.")]
        public string CEP { get; set; }
        
        [Required(ErrorMessage = "Logradouro deve ser informado.")]
        [MaxLength(115, ErrorMessage = "O limite de 115 caractéres foi atingido.")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "Numero deve ser informado.")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "Bairro deve ser informado.")]
        [MaxLength(145, ErrorMessage = "O limite de 145 caractéres foi atingido.")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Cidade deve ser informado.")]
        [MaxLength(115, ErrorMessage = "O limite de 115 caractéres foi atingido.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Estado deve ser informado.")]
        [MaxLength(2, ErrorMessage = "O limite de 2 caractéres foi atingido.")]
        public string Estado { get; set; }

    }
}
