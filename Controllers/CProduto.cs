using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Database;
using System;
using System.Linq;
using OrderAPI.Data.Request;
using OrderAPI.Enums;
using OrderAPI.Services;
using OrderAPI.Models;
using System.Collections.Generic;
using OrderAPI.Data.Response;

namespace OrderAPI.Controllers {
    
    [Route("api/produto/")]
    public class CProduto : ControllerBase {
        
        private DBContext _context;

        private IMapper _mapper;
        public CProduto(DBContext context, IMapper mapper) {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpPost("registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<HttpResponse> Registrar([FromBody] CriarProdutoRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((element) => element.Codigo == dados.CategoriaCodigo);

                if (categoria == null) {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                MProduto produto = _context.Produto
                    .FirstOrDefault((element) => element.Titulo.Equals(dados.Titulo) && element.Status == true);

                if (produto != null) {
                    response.Message = "Produto ja cadastrado.";
                    return StatusCode(response.Code, response);
                }

                MProduto produtoDB = _mapper.Map<MProduto>(dados);
                produtoDB.Status = true;
                produtoDB.Categoria = categoria;

                _context.Produto.Add(produtoDB);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.CREATED;
                response.Message = "Produto cadastrado com sucesso.";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("alterar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<HttpResponse> Alterar([FromBody] AlterarProdutoRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                
                MProduto produto = _context.Produto
                    .Include((element) => element.Categoria)
                    .FirstOrDefault((element) => element.Codigo == dados.Codigo && element.Status == true);
                    

                if (produto == null) {
                    response.Code = (int)EHttpResponse.NOT_FOUND;
                    response.Message = "Produto não encontrado.";
                    return StatusCode(response.Code, response);
                }

                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((element) => element.Codigo == dados.CategoriaCodigo);

                if (categoria == null) {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                _mapper.Map(dados, categoria);
                produto.Categoria = categoria;
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Produto alterado com sucesso.";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("deletar/{codigo}")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<HttpResponse> Deletar(int codigo) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada."
            };

            try {
                
                MProduto produto = _context.Produto
                    .FirstOrDefault((element) => element.Codigo == codigo);

                if (produto == null) {
                    response.Message = "Produto não encontrada.";
                    return StatusCode(response.Code, response);
                }

                produto.Status = false;
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Produto deletado com sucesso.";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("consultar/{codigo}")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<HttpResponse> Consultar(int codigo) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada."
            };  

            try {

                MProduto produto = _context.Produto
                    .FirstOrDefault((element) => element.Codigo == codigo);

                if (produto == null) {
                    response.Code = (int)EHttpResponse.NOT_FOUND;
                    response.Message = $"Produto de codigo {codigo} não encontrado.";
                    return StatusCode(response.Code, response);
                }

                ConsultarProdutoResponse produtoResp = _mapper.Map<ConsultarProdutoResponse>(produto);

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Produto encontrado.";
                response.Response = produtoResp;
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor.";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            } 
        }

        [HttpGet("listar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<HttpResponse> Listar() {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada."
            };

            try {
                List<MProduto> produto = _context.Produto
                    .Where((element) => element.Status == true)
                    .ToList();

                if (produto.Count <= 0) {
                    response.Code = (int)EHttpResponse.NOT_FOUND;
                    response.Message = "Nenhum produto encontrado.";
                    return StatusCode(response.Code, response);
                }

                List<ConsultarProdutoResponse> categoriaDB = _mapper.Map<List<ConsultarProdutoResponse>>(produto);

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Produto(s) encontrado(s).";
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