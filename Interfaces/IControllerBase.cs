using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace OrderAPI.Interfaces {
    interface IControllerBase<T> {
        public ActionResult<HttpResponse> Registrar([FromBody] T dados);
        public ActionResult<HttpResponse> Alterar([FromBody] T daods);
        public ActionResult<HttpResponse> Deletar(int codigo);
        public ActionResult<HttpResponse> Consultar(int codigo);
        public ActionResult<HttpResponse> ConsultarTodos();
    }
}
