using System;
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
using System.Collections.Generic;
using OrderAPI.API.HTTP.Response;
using Microsoft.EntityFrameworkCore;
using OrderAPI.API.EntensionMethods;

namespace OrderAPI.API.Controllers
{
    [Route("api/Produto/")]
    public class EstoqueController : ControllerBase
    {
        private OrderAPIContext context;
        private IMapper mapper;
        private ModelService model;
        private UtilsService utils;

        public EstoqueController(OrderAPIContext context, IMapper mapper, ModelService model, UtilsService utils)
        {
            this.context = context;
            this.mapper = mapper;
            this.model = model;
            this.utils = utils;
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            if (!this.utils.CompareStrings(body.Tipo, new string[]{ "ENTRADA", "SAIDA" }))
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
                var funcionario = this.context.Funcionario
                    .Where(x => x.Codigo == funcionarioCodigo && x.Status == true)
                    .SingleOrDefault();
                    
                if (funcionario == null)
                {
                    http.Message = "Funcionario não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var produto = this.context.Produto
                    .Where(x => x.Codigo == body.ProdutoCodigo && x.Status == true)
                    .SingleOrDefault();

                if (produto == null) 
                {
                    http.Message = "Produto não encontrado.";
                    return StatusCode(http.Code, http);
                }

                MControleEstoque controle = new MControleEstoque()
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

                this.context.ControleEstoque.Add(controle);
                this.context.SaveChanges();

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

        [HttpGet("ListarCrontole/")]
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {

                IQueryable<MControleEstoque> sql = this.context.ControleEstoque;
                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) => 
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Observacao.Contains(query.CampoPesquisa) || 
                        e.Data.ToString().Contains(query.CampoPesquisa) || 
                        e.Tipo.ToString().Contains(query.CampoPesquisa));
                }

                var estoque = sql
                    .Include(x => x.Funcionario)
                    .Include(x => x.Produto)
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

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
                    NumeroRegistros = this.context.ControleEstoque.Where(e => e.Status == true).Count(),
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