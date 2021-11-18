using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Data;
using AutoMapper;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.Data.Models;
using OrderAPI.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using OrderAPI.API.Services;
using OrderAPI.API.EntensionMethods;
using OrderAPI.API.HTTP.Response;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class CardapioController : ControllerBase
    {
        private OrderAPIContext context;
        private IMapper mapper;
        private ModelService model;

        public CardapioController(OrderAPIContext context, IMapper mapper, ModelService model)
        {
            this.context = context;
            this.mapper = mapper;
            this.model = model;
        }
        
        [HttpGet]
    	[Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Cardapio([FromQuery] ListarCardapioRequest query) 
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid)
            {
                http.Message = "Parametros Ausentes";
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                IQueryable<MProduto> sql = this.context.Produto;
                int codigo = 0;
                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    codigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                    if (!this.context.Usuario.Any((e) => e.Codigo == codigo && e.Status == true)) 
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Usuario não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    sql = sql
                        .Include(e => e.Favoritos)
                        .Where((e) => e.Favoritos.Any((fav) => fav.UsuarioCodigo == codigo && fav.Status == true));
                }
                
                List<MProduto> produtos = sql
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
                    Favorito = e.Favoritos.Any((fav) => fav.UsuarioCodigo == codigo && fav.ProdutoCodigo == e.Codigo &&  fav.Status == true),
                    Categoria = new ConsultarCategoriaResponse() {
                        Codigo = e.Categoria.Codigo,
                        Titulo = e.Categoria.Titulo,
                        Descricao = e.Categoria.Descricao
                    }
                });

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = this.context.Produto.Where(e => e.Status == true).Count(),
                    Dados = dados
                };

                http.Code = StatusCodes.Status200OK;
                http.Message = "Categoria encontrada(s)!";
                http.Response = list;
                return StatusCode(http.Code, http);
            }   
            catch (Exception E)   
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor.";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpPost("Favorito/")]
        [Authorize(Roles = "USUARIO")]
        public ActionResult<DefaultResponse> Favorito([FromBody] FavoritoRequest body) 
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                var codigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                var usuario = this.context.Usuario.SingleOrDefault((e) => e.Codigo == codigo && e.Status == true);
                if (usuario == null)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Usuario não encontrado.";
                    return StatusCode(http.Code, http);
                }
                
                if (body.Estado)
                {
                    var produto = this.context.Produto.SingleOrDefault((e) => e.Codigo == body.ProdutoCodigo && e.Status == true);
                    if (produto == null) 
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Produto não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    MFavorito favorito = new MFavorito() 
                    {
                        Produto = produto,
                        Usuario = usuario
                    };

                    this.context.Favorito.Add(favorito);
                    this.context.SaveChanges();

                    http.Code = StatusCodes.Status200OK;
                    http.Message = "Produto favoritado com sucesso.";
                    return StatusCode(http.Code, http);
                }
                else
                {
                    var favorito = this.context.Favorito
                        .Include((e) => e.Produto)
                            .Where((e) => e.Produto.Codigo == body.ProdutoCodigo)
                        .Include((e) => e.Usuario)
                            .Where((e) => e.Usuario.Codigo == codigo && e.Status == true)
                        .SingleOrDefault();

                    if (favorito == null) 
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Favorito não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    favorito.Status = false;
                    this.context.SaveChanges();

                    http.Code = StatusCodes.Status200OK;
                    http.Message = "Produto favoritado removido com sucesso.";
                    return StatusCode(http.Code, http);
                }
            }
            catch (Exception E)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor.";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

    }
}