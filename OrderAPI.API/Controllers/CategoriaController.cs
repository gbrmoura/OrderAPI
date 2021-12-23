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
    public class CategoriaController : ControllerBase
    {
        private OrderAPIContext context;
        private IMapper mapper;
        private ModelService model;

        public CategoriaController(OrderAPIContext context, IMapper mapper, ModelService model)
        {   
            this.context = context;
            this.mapper = mapper;
            this.model = model;
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                if (this.context.Categoria.Any(e => e.Titulo.Equals(body.Titulo) && e.Status == true)) 
                {
                    http.Message = "Categoria ja cadastrada";
                    return StatusCode(http.Code, http);
                }

                CategoriaModel categoria = this.mapper.Map<CategoriaModel>(body);
                this.context.Categoria.Add(categoria);
                this.context.SaveChanges();

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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }   

            try 
            {
                CategoriaModel categoria = this.context.Categoria
                    .Where(e => e.Codigo == body.Codigo)
                    .SingleOrDefault();

                if (categoria == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Categoria não encontrada.";
                    return StatusCode(http.Code, http);
                }

                this.mapper.Map(body, categoria);
                this.context.SaveChanges();

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
                CategoriaModel categoria = this.context.Categoria
                    .Where(e => e.Codigo == codigo)
                    .SingleOrDefault();

                if (categoria == null) 
                {
                    http.Message = "Categoria não encontrada.";
                    return StatusCode(http.Code, http);
                }

                categoria.Status = false;
                this.context.SaveChanges();

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
                CategoriaModel categoria = this.context.Categoria
                    .Where(e => e.Codigo == codigo)
                    .SingleOrDefault();

                if (categoria == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = $"Categoria de codigo { codigo }, não encontrada.";
                    return StatusCode(http.Code, http);
                }
                
                http.Code = StatusCodes.Status200OK;
                http.Message = "Categoria encontrada.";
                http.Response = this.mapper.Map<ConsultarCategoriaResponse>(categoria);;
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                IQueryable<CategoriaModel> sql = this.context.Categoria;
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
                    NumeroRegistros = this.context.Categoria.Where(e => e.Status == true).Count(),
                    Dados = this.mapper.Map<List<ConsultarCategoriaResponse>>(categorias)
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