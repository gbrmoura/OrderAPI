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
                if (IdentityService.getRole(User.Claims) == PrevilegioEnum.USUARIO.ToString())
                {
                    
                    if (String.IsNullOrEmpty(query.UsuarioCodigo) || !Int32.TryParse(query.UsuarioCodigo, out usuarioCodigo))
                    {
                        response.Message = "Parametros ausentes.";
                        response.Error = new List<ErrorResponse>() {
                            new ErrorResponse() { Field = "UsuarioCodigo", Message = "Codigo de usuario deve ser informado." }
                        };
                        return StatusCode(response.Code, response);
                    }

                    if (!_context.Usuario.Any((e) => e.Codigo == usuarioCodigo && e.Status == true)) 
                    {
                        response.Code = StatusCodes.Status401Unauthorized;
                        response.Message = "Usuario não encontrado.";
                        return StatusCode(response.Code, response);
                    }
                }
                
                List<MProduto> produtos = _context.Produto
                    .Include((e) => e.Categoria)
                    .Include((e) => e.Favoritos)
                    .Where((e) => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                var dados = produtos.Select(e => new ConsultarCardapioProdutoResponse() {
                    Codigo = e.Codigo,
                    Titulo = e.Titulo,
                    Descricao = e.Descricao,
                    Valor = e.Valor,
                    Quantidade = e.Quantidade,
                    Favorito = e.Favoritos.Any((fav) => fav.UsuarioCodigo == usuarioCodigo && fav.ProdutoCodigo == e.Codigo &&  fav.Status == true),
                    Categoria = new ConsultarCategoriaResponse() {
                        Codigo = e.Categoria.Codigo,
                        Titulo = e.Categoria.Titulo,
                        Descricao = e.Categoria.Descricao
                    }
                });
                
                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = _context.Produto.Where(e => e.Status == true).Count(),
                    Dados = dados
                };

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

        [HttpPost("Favorito/")]
        [Authorize(Roles = "USUARIO")]
        public ActionResult<DefaultResponse> Favorito([FromBody] FavoritoRequest body) 
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
                var usuario = _context.Usuario.SingleOrDefault((e) => e.Codigo == body.UsuarioCodigo && e.Status == true);
                if (usuario == null)
                {
                    response.Code = StatusCodes.Status401Unauthorized;
                    response.Message = "Usuario não encontrado.";
                    return StatusCode(response.Code, response);
                }
                
                if (body.Estado)
                {
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
                else
                {
                    var favorito = _context.Favorito
                        .Include((e) => e.Produto)
                        .Where((e) => e.Produto.Codigo == body.ProdutoCodigo)
                        .Include((e) => e.Usuario)
                        .Where((e) => e.Usuario.Codigo == body.UsuarioCodigo && e.Status == true)
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