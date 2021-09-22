using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Database;
using OrderAPI.Enums;
using OrderAPI.Models;
using OrderAPI.Services;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;


namespace OrderAPI.Controllers {

    [Route("api/cardapio")]
    public class CCardapio : ControllerBase {

        private DBContext _context;

        private IMapper _mapper;

        public CCardapio(DBContext context, IMapper mapper) {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpPost("/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<HttpResponse> Cardapio() {
            
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };


            return StatusCode(response.Code, response);
        }

    }
}