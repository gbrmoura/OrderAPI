using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.API.HTTP.Response 
{
    public class ConsultarCardapioCategoriaResponse 
    {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Titulo deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 145 caractéres foi atingido.")]
        public string Titulo { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Descricao { get; set; }
        
        public List<ConsultarProdutoSimplesResponse> Produtos { get; set; }
    }
}