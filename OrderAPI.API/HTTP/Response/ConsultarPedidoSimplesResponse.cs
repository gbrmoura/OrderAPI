using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderAPI.Data.Helpers;

namespace OrderAPI.API.HTTP.Response
{
    public class ConsultarPedidoSimplesResponse 
    {
        [Key]
        [Required(ErrorMessage = "Codigo deve ser informado.")]
        [Range(1, int.MaxValue, ErrorMessage = "Codigo deve ser maior que zero")]
        public int Codigo { get; set; }
        
        [Required(ErrorMessage = "Data deve ser informada.")]
        public DateTime Data { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }
        public PedidoStatusEnum Status { get; set; }
    }
}
