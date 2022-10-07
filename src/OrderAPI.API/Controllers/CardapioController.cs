using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Data;
using AutoMapper;
using OrderAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using OrderAPI.API.Services;
using OrderAPI.API.EntensionMethods;
using OrderAPI.Domain.Http;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Enums;
using OrderAPI.Domain.Http.Response;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class CardapioController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;

        public CardapioController(OrderAPIContext context, IMapper mapper, ModelService model)
        {
            _context = context;
            _mapper = mapper;
            _model = model;
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
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                var codigo = 0;
                IQueryable<ProdutoModel> sql = _context.Produto;
                IQueryable<ProdutoModel> sqlCount = _context.Produto;

                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    codigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                    if (!_context.Usuario.Any((e) => e.Codigo == codigo && e.Status == true))
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Usuario não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    if (query.Favorito)
                    {
                        sql = sql
                            .Include(e => e.Favoritos)
                            .Where((e) => e.Favoritos.Any((f) => f.UsuarioCodigo == codigo && f.Status == true));

                        sqlCount = sqlCount
                            .Include(e => e.Favoritos)
                            .Where((e) => e.Favoritos.Any((f) => f.UsuarioCodigo == codigo && f.Status == true));
                    }
                }

                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql
                        .Include(e => e.Categoria)
                        .Where((e) => e.Titulo.Contains(query.CampoPesquisa) ||
                            e.Descricao.Contains(query.CampoPesquisa) ||
                            e.Valor.ToString().Contains(query.CampoPesquisa) ||
                            e.Categoria.Titulo.Contains(query.CampoPesquisa) ||
                            e.Quantidade.ToString().Contains(query.CampoPesquisa));

                    sqlCount = sqlCount
                        .Include(e => e.Categoria)
                        .Where((e) => e.Titulo.Contains(query.CampoPesquisa) ||
                            e.Descricao.Contains(query.CampoPesquisa) ||
                            e.Valor.ToString().Contains(query.CampoPesquisa) ||
                            e.Categoria.Titulo.Contains(query.CampoPesquisa) ||
                            e.Quantidade.ToString().Contains(query.CampoPesquisa));
                }

                List<ProdutoModel> produtos = sql
                    .Include((e) => e.Categoria)
                    .Include((e) => e.Favoritos)
                    .Where((e) => e.Status == true && e.Quantidade > 0)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                List<ProdutoModel> count = sqlCount
                    .Where((e) => e.Status == true && e.Quantidade > 0)
                    .ToList();

                var dados = produtos.Select(e => new ConsultarCardapioProdutoResponse()
                {
                    Codigo = e.Codigo,
                    Titulo = e.Titulo,
                    Descricao = e.Descricao,
                    Valor = e.Valor,
                    Quantidade = e.Quantidade,
                    Favorito = e.Favoritos.Any((fav) => fav.UsuarioCodigo == codigo && fav.ProdutoCodigo == e.Codigo && fav.Status == true),
                    Categoria = new ConsultarCategoriaResponse()
                    {
                        Codigo = e.Categoria.Codigo,
                        Titulo = e.Categoria.Titulo,
                        Descricao = e.Categoria.Descricao
                    }
                });

                ListarResponse list = new ListarResponse
                {
                    NumeroRegistros = count.Count,
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
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                var codigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                var usuario = _context.Usuario.SingleOrDefault((e) => e.Codigo == codigo && e.Status == true);
                if (usuario == null)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Usuario não encontrado.";
                    return StatusCode(http.Code, http);
                }

                if (body.Estado)
                {
                    var produto = _context.Produto.SingleOrDefault((e) => e.Codigo == body.ProdutoCodigo && e.Status == true);
                    if (produto == null)
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Produto não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    FavoritoModel favorito = new FavoritoModel()
                    {
                        Produto = produto,
                        Usuario = usuario
                    };

                    _context.Favorito.Add(favorito);
                    _context.SaveChanges();

                    http.Code = StatusCodes.Status200OK;
                    http.Message = "Produto favoritado com sucesso.";
                    return StatusCode(http.Code, http);
                }
                else
                {
                    var favorito = _context.Favorito
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
                    _context.SaveChanges();

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