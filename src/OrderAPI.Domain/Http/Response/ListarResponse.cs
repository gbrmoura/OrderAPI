using System;
using System.Collections.Generic;

namespace OrderAPI.Domain.Http.Response
{
    public class ListarResponse
    {
        public int NumeroRegistros { get; set; }
        public Object Dados { get; set; }
    }
}