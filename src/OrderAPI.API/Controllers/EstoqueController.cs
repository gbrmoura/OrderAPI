using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Data;
using AutoMapper;
using OrderAPI.API.Services;
using OrderAPI.Domain.Http;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.Domain.Models;
using OrderAPI.Domain.Enums;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OrderAPI.API.EntensionMethods;

namespace OrderAPI.API.Controllers
{
    [Route("api/Estoque/")]
    public class EstoqueController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;
        private UtilsService _utils;

        public EstoqueController(OrderAPIContext context, IMapper mapper, ModelService model, UtilsService utils)
        {
            _context = context;
            _mapper = mapper;
            _model = model;
            _utils = utils;
        }

        [HttpPost("Controle/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Controle([FromBody] EstoqueRequest body)
        {
            DefaultResponse http = new DefaultResponse()
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes.";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            if (!_utils.CompareStrings(body.Tipo, new string[]{ "ENTRADA", "SAIDA" }))
            {
                http.Message = "Tipo de operação inválida. ";
                http.Error = new List<ErrorResponse>() {
                    new ErrorResponse() { Field = "Tipo", Message = "Tipo deve estrar entre 'ENTRADA' e 'SAIDA'" }
                };
                return StatusCode(http.Code, http);
            }

            try
            {
                var funcionarioCodigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                var funcionario = _context.Funcionario
                    .Where(x => x.Codigo == funcionarioCodigo && x.Status == true)
                    .SingleOrDefault();
                    
                if (funcionario == null)
                {
                    http.Message = "Funcionario não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var produto = _context.Produto
                    .Where(x => x.Codigo == body.ProdutoCodigo && x.Status == true)
                    .SingleOrDefault();

                if (produto == null) 
                {
                    http.Message = "Produto não encontrado.";
                    return StatusCode(http.Code, http);
                }

                ControleEstoqueModel controle = new ControleEstoqueModel()
                {
                    Produto = produto,
                    Quantidade = body.Quantidade,
                    Funcionario = funcionario,
                    Observacao = body.Observacao,
                    Data = DateTime.Now,
                    Tipo = body.Tipo == EstoqueCrontoleEnum.ENTRADA.ToString() ? EstoqueCrontoleEnum.ENTRADA : EstoqueCrontoleEnum.SAIDA
                };

                switch(controle.Tipo)
                {
                    case EstoqueCrontoleEnum.ENTRADA:
                        produto.Quantidade += controle.Quantidade;
                        break;
                    case EstoqueCrontoleEnum.SAIDA:
                        produto.Quantidade -= controle.Quantidade;
                        break;
                }

                _context.ControleEstoque.Add(controle);
                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Estoque registrado com sucesso.";
                return StatusCode(http.Code, http);
            }
            catch (Exception err)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro ao registrar produto estoque.";
                http.Error = err.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpGet("Listar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> ListarControle([FromQuery] ListarRequest query)
        {
            DefaultResponse http = new DefaultResponse()
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes.";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                IQueryable<ControleEstoqueModel> sql = _context.ControleEstoque;
                IQueryable<ControleEstoqueModel> sqlCount = _context.ControleEstoque;

                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql
                        .Include(e => e.Funcionario)
                        .Include(e => e.Produto)
                        .Where((e) => 
                            e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                            e.Observacao.Contains(query.CampoPesquisa) || 
                            e.Quantidade.ToString().Contains(query.CampoPesquisa) ||
                            e.Data.ToString().Contains(query.CampoPesquisa) || 
                            e.Funcionario.Nome.Contains(query.CampoPesquisa) ||
                            e.Produto.Titulo.Contains(query.CampoPesquisa));

                    sqlCount = sqlCount
                        .Include(e => e.Funcionario)
                        .Include(e => e.Produto)
                        .Where((e) => 
                            e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                            e.Observacao.Contains(query.CampoPesquisa) || 
                            e.Quantidade.ToString().Contains(query.CampoPesquisa) ||
                            e.Data.ToString().Contains(query.CampoPesquisa) ||
                            e.Funcionario.Nome.Contains(query.CampoPesquisa) ||
                            e.Produto.Titulo.Contains(query.CampoPesquisa));
                }

                var estoque = sql
                    .Include(x => x.Funcionario)
                    .Include(x => x.Produto)
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                var count = sqlCount
                    .Where(e => e.Status == true)
                    .Count();

                var dados = estoque.Select(e => new ConsultarEstoqueResponse()
                {
                    Codigo = e.Codigo,
                    Quantidade = e.Quantidade,
                    Observacao = e.Observacao,
                    Tipo = e.Tipo.ToString(),
                    Data = e.Data,
                    Funcionario = new ConsultarFuncionarioSimplesResponse()
                    {
                        Codigo = e.Funcionario.Codigo,
                        Nome = e.Funcionario.Nome
                    },
                    Produto = new ConsultarProdutoSimplesResponse()
                    {
                        Codigo = e.Produto.Codigo,
                        Titulo = e.Produto.Titulo,
                        Descricao = e.Produto.Descricao,
                        Valor = e.Produto.Valor
                    }
                });

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = count,
                    Dados = dados
                };

                http.Code = StatusCodes.Status200OK;
                http.Message = "Crontole(s) de estoque encontrada(s).";
                http.Response = list;
                return StatusCode(http.Code, http);
            }
            catch (Exception err)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro ao listar produtos estoque.";
                http.Error = err.Message;
                return StatusCode(http.Code, http);
            }
        }
    }
}