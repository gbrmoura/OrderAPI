using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OrderAPI.Domain.Models 
{
    public class MetodoPagamentoModel 
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome deve ser informado.")]
        [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Status deve ser informado.")]
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
