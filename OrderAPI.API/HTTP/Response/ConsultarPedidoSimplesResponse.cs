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
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "Numero deve ser informado")]
        [Range(1, int.MaxValue, ErrorMessage = "Numero deve estar entre 1 e N")]
        public int Numero { get; set; }
        
        [Required(ErrorMessage = "Data deve ser informada.")]
        public DateTime Data { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }
        public PedidoStatusEnum Status { get; set; }
    }
}
