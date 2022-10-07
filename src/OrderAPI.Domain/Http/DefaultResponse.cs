using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Domain.Http
{

    public class DefaultResponse
    {
        public int Code { get; set; }
        public String Message { get; set; }
        public Object Error { get; set; }
        public Object Response { get; set; }
    }

    public class ErrorResponse
    {
        public String Field { get; set; }
        public String Message { get; set; }
    }
}
