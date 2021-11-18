using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.API.EntensionMethods;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.API.Services;
using OrderAPI.Data;
using OrderAPI.Data.Helpers;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class PedidoController : ControllerBase
    {
        private OrderAPIContext context;
        private IMapper mapper;
        private ModelService model;
        public PedidoController(OrderAPIContext context, IMapper mapper, ModelService model)
        {   
            this.context = context;
            this.mapper = mapper;
            this.model = model;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "USUARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] PedidoRequest body) 
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

            if (body.Items.Count <= 0) 
            {
                http.Message = "Parametros Ausentes.";
                http.Error = new List<ErrorResponse>() {
                    new ErrorResponse() { Field = "Items", Message = "Pedido deve conter items." }
                };
                return StatusCode(http.Code, http);
            }

            try
            {
                var codigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                var usuario = this.context.Usuario
                    .Where((x) => x.Codigo == codigo && x.Status == true)
                    .SingleOrDefault();
                
                if (usuario == null) 
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Usuario não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var pagto = this.context.MetodoPagamento
                    .Where((x) => x.Codigo == body.MetodoPagamentoCodigo && x.Status == true)
                    .SingleOrDefault();
                
                if (pagto == null) 
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Metodo de Pagamento não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var errors = new List<ErrorResponse>();
                var items = new List<MPedidoItem>();
                foreach (var item in body.Items)
                {
                    var produto = this.context.Produto
                        .Where((x) => x.Codigo == item.ProdutoCodigo && x.Status == true)
                        .SingleOrDefault();
                    
                    if (produto == null)
                    {
                        errors.Add(new ErrorResponse() { 
                            Field = "Produto", 
                            Message = $"Produto de codigo { item.ProdutoCodigo }  não encontrado." 
                        });
                        break;
                    }

                    if (produto.Quantidade <= 0 || (produto.Quantidade - item.Quantidade) < 0)
                    {
                        errors.Add(new ErrorResponse() { 
                            Field = "Produto", 
                            Message = $"Produto {produto.Titulo } esta fora de estoque." 
                        });
                        break;
                    }

                    items.Add(new MPedidoItem() {
                        Produto = produto,
                        ProdutoCodigo = produto.Codigo,
                        Quantidade = item.Quantidade,
                        Valor = (produto.Valor * item.Quantidade)
                    });
                }

                if (errors.Count > 0)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Parametros Ausentes.";
                    http.Error = errors;
                    return StatusCode(http.Code, http);
                }

                MPedido pedido = new MPedido()
                {
                    Data = DateTime.Now,
                    MetodoPagamento = pagto,
                    MetodoPagamentoCodigo = pagto.Codigo,
                    Status = Data.Helpers.PedidoStatusEnum.ABERTO,
                    Observacao = body.Obersavacao,
                    Usuario = usuario,
                    UsuarioCodigo = usuario.Codigo,
                    Items = items
                };

                this.context.Pedido.Add(pedido);
                this.context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Pedido criado com sucesso.";
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

        [HttpGet("Listar/")]
        [Authorize]
        public ActionResult<DefaultResponse> Listar([FromQuery] ListarPedidosRequest query) 
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid)
            {
                response.Message = "Parametros Ausentes.";
                response.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }
            
            try
            {
                IQueryable<MPedido> count = this.context.Pedido;
                IQueryable<MPedido> sql = this.context.Pedido;

                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) =>
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Data.ToString().Contains(query.CampoPesquisa) ||
                        e.Observacao.Contains(query.CampoPesquisa)
                    );
                }

                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    var codigo = Int32.Parse(User.Identity.GetUsuarioCodigo());

                    if (!this.context.Usuario.Any((e) => e.Codigo == codigo && e.Status == true)) 
                    {
                        response.Code = StatusCodes.Status401Unauthorized;
                        response.Message = "Usuario não encontrado.";
                        return StatusCode(response.Code, response);
                    }

                    count = count.Where(e => e.UsuarioCodigo == codigo);
                    sql = sql.Where(e => e.UsuarioCodigo == codigo); 
                }

                var dados = sql.Where(e => e.Status == query.Status)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .OrderBy(e => e.Codigo)
                    .ToList();

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = count.Where((e) => e.Status == query.Status).Count(),
                    Dados = this.mapper.Map<List<ConsultarPedidoSimplesResponse>>(dados)
                };

                response.Code = StatusCodes.Status200OK;
                response.Message = "Pedido(s) encontrados com sucesso.";
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

        [HttpGet("Consultar/")]
        [Authorize]
        public ActionResult<DefaultResponse> Consultar([FromQuery] int codigo)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };
            
            try
            {
                IQueryable<MPedido> sql = this.context.Pedido;
                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    var usuarioCodigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                    if (!this.context.Usuario.Any((e) => e.Codigo == usuarioCodigo && e.Status == true)) 
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Usuario não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    sql = sql.Where((e) => e.UsuarioCodigo == usuarioCodigo);
                }

                var pedido = sql
                    .Where((e) => e.Codigo == codigo)
                    .SingleOrDefault();

                if (pedido == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Pedido não encontrado.";
                    return StatusCode(http.Code, http);
                }
                
                var response = this.mapper.Map<ConsultarPedidoResponse>(pedido);

                var pagto = this.context.MetodoPagamento
                    .Where((e) => e.Codigo == pedido.MetodoPagamentoCodigo)
                    .SingleOrDefault();
                
                response.MetodoPagamento = this.mapper.Map<ConsultarMetodoPagtoResponse>(pagto);

                var items = this.context.PedidoItem
                    .Where((e) => e.PedidoCodigo == pedido.Codigo && e.Status == true)
                    .Include((e) => e.Produto)
                    .ToList();
            
                items.ForEach((e) => {
                    response.Items.Add(new ConsultarPedidoItemResponse(){
                        Codigo = e.Codigo,
                        Titulo = e.Produto.Titulo,
                        Descricao = e.Produto.Descricao,
                        Quantidade = e.Quantidade,
                        Valor = e.Valor
                    }); 
                });

                http.Code = StatusCodes.Status200OK;
                http.Message = "Pedido encontrado com sucesso.";
                http.Response = response;                
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

        [HttpGet("Cancelar/")]
        [Authorize]
        public ActionResult<DefaultResponse> Cancelar([FromQuery] int codigo)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };
            
            try
            {
                IQueryable<MPedido> sql = this.context.Pedido;
                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    var usuarioCodigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                    if (!this.context.Usuario.Any((e) => e.Codigo == usuarioCodigo && e.Status == true)) 
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Usuario não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    sql = sql.Where((e) => e.UsuarioCodigo == usuarioCodigo);
                }

                var pedido = sql
                    .Where((e) => e.Codigo == codigo)
                    .Where((e) => e.Status == PedidoStatusEnum.ABERTO)
                    .SingleOrDefault();

                if (pedido == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Pedido não encontrado.";
                    return StatusCode(http.Code, http);
                }
                
                pedido.Status = PedidoStatusEnum.CANCELADO;
                this.context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Pedido cancelado.";             
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

        [HttpGet("Retirar/")]
        [Authorize(Roles = "FUNCIONARIO, GERENTE, MASTER")]
        public ActionResult<DefaultResponse> Retirar([FromQuery] int codigo)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };
            
            try
            {
                MPedido pedido = this.context.Pedido
                    .Where((e) => e.Codigo == codigo && e.Status == PedidoStatusEnum.ABERTO)
                    .SingleOrDefault();
                
                if (pedido == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Pedido não encontrado.";
                    return StatusCode(http.Code, http);
                }
                
                pedido.Status = PedidoStatusEnum.RETIRADO;
                this.context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Pedido retirado.";             
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
    }
}