using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class ProdutoController : ControllerBase
    {
        
        private OrderAPIContext _context;

        private IMapper _mapper;

        public ProdutoController(OrderAPIContext context, IMapper mapper)
        {   
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarProdutoRequest body) 
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                
                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((element) => element.Codigo == body.CategoriaCodigo);

                if (categoria == null) 
                {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                MProduto produto = _context.Produto
                    .FirstOrDefault((element) => element.Titulo.Equals(body.Titulo) && element.Status == true);

                if (produto != null) 
                {
                    response.Message = "Produto ja cadastrado.";
                    return StatusCode(response.Code, response);
                }

                MProduto produtoDB = _mapper.Map<MProduto>(body);
                produtoDB.Status = true;
                produtoDB.Categoria = categoria;

                _context.Produto.Add(produtoDB);
                _context.SaveChanges();

                response.Code = StatusCodes.Status201Created;
                response.Message = "Produto cadastrado com sucesso.";
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
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarProdutoRequest body)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try 
            {
                
                MProduto produto = _context.Produto
                    .Include((element) => element.Categoria)
                    .FirstOrDefault((element) => element.Codigo == body.Codigo && element.Status == true);
                    

                if (produto == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Produto não encontrado.";
                    return StatusCode(response.Code, response);
                }

                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((element) => element.Codigo == body.CategoriaCodigo);

                if (categoria == null) 
                {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                _mapper.Map(body, categoria);
                produto.Categoria = categoria;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto alterado com sucesso.";
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
                Message = "Rota não autorizada."
            };

            try 
            {
                MProduto produto = _context.Produto
                    .FirstOrDefault((element) => element.Codigo == codigo);

                if (produto == null) 
                {
                    response.Message = "Produto não encontrada.";
                    return StatusCode(response.Code, response);
                }

                produto.Status = false;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto deletado com sucesso.";
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
                Message = "Rota não autorizada."
            };  

            try 
            {

                MProduto produto = _context.Produto
                    .FirstOrDefault((element) => element.Codigo == codigo);

                if (produto == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = $"Produto de codigo {codigo} não encontrado.";
                    return StatusCode(response.Code, response);
                }

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto encontrado.";
                response.Response = _mapper.Map<ConsultarProdutoResponse>(produto);
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
                List<MProduto> produtos = _context.Produto
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                if (produtos.Count <= 0) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Nenhum produto encontrado.";
                    return StatusCode(response.Code, response);
                }

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produtos encontrado(s).";
                response.Response = _mapper.Map<List<ConsultarProdutoResponse>>(produtos);
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