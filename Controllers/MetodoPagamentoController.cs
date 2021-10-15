using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.Data;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class MetodoPagamentoController : ControllerBase
    {
        
        private OrderAPIContext _context;

        private IMapper _mapper;

        public MetodoPagamentoController(OrderAPIContext context, IMapper mapper)
        {   
            _context = context;
            _mapper = mapper;
        }


        // TODO: Registrar
        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarMetodoPagtoRequest body) 
        {
            return NotFound();
        }

        // TODO: Alterar
        [HttpPost("Alterar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarMetodoPagtoRequest body)
        {
            return NotFound();
        }

        // TODO: Deletar
        [HttpGet("Deletar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Deletar([FromQuery] int codigo)
        {
            return NotFound();
        }

        // TODO: Consultar
        [HttpGet("Consultar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Consultar([FromQuery] int codigo)
        {
            return NotFound();
        }

        // TODO: Listar
        [HttpGet("Listar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Listar([FromQuery] ListarRequest query) 
        {
            return NotFound();
        }

    }
}