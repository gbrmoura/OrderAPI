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
        
        private OrderAPIContext _context;

        private IMapper _mapper;

        public CategoriaController(OrderAPIContext context, IMapper mapper)
        {   
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarCategoriaRequest body) 
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try 
            {
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Titulo.Equals(body.Titulo) && categoria.Status == true);

                if (categoria != null) 
                {
                    response.Message = "Categoria ja cadastrada";
                    return StatusCode(response.Code, response);
                }

                MCategoria categoriaDB = _mapper.Map<MCategoria>(body);
                _context.Categoria.Add(categoriaDB);
                _context.SaveChanges();

                response.Code = StatusCodes.Status201Created;
                response.Message = "Categoria cadastrada com sucesso";
                response.Response = _mapper.Map<ConsultarCategoriaResponse>(categoriaDB);

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
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarCategoriaRequest body)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }   

            try 
            {
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Codigo == body.Codigo);

                if (categoria == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                _mapper.Map(body, categoria);
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Categoria alterada com sucesso";
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
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Codigo == codigo);

                if (categoria == null) 
                {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                categoria.Status = false;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Categoria deletada com sucesso";
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
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            try 
            {

                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Codigo == codigo);

                if (categoria == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = $"Categoria de codigo { codigo }, não encontrada.";
                    return StatusCode(response.Code, response);
                }

                ConsultarCategoriaResponse categoriaDB = _mapper.Map<ConsultarCategoriaResponse>(categoria);

                response.Code = StatusCodes.Status200OK;
                response.Message = "Categoria encontrada.";
                response.Response = categoriaDB;
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
                IQueryable<MCategoria> sql = _context.Categoria;

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

                if (categorias.Count <= 0) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Nenhuma categoria encontrada.";
                    return StatusCode(response.Code, response);
                }

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = _context.Categoria.Where(e => e.Status == true).Count(),
                    Dados = _mapper.Map<List<ConsultarCategoriaResponse>>(categorias)
                };

                response.Code = StatusCodes.Status200OK;
                response.Message = "Categoria encontrada(s).";
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