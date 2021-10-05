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

        [HttpGet("completo/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<HttpResponse> CardapioCompleto() {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            try {


                List<MCategoria> categorias = _context.Categoria
                    .Where((element) => element.Status == true)
                    .Include((element) => element.Produtos)
                    .ToList();

                

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
                    .Where((element) => element.Status == true && element.Valor > 0)
                    .ToList();

                List<ConsultarCardapioProdutoResponse> produto = _mapper.Map<List<ConsultarCardapioProdutoResponse>>(values);

                response.Code = (int) EHttpResponse.OK;
                response.Message = "";
                response.Response = produto;

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