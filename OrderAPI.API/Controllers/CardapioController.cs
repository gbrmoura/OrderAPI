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
using Microsoft.EntityFrameworkCore;

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
        
        [HttpGet]
    	[Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Cardapio([FromQuery] ListarCardapioRequest query) 
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
                var usuarioCodigo = 0;
                IQueryable<MCategoria> sql = _context.Categoria;
                if (IdentityService.getRole(User.Claims) == PrevilegioEnum.USUARIO.ToString() && Int32.TryParse(query.UsuarioCodigo, out usuarioCodigo))
                {
                    if (IdentityService.getRole(User.Claims) == PrevilegioEnum.USUARIO.ToString() && !_context.Usuario.Any((e) => e.Codigo == usuarioCodigo && e.Status == true)) 
                    {
                        response.Code = StatusCodes.Status401Unauthorized;
                        response.Message = "Usuario não encontrado.";
                        return StatusCode(response.Code, response);
                    }
                }
                
                List<MCategoria> categorias = _context.Categoria
                    .Include((e) => e.Produtos)
                        .ThenInclude((e) => e.Favoritos)       
                    .Where((e) => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                var list = categorias.Select((e) => new ConsultarCardapioCategoriaResponse() 
                {
                    Codigo = e.Codigo,
                    Titulo = e.Titulo,
                    Descricao = e.Descricao,
                    Produtos = e.Produtos.Where((p) => p.Status == true).Select((j) => new ConsultarProdutoSimplesResponse() 
                    {
                        Codigo = j.Codigo,
                        Titulo = j.Titulo,
                        Descricao = j.Descricao,
                        Valor = j.Valor,
                        Quantidade = j.Quantidade,
                        Favorito = j.Favoritos.Any((h) => 
                            h.UsuarioCodigo == usuarioCodigo && 
                            h.ProdutoCodigo == j.Codigo && 
                            h.Status == true)
                    }).ToList()
                });

                response.Code = StatusCodes.Status200OK;
                response.Message = "Categoria encontrada(s)!";
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

        [HttpPost("RegistrarFavorito/")]
        [Authorize(Roles = "USUARIO")]
        public ActionResult<DefaultResponse> RegistrarFavorito([FromBody] RegistrarFavoritoRequest body)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                var usuario = _context.Usuario.SingleOrDefault((e) => e.Codigo == body.UsuarioCodigo && e.Status == true);
                if (usuario == null)
                {
                    response.Code = StatusCodes.Status401Unauthorized;
                    response.Message = "Usuario não encontrado.";
                    return StatusCode(response.Code, response);
                }

                var produto = _context.Produto.SingleOrDefault((e) => e.Codigo == body.ProdutoCodigo && e.Status == true);
                if (produto == null) 
                {
                    response.Code = StatusCodes.Status401Unauthorized;
                    response.Message = "Produto não encontrado.";
                    return StatusCode(response.Code, response);
                }

                MFavorito favorito = new MFavorito() 
                {
                    Produto = produto,
                    Usuario = usuario
                };

                _context.Favorito.Add(favorito);
                _context.SaveChanges();
                
                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto favoritado com sucesso.";
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

        [HttpGet("RemoverFavorito/")]
        [Authorize(Roles = "USUARIO")]
        public ActionResult<DefaultResponse> RemoverFavorito([FromQuery] RemoverFavoritoRequest query) 
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };
            
            if (!ModelState.IsValid) 
            {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                var usuario = _context.Usuario.SingleOrDefault((e) => e.Codigo == query.UsuarioCodigo && e.Status == true);
                if (usuario == null)
                {
                    response.Code = StatusCodes.Status401Unauthorized;
                    response.Message = "Usuario não encontrado.";
                    return StatusCode(response.Code, response);
                }

                var favorito = _context.Favorito
                    .Include((e) => e.Produto)
                    .Include((e) => e.Usuario)
                    .Where((e) => e.Produto.Codigo == query.ProdutoCodigo && e.Usuario.Codigo == query.UsuarioCodigo)
                    .SingleOrDefault();
                
                if (favorito == null) 
                {
                    response.Code = StatusCodes.Status401Unauthorized;
                    response.Message = "Favorito não encontrado.";
                    return StatusCode(response.Code, response);
                }

                favorito.Status = false;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto favoritado removido com sucesso.";
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