using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Request 
{
    public class AlterarProdutoRequest 
    {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int Codigo { get; set; }
        
        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45,  ErrorMessage = "O limite de 45 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Valor deve ser informado.")]
        public float Valor { get; set; }

        [Required(ErrorMessage = "Codigo de Categoria deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int CategoriaCodigo { get; set; }
        public string Imagem { get; set; }
    }
}