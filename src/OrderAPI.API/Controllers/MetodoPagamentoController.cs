using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Domain.Http;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.API.Services;
using OrderAPI.Data;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class MetodoPagamentoController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;

        public MetodoPagamentoController(OrderAPIContext context, IMapper mapper, ModelService model)
        {   
            _context = context;
            _mapper = mapper;
            _model = model;
        }
        
        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarMetodoPagtoRequest body) 
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                if (_context.MetodoPagamento.Any((e) => e.Titulo.Equals(body.Titulo) && e.Status == true)) 
                {
                    http.Message = "Método de Pagamento já cadastrado!";
                    return StatusCode(http.Code, http);
                }

                MetodoPagamentoModel pagto = _mapper.Map<MetodoPagamentoModel>(body);
                _context.Add(pagto);
                _context.SaveChanges();

                http.Code = StatusCodes.Status201Created;
                http.Message = "Método de Pagamento cadastrado com sucesso!";
                return StatusCode(http.Code, http);
            } 
            catch (Exception E) 
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor!";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpPost("Alterar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarMetodoPagtoRequest body)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                MetodoPagamentoModel pagto = _context.MetodoPagamento
                    .Where((e) => e.Codigo == body.Codigo && e.Status == true)
                    .SingleOrDefault();

                if (pagto == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Método Pagto não encontrada.";
                    return StatusCode(http.Code, http);
                }

                _mapper.Map(body, pagto);
                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Método Pagto alterado com sucesso";
                return StatusCode(http.Code, http);
            } 
            catch (Exception E) 
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor!";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpGet("Deletar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Deletar([FromQuery] int codigo)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            if (codigo <= 0) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = new List<ErrorResponse>()
                {
                    new ErrorResponse() { Field = "Codigo", Message = "Codigo deve ser maior que zero" }
                };
                return StatusCode(http.Code, http);
            }

            try 
            {
                MetodoPagamentoModel pagto = _context.MetodoPagamento
                    .Where((e) => e.Codigo == codigo && e.Status == true)
                    .SingleOrDefault();

                if (pagto == null) 
                {
                    http.Message = "Método de Pagto não encontrado.";
                    return StatusCode(http.Code, http);
                }

                pagto.Status = false;
                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Método de Pagto deletada com sucesso";
                return StatusCode(http.Code, http);
            } 
            catch (Exception E) 
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor!";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpGet("Consultar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Consultar([FromQuery] int codigo)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada!"
            };

            if (codigo <= 0) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = new List<ErrorResponse>()
                {
                    new ErrorResponse() { Field = "Codigo", Message = "Codigo deve ser maior que zero" }
                };
                return StatusCode(http.Code, http);
            }

            try 
            {
                MetodoPagamentoModel pagto = _context.MetodoPagamento
                    .Where((e) => e.Codigo == codigo && e.Status == true)
                    .SingleOrDefault();

                if (pagto == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = $"Método de Pagto. de código { codigo }, não encontrado.";
                    return StatusCode(http.Code, http);
                }

                http.Code = StatusCodes.Status200OK;
                http.Message = "Método de Pagto. encontrado.";
                http.Response = _mapper.Map<ConsultarMetodoPagtoResponse>(pagto);
                return StatusCode(http.Code, http);
            }
            catch (Exception E) 
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor.";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpGet("Listar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Listar([FromQuery] ListarRequest query) 
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                IQueryable<MetodoPagamentoModel> sql = _context.MetodoPagamento;
                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) =>
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Titulo.Contains(query.CampoPesquisa)
                    );
                }

                var result = sql.Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = _context.MetodoPagamento.Where(e => e.Status == true).Count(),
                    Dados = _mapper.Map<List<ConsultarMetodoPagtoResponse>>(result)
                };

                http.Code = StatusCodes.Status200OK;
                http.Message = "Metodo pagamento encontrado(s).";
                http.Response = list;
                return StatusCode(http.Code, http);
            } 
            catch (Exception E) 
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor.";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }
    }
}