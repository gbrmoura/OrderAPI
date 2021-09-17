using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Database;
using System;
using System.Linq;
using OrderAPI.Data.Request;
using OrderAPI.Enums;
using OrderAPI.Services;
using OrderAPI.Models;

namespace OrderAPI.Controllers {
    
    [Route("api/produto/")]
    public class CProduto : ControllerBase {
        
        private DBContext _context;

        private IMapper _mapper;
        public CProduto(DBContext context, IMapper mapper) {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpPost("registrar")]
        [Authorize(Roles = "MASTER, GERENTE, PADRAO")]
        public ActionResult<HttpResponse> Registrar([FromBody] CriarProdutoRequest dados) {
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
                    .FirstOrDefault((element) => element.Codigo == dados.CategoriaCodigo);

                if (categoria == null) {
                    response.Message = "Categoria não encontrada";
                    return StatusCode(response.Code, response);
                }

                MProduto produto = _context.Produto
                    .FirstOrDefault((element) => element.Titulo.Equals(dados.Titulo) && element.Status == true);

                if (produto != null) {
                    response.Message = "Produto ja cadastrado";
                    return StatusCode(response.Code, response);
                }

                MProduto produtoDB = _mapper.Map<MProduto>(dados);
                produtoDB.Status = true;
                produtoDB.Categoria = categoria;

                _context.Produto.Add(produtoDB);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Produto cadastrado com sucesso";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

    }
}