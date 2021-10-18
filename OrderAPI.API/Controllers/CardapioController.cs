using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Data;
using AutoMapper;
using OrderAPI.API.Services;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.Data.Models;
using OrderAPI.Data.Helpers;
using OrderAPI.API.Helpers;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class CardapioController : ControllerBase
    {
        private OrderAPIContext _context;

        private IMapper _mapper;

        public CardapioController(OrderAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet("Completo/")]
    	[Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Cardapio() 
        {
            return NotFound();
        }

        [HttpGet("Categorias/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Categorias ()
        {
            return NotFound();
        }

        [HttpGet("Categoria/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Categoria()
        {
            return NotFound();
        }

        [HttpGet("Produtos/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Produtos ()
        {
            return NotFound();
        }

        [HttpGet("Produto/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Produto()
        {
            return NotFound();
        }


    }
}