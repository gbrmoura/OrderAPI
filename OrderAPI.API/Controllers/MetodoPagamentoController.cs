using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.API.Services;
using OrderAPI.Data;
using OrderAPI.Data.Models;

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
        
        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarMetodoPagtoRequest body) 
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try 
            {
                MMetodoPagamento MetodoPagamento = _context.MetodoPagamento
                    .FirstOrDefault(pagto => pagto.Nome.Equals(body.Nome));

                if (MetodoPagamento != null) 
                {
                    response.Message = "Método de Pagamento já cadastrado!";
                    return StatusCode(response.Code, response);
                }

                MMetodoPagamento pagtoDB = _mapper.Map<MMetodoPagamento>(body);
                pagtoDB.Status = true;

                _context.Add(pagtoDB);
                _context.SaveChanges();

                response.Code = StatusCodes.Status201Created;
                response.Message = "Método de Pagamento cadastrado com sucesso!";
                return StatusCode(response.Code, response);
            } 
            catch (Exception E) 
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("Alterar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarMetodoPagtoRequest body)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                MMetodoPagamento metodoPagto = _context.MetodoPagamento
                    .FirstOrDefault((pagto) => pagto.Codigo == body.Codigo);

                if (metodoPagto == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Método Pagto não encontrada.";
                    return StatusCode(response.Code, response);
                }

                _mapper.Map(body, metodoPagto);
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Método Pagto alterado com sucesso";
                return StatusCode(response.Code, response);

            } 
            catch (Exception E) 
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("Deletar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Deletar([FromQuery] int codigo)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            try 
            {
                MMetodoPagamento metodoPagto = _context.MetodoPagamento
                    .FirstOrDefault((pagto) => pagto.Codigo == codigo);

                if (metodoPagto == null) 
                {
                    response.Message = "Método de Pagto não encontrado.";
                    return StatusCode(response.Code, response);
                }

                metodoPagto.Status = false;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Método de Pagto deletada com sucesso";
                return StatusCode(response.Code, response);
            } 
            catch (Exception E) 
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("Consultar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Consultar([FromQuery] int codigo)
        {
            DefaultResponse httpMessage = new DefaultResponse() 
            {
                    Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada!"
            };

            try 
            {
                MMetodoPagamento metodoPagto = _context.MetodoPagamento
                    .FirstOrDefault(pagto => pagto.Codigo == codigo);

                if (metodoPagto == null) 
                {
                    httpMessage.Code = StatusCodes.Status404NotFound;
                    httpMessage.Message = $"Método de Pagto. de código { codigo }, não encontrado.";
                    return StatusCode(httpMessage.Code, httpMessage);
                }

                httpMessage.Code = StatusCodes.Status200OK;
                httpMessage.Message = "Método de Pagto. encontrado.";
                httpMessage.Response = _mapper.Map<ConsultarMetodoPagtoResponse>(metodoPagto);
                return StatusCode(httpMessage.Code, httpMessage);
            }
            catch (Exception E) 
            {
                httpMessage.Code = StatusCodes.Status500InternalServerError;
                httpMessage.Message = "Erro interno do servidor.";
                httpMessage.Error = E.Message;
                return StatusCode(httpMessage.Code, httpMessage);
            }
        }

        [HttpGet("Listar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Listar([FromQuery] ListarRequest query) 
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            try 
            {
                List<MMetodoPagamento> metodos = _context.MetodoPagamento
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                if (metodos.Count <= 0) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Nenhum metododo de pagamento encontrado.";
                    return StatusCode(response.Code, response);
                }

                var count = _context.MetodoPagamento
                    .Where(e => e.Status == true)
                    .Count();

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = count,
                    Dados = _mapper.Map<List<ConsultarMetodoPagtoResponse>>(metodos)
                };

                response.Code = StatusCodes.Status200OK;
                response.Message = "Metodo pagamento encontrado(s).";
                response.Response = list;
                return StatusCode(response.Code, response);
            } 
            catch (Exception E) 
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro interno do servidor.";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

    }
}