using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace OrderAPI.Interfaces {
    interface IControllerBase<T> {
        public Task<ActionResult<HttpResponse>> Registrar([FromBody] T dados);
        public Task<ActionResult<HttpResponse>> Alterar([FromBody] T daods);
        public Task<ActionResult<HttpResponse>> Deletar([FromBody] int codigo);
        public Task<ActionResult<HttpResponse>> Consultar([FromBody] int codigo);
        public Task<ActionResult<HttpResponse>> ConsultarTodos();
    }
}
