using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Data.Models
{
    public class MFavorito
    {
        [Key]
        public int Codigo { get; set; }
        
        [ForeignKey("Produto")]
        public int ProdutoCodigo { get; set; }
        public MProduto Produto { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioCodigo { get; set; }
        public MUsuario Usuario { get; set; }
        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}