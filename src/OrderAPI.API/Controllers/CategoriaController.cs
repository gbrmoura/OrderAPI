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
    public class CategoriaController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;

        public CategoriaController(OrderAPIContext context, IMapper mapper, ModelService model)
        {   
            _context = context;
            _mapper = mapper;
            _model = model;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarCategoriaRequest body) 
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
                if (_context.Categoria.Any(e => e.Titulo.Equals(body.Titulo) && e.Status == true)) 
                {
                    http.Message = "Categoria ja cadastrada";
                    return StatusCode(http.Code, http);
                }

                CategoriaModel categoria = _mapper.Map<CategoriaModel>(body);
                _context.Categoria.Add(categoria);
                _context.SaveChanges();

                http.Code = StatusCodes.Status201Created;
                http.Message = "Categoria cadastrada com sucesso";
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
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarCategoriaRequest body)
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
                CategoriaModel categoria = _context.Categoria
                    .Where(e => e.Codigo == body.Codigo && e.Status == true)
                    .SingleOrDefault();

                if (categoria == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Categoria não encontrada.";
                    return StatusCode(http.Code, http);
                }

                _mapper.Map(body, categoria);
                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Categoria alterada com sucesso";
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
                    new ErrorResponse() { Field = "Codigo", Message = "Código deve ser maior que zero" }
                };
                return StatusCode(http.Code, http);
            }

            try 
            {
                CategoriaModel categoria = _context.Categoria
                    .Where(e => e.Codigo == codigo && e.Status == true)
                    .SingleOrDefault();

                if (categoria == null) 
                {
                    http.Message = "Categoria não encontrada.";
                    return StatusCode(http.Code, http);
                }

                categoria.Status = false;
                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Categoria deletada com sucesso";
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
                Message = "Rota não autorizada"
            };

            if (codigo <= 0) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = new List<ErrorResponse>()
                {
                    new ErrorResponse() { Field = "Codigo", Message = "Código deve ser maior que zero" }
                };
                return StatusCode(http.Code, http);
            }

            try 
            {
                CategoriaModel categoria = _context.Categoria
                    .Where(e => e.Codigo == codigo && e.Status == true)
                    .SingleOrDefault();

                if (categoria == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = $"Categoria de codigo { codigo }, não encontrada.";
                    return StatusCode(http.Code, http);
                }
                
                http.Code = StatusCodes.Status200OK;
                http.Message = "Categoria encontrada.";
                http.Response = _mapper.Map<ConsultarCategoriaResponse>(categoria);;
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
                IQueryable<CategoriaModel> sql = _context.Categoria;
                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) => 
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Titulo.Contains(query.CampoPesquisa) || 
                        e.Descricao.Contains(query.CampoPesquisa));
                }

                var categorias = sql
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = _context.Categoria.Where(e => e.Status == true).Count(),
                    Dados = _mapper.Map<List<ConsultarCategoriaResponse>>(categorias)
                };

                http.Code = StatusCodes.Status200OK;
                http.Message = "Categoria encontrada(s).";
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