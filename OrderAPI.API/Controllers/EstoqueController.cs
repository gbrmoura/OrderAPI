using System;
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

namespace OrderAPI.API.Controllers
{
    [Route("api/Produto/")]
    public class EstoqueController : ControllerBase
    {

        private OrderAPIContext _context;

        private IMapper _mapper;

        public EstoqueController(OrderAPIContext context, IMapper mapper, TokenService jwtService)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] RegistrarEstoqueRequest body)
        {
            DefaultResponse response = new DefaultResponse()
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                var produto = _context.Produto.SingleOrDefault(x => x.Codigo == body.ProdutoCodigo);

                if (produto == null) 
                {
                    response.Message = "Produto não encontrado.";
                    return StatusCode(response.Code, response);
                }
                
                produto.Quantidade += body.Quantidade;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Estoque registrado com sucesso.";
                return StatusCode(response.Code, response);
            }
            catch (Exception err)
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro ao registrar produto estoque.";
                response.Error = err.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("Retirar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Retirar([FromBody] RegistrarEstoqueRequest body)
        {
            DefaultResponse response = new DefaultResponse()
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                var produto = _context.Produto.SingleOrDefault(x => x.Codigo == body.ProdutoCodigo);

                if (produto == null) 
                {
                    response.Message = "Produto não encontrado.";
                    return StatusCode(response.Code, response);
                }
                
                produto.Quantidade -= body.Quantidade;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Estoque registrado com sucesso.";
                return StatusCode(response.Code, response);
            }
            catch (Exception err)
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro ao registrar produto estoque.";
                response.Error = err.Message;
                return StatusCode(response.Code, response);
            }
        }

        // TODO: 
        // TODO: listar
    }
}