using System;
using System.Collections.Generic;

namespace OrderAPI.API.HTTP.Response
{
    public class ListarResponse
    {
        public int NumeroRegistros { get; set; }
        public Object Dados { get; set; }
    }
}