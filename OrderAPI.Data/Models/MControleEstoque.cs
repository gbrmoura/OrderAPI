using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.Models 
{
    public class MControleEstoque 
    {
        [Key]
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string observacao { get; set; }
    }
}
