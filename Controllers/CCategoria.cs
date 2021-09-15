using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
using OrderAPI.Database;
using OrderAPI.Enums;
using OrderAPI.Models;
using OrderAPI.Services;

namespace orderapi.Controllers
{   
    [Route("api/categoria/")]
    public class CCategoria : ControllerBase
    {

        private DBContext _context;

        private IMapper _mapper;

        public CCategoria(DBContext context, IMapper mapper) {
            this._context = context;
            this._mapper = mapper;
        }


        [HttpPost("registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, PADRAO")]
        public ActionResult<HttpResponse> Registrar([FromBody] CriarCategoriaRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Titulo.Equals(dados.Titulo) && categoria.Status == true);

                if (categoria != null) {
                    response.Message = "Categoria ja cadastrada";
                    return StatusCode(response.Code, response);
                }

                MCategoria categoriaDB = _mapper.Map<MCategoria>(dados);
                categoriaDB.Status = true;

                _context.Categoria.Add(categoriaDB);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Categoria cadastrada com sucesso";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("alterar/")]
        [Authorize(Roles = "MASTER, GERENTE, PADRAO")]
        public ActionResult<HttpResponse> Alterar([FromBody] AlterarCategoriaRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }   

            try {
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Codigo == dados.Codigo);

                if (categoria == null) {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                categoria = _mapper.Map<MCategoria>(dados);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Categoria alterada com sucesso";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("deletar/{codigo}")]
        [Authorize(Roles = "MASTER, GERENTE, PADRAO")]
        public ActionResult<HttpResponse> Deletar(int codigo) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            try {
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Codigo == codigo);

                if (categoria == null) {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                categoria.Status = false;
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Categoria deletada com sucesso";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("consultar/{codigo}")]
        [Authorize(Roles = "MASTER, GERENTE, PADRAO")]
        public ActionResult<HttpResponse> Consultar(int codigo) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            try {

                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((categoria) => categoria.Codigo == codigo);

                if (categoria == null) {
                    response.Code = (int)EHttpResponse.NOT_FOUND;
                    response.Message = $"Categoria de codigo {codigo}, não encontrada.";
                    return StatusCode(response.Code, response);
                }

                ConsultarCategoriaResponse categoriaDB = _mapper.Map<ConsultarCategoriaResponse>(categoria);

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Categoria encontrada.";
                response.Response = categoriaDB;
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor.";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        } 

        [HttpGet("todos/")]
        [Authorize(Roles = "MASTER, GERENTE, PADRAO")]
        public ActionResult<HttpResponse> Todos() {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            try {
                List<MCategoria> categoria = _context.Categoria
                    .Where((categoria) => categoria.Status == true)
                    .ToList();

                if (categoria.Count <= 0) {
                    response.Code = (int)EHttpResponse.NOT_FOUND;
                    response.Message = "Nenhuma categoria encontrada";
                    return StatusCode(response.Code, response);
                }

                List<ConsultarCategoriaResponse> categoriaDB = _mapper.Map<List<ConsultarCategoriaResponse>>(categoria);

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Categoria encontrado.";
                response.Response = categoriaDB;
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor.";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }
        
    }
}