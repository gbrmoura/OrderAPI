using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Database;
using OrderAPI.Enums;
using OrderAPI.Models;
using OrderAPI.Services;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Controllers {

    [Route("api/cardapio/")]
    public class CCardapio : ControllerBase {

        private DBContext _context;

        private IMapper _mapper;

        public CCardapio(DBContext context, IMapper mapper) {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpGet("categoria/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<HttpResponse> Categoria() {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            try {

                List<MCategoria> values = _context.Categoria
                    .Where((element) => element.Status == true)
                    .Include((element) => element.Produtos)
                    .ToList();

                if (values.Count <= 0) {
                    response.Code = (int) EHttpResponse.NOT_FOUND;
                    response.Message = "Nenhum registro encontrado.";

                    return StatusCode(response.Code, response);
                }

                List<ConsultarCardapioCategoriaResponse> categorias = new List<ConsultarCardapioCategoriaResponse>();
                values.ForEach((element) => {
                    var categoria = new ConsultarCardapioCategoriaResponse() {
                        Codigo = element.Codigo,
                        Descricao = element.Descricao,
                        Titulo = element.Titulo,
                    };
                    
                    categoria.Produtos = new List<ConsultarProdutoResponse>();
                    element.Produtos.ForEach((e) => {
                        var produto = new ConsultarProdutoResponse()  {
                            Codigo = e.Codigo,
                            Titulo = e.Titulo,
                            Descricao = e.Descricao,
                            Quantidade = e.Quantidade,
                            Valor = e.Valor
                        };
                        categoria.Produtos.Add(produto);
                    });

                    categorias.Add(categoria);
                });

                response.Code = (int) EHttpResponse.OK;
                response.Message = "";
                response.Response = categorias;

                return StatusCode(response.Code, response);
            } catch(Exception E) {
                response.Code = (int) EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor.";
                response.Error  = E.Message;

                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("produtos/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<HttpResponse> Produtos() {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            try {

                List<MProduto> values = _context.Produto
                    .Where((element) => element.Status == true)
                    .Include((element) => element.Categoria)
                    .ToList();

                if (values.Count <= 0) {
                    response.Code = (int) EHttpResponse.NOT_FOUND;
                    response.Message = "Nenhum registro encontrado.";

                    return StatusCode(response.Code, response);
                }

                List<ConsultarCardapioProdutoResponse> produtos = new List<ConsultarCardapioProdutoResponse>();
                values.ForEach((element) => {
                    var produto = new ConsultarCardapioProdutoResponse();
                    produto.Codigo = element.Codigo;
                    produto.Titulo = element.Titulo;
                    produto.Descricao = element.Descricao;
                    produto.Quantidade = element.Quantidade;;
                    produto.Valor = element.Valor;
                    produto.Categoria = _mapper.Map<ConsultarCategoriaResponse>(element.Categoria);
                    produtos.Add(produto);
                });

                response.Code = (int) EHttpResponse.OK;
                response.Message = "";
                response.Response = produtos;

                return StatusCode(response.Code, response);
            } catch(Exception E) {
                response.Code = (int) EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor.";
                response.Error  = E.Message;

                return StatusCode(response.Code, response);
            }
        }
    }
}