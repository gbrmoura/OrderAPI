using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Data;
using AutoMapper;
using OrderAPI.API.Services;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.Data.Models;
using OrderAPI.Data.Helpers;
using OrderAPI.API.Helpers;
using OrderAPI.API.HTTP.Response;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class CardapioController : ControllerBase
    {
        private OrderAPIContext _context;

        private IMapper _mapper;

        public CardapioController(OrderAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet("Completo/")]
    	[Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Cardapio() 
        {
            return NotFound();
        }

        [HttpGet("Categorias/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Categorias ([FromQuery] ListarRequest query)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            try 
            {
                List<MCategoria> categorias = _context.Categoria            
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                if (categorias.Count <= 0) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = $"Categoria(s) não encontrado(s).";
                    return StatusCode(response.Code, response);
                }

                List<ConsultarCategoriaResponse> produtoResponse = _mapper.Map<List<ConsultarCategoriaResponse>>(categorias);
                
                response.Code = StatusCodes.Status200OK;
                response.Message = "Categoria encontrada(s)!";
                response.Response = produtoResponse;
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

        [HttpGet("Categoria/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Categoria([FromQuery] int codigo)
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

                List<MProduto> produtos = _context.Produto                   
                    .Where((p) => p.Status == true)
                    .ToList();

                ConsultarCardapioCategoriaResponse categoriaResponse = _mapper.Map<ConsultarCardapioCategoriaResponse>(categoria);
                categoriaResponse.Produtos = _mapper.Map<List<ConsultarProdutoResponse>>(produtos);
                
                response.Code = StatusCodes.Status200OK;
                response.Message = "Categoria encontrada(s)!";
                response.Response = categoriaResponse;
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

        [HttpGet("Produtos/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Produtos ([FromQuery] ListarRequest query)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            try 
            {
                List<MProduto> produtos = _context.Produto            
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                if (produtos.Count <= 0) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = $"Produtos não encontrados.";
                    return StatusCode(response.Code, response);
                }

                List<ConsultarProdutoResponse> produtoResponse = _mapper.Map<List<ConsultarProdutoResponse>>(produtos);
                
                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto encontrado(s)!";
                response.Response = produtoResponse;
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

        [HttpGet("Produto/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Produto([FromQuery] int codigo)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            try 
            {
                MProduto produto = _context.Produto            
                    .FirstOrDefault((e) => e.Codigo == codigo);

                if (produto == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = $"Produto de codigo { codigo }, não encontrada.";
                    return StatusCode(response.Code, response);
                }

                MCategoria categoria = _context.Categoria
                    .FirstOrDefault((e) => e.Codigo == codigo);

                ConsultarCardapioProdutoResponse produtoResponse = _mapper.Map<ConsultarCardapioProdutoResponse>(produto);
                produtoResponse.Categoria = _mapper.Map<ConsultarCategoriaResponse>(categoria);
                
                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto encontrado(s)!";
                response.Response = produtoResponse;
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