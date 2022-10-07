using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Domain.Http.Response
{
    public class ConsultarEstoqueResponse
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        [MaxLength(245, ErrorMessage = "O limite de 245 caractéres foi atingido.")]
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public string Tipo { get; set; }
        public ConsultarProdutoSimplesResponse Produto { get; set; }
        public ConsultarFuncionarioSimplesResponse Funcionario { get; set; }
    }
}