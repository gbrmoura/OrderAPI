using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.API.HTTP.Request
{
    public class ListarRequest
    {
        public int TamanhoPagina { get; set; }
        public int NumeroPagina { get; set; }
        public string CampoOrdenacao { get; set; }
    }
}