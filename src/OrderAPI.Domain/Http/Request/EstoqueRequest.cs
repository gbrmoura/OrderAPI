using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Domain.Http.Request
{
    public class EstoqueRequest
    {   
        [Required(ErrorMessage = "O codigo do produto deve ser informado")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo do produto deve ser maior que zero.")]
        public int ProdutoCodigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser infomada.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
        
        [Required(ErrorMessage = "O tipo de movimento deve ser infomado.")]	
        public string Tipo { get; set; }
        public string Observacao { get; set; }
    }
}