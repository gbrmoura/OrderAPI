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

namespace OrderAPI.API.Controllers
{
    [Route("api/Produto/")]
    public class EstoqueController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;

        public EstoqueController(OrderAPIContext context, IMapper mapper, TokenService jwtService)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Controle/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Controle([FromBody] EstoqueRequest body)
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

            if (!UtilsService.CompareStrings(body.Tipo, new string[]{ EstoqueCrontoleEnum.ENTRADA.ToString(), EstoqueCrontoleEnum.SAIDA.ToString()}))
            {
                response.Message = "Tipo de operação inválida. ";
                response.Error = new List<ErrorResponse>() {
                    new ErrorResponse() { Field = "Tipo", Message = "Tipo deve estrar entre 'ENTRADA' e 'SAIDA'" }
                };
                return StatusCode(response.Code, response);
            }

            try
            {
                var funcionario = _context.Funcionario.SingleOrDefault(x => x.Codigo == body.FuncionarioCodigo && x.Status == true);
                if (funcionario == null) 
                {
                    response.Message = "Funcionario não encontrado.";
                    return StatusCode(response.Code, response);
                }

                var produto = _context.Produto.SingleOrDefault(x => x.Codigo == body.ProdutoCodigo);
                if (produto == null) 
                {
                    response.Message = "Produto não encontrado.";
                    return StatusCode(response.Code, response);
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

                _context.ControleEstoque.Add(controle);
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Estoque registrado com sucesso.";
                return StatusCode(response.Code, response);
            }
            catch (Exception err)
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro ao registrar produto estoque.";
                response.Error = err.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("ListarCrontole/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> ListarControle([FromQuery] ListarRequest query)
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

                IQueryable<MControleEstoque> sql = _context.ControleEstoque;
                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) => 
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Observacao.Contains(query.CampoPesquisa) || 
                        e.Data.ToString().Contains(query.CampoPesquisa) || 
                        e.Tipo.ToString().Contains(query.CampoPesquisa));
                }

                var categorias = sql
                    .Include(x => x.Funcionario)
                    .Include(x => x.Produto)
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                var dados = categorias.Select(e => new ConsultarEstoqueResponse()
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
                    NumeroRegistros = _context.Categoria.Where(e => e.Status == true).Count(),
                    Dados = dados
                };

                response.Code = StatusCodes.Status200OK;
                response.Message = "Crontole(s) de estoque encontrada(s).";
                response.Response = list;
                return StatusCode(response.Code, response);
            }
            catch (Exception err)
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro ao listar produtos estoque.";
                response.Error = err.Message;
                return StatusCode(response.Code, response);
            }
        }
    }
}