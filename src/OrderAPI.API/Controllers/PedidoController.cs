using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.API.EntensionMethods;
using OrderAPI.Domain.Http;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.API.Services;
using OrderAPI.Data;
using OrderAPI.Domain.Enums;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class PedidoController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;
        public PedidoController(OrderAPIContext context, IMapper mapper, ModelService model)
        {
            _context = context;
            _mapper = mapper;
            _model = model;
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
                http.Error = _model.ErrorConverter(ModelState);
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
                var usuario = _context.Usuario
                    .Where((x) => x.Codigo == codigo && x.Status == true)
                    .SingleOrDefault();

                if (usuario == null)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Usuario não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var pagto = _context.MetodoPagamento
                    .Where((x) => x.Codigo == body.MetodoPagamentoCodigo && x.Status == true)
                    .SingleOrDefault();

                if (pagto == null)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Metodo de Pagamento não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var errors = new List<ErrorResponse>();
                var items = new List<PedidoItemModel>();
                foreach (var item in body.Items)
                {
                    var produto = _context.Produto.AsNoTracking()
                        .Where((x) => x.Codigo == item.ProdutoCodigo && x.Status == true)
                        .SingleOrDefault();

                    if (produto == null)
                    {
                        errors.Add(new ErrorResponse()
                        {
                            Field = "Produto",
                            Message = $"Produto de codigo { item.ProdutoCodigo }  não encontrado."
                        });
                        break;
                    }

                    if (produto.Quantidade <= 0 || (produto.Quantidade - item.Quantidade) < 0)
                    {
                        errors.Add(new ErrorResponse()
                        {
                            Field = "Produto",
                            Message = $"Produto {produto.Titulo } esta fora de estoque."
                        });
                        break;
                    }

                    

                    items.Add(new PedidoItemModel()
                    {
                        Produto = produto,
                        ProdutoCodigo = produto.Codigo,
                        Quantidade = item.Quantidade,
                        Valor = (produto.Valor * item.Quantidade)
                    });

                    produto.Quantidade -= item.Quantidade;
                    _context.Produto.Update(produto);
                
                }

                if (errors.Count > 0)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Parametros Ausentes.";
                    http.Error = errors;
                    return StatusCode(http.Code, http);
                }

                PedidoModel pedido = new PedidoModel()
                {
                    Data = DateTime.Now,
                    MetodoPagamento = pagto,
                    MetodoPagamentoCodigo = pagto.Codigo,
                    Status = PedidoStatusEnum.ABERTO,
                    Observacao = body.Observacao,
                    Usuario = usuario,
                    UsuarioCodigo = usuario.Codigo,
                    Items = items
                };

                _context.Pedido.Add(pedido);
                _context.SaveChanges();

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
                response.Error = _model.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                IQueryable<PedidoModel> count = _context.Pedido;
                IQueryable<PedidoModel> sql = _context.Pedido;

                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql
                        .Include(x => x.Usuario)
                        .Include(x => x.MetodoPagamento)
                        .Where((e) =>
                            e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                            e.Data.ToString().Contains(query.CampoPesquisa) ||
                            e.Observacao.Contains(query.CampoPesquisa) || 
                            e.Usuario.Nome.Contains(query.CampoPesquisa) ||
                            e.MetodoPagamento.Titulo.Contains(query.CampoPesquisa)
                        );

                    count = count
                        .Include(x => x.Usuario)
                        .Include(x => x.MetodoPagamento)
                        .Where((e) =>
                            e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                            e.Data.ToString().Contains(query.CampoPesquisa) ||
                            e.Observacao.Contains(query.CampoPesquisa) ||
                            e.Usuario.Nome.Contains(query.CampoPesquisa) ||
                            e.MetodoPagamento.Titulo.Contains(query.CampoPesquisa)
                        );
                }


                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    var codigo = Int32.Parse(User.Identity.GetUsuarioCodigo());

                    if (!_context.Usuario.Any((e) => e.Codigo == codigo && e.Status == true))
                    {
                        response.Code = StatusCodes.Status401Unauthorized;
                        response.Message = "Usuario não encontrado.";
                        return StatusCode(response.Code, response);
                    }

                    sql = sql.Where(e => e.UsuarioCodigo == codigo);
                    count = count.Where(e => e.UsuarioCodigo == codigo);
                }

                if (query.Status != PedidoStatusEnum.UNDEFINED)
                {
                    sql = sql.Where(e => e.Status == query.Status);
                }

                var dados = sql.Include(e => e.MetodoPagamento)
                    .Include(e => e.Usuario)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .OrderBy(e => e.Codigo)
                    .ToList();

                var result = dados.Select(x => new 
                {
                    Codigo = x.Codigo,
                    Data = x.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                    Observacao = x.Observacao,
                    Status = x.Status.ToString().ToLower().Capitalize(),
                    MetodoPagamento = new 
                    {
                        Codigo = x.MetodoPagamento.Codigo,
                        Titulo = x.MetodoPagamento.Titulo
                    },
                    Usuario = new 
                    {
                        Codigo = x.Usuario.Codigo,
                        Nome = x.Usuario.Nome
                    }
                });

                ListarResponse list = new ListarResponse
                {
                    NumeroRegistros = count.Where((e) => e.Status == query.Status).Count(),
                    Dados = result
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
                IQueryable<PedidoModel> sql = _context.Pedido;
                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    var usuarioCodigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                    if (!_context.Usuario.Any((e) => e.Codigo == usuarioCodigo && e.Status == true))
                    {
                        http.Code = StatusCodes.Status401Unauthorized;
                        http.Message = "Usuario não encontrado.";
                        return StatusCode(http.Code, http);
                    }

                    sql = sql.Where((e) => e.UsuarioCodigo == usuarioCodigo);
                }

                var pedido = sql
                    .Include(e => e.MetodoPagamento)
                    .Include(e => e.Usuario)
                    .Include(e => e.Items)
                        .ThenInclude(e => e.Produto)
                    .Where((e) => e.Codigo == codigo)
                    .SingleOrDefault();

                if (pedido == null)
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Pedido não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var result = new
                {
                    Codigo = pedido.Codigo,
                    Data = pedido.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                    Observacao = pedido.Observacao,
                    Status = pedido.Status.ToString().ToLower().Capitalize(),
                    MetodoPagamento = new
                    {
                        Codigo = pedido.MetodoPagamento.Codigo,
                        Titulo = pedido.MetodoPagamento.Titulo
                    },
                    Usuario = new
                    {
                        Codigo = pedido.Usuario.Codigo,
                        Nome = pedido.Usuario.Nome
                    },
                    Items = pedido.Items.Select(e => new
                    {
                        Codigo = e.Codigo,
                        Produto = new
                        {
                            Codigo = e.Produto.Codigo,
                            Titulo = e.Produto.Titulo,
                            Descricao = e.Produto.Descricao
                        },
                        Quantidade = e.Quantidade,
                        Valor = e.Valor
                    })
                };

                http.Code = StatusCodes.Status200OK;
                http.Message = "Pedido encontrado com sucesso.";
                http.Response = result;
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
                IQueryable<PedidoModel> sql = _context.Pedido;
                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    var usuarioCodigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                    if (!_context.Usuario.Any((e) => e.Codigo == usuarioCodigo && e.Status == true))
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

                if (pedido.Status == PedidoStatusEnum.RETIRADO)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Pedido já foi retirado.";
                    return StatusCode(http.Code, http);
                }

                if (pedido.Status == PedidoStatusEnum.CANCELADO)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Pedido já foi cancelado";
                    return StatusCode(http.Code, http);
                }

                pedido.Status = PedidoStatusEnum.CANCELADO;
                _context.SaveChanges();

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
                IQueryable<PedidoModel> sql = _context.Pedido;
                if (User.Identity.GetUsuarioPrivilegio() == PrevilegioEnum.USUARIO.ToString())
                {
                    var usuarioCodigo = Int32.Parse(User.Identity.GetUsuarioCodigo());
                    if (!_context.Usuario.Any((e) => e.Codigo == usuarioCodigo && e.Status == true))
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

                if (pedido.Status == PedidoStatusEnum.CANCELADO)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Pedido já foi cancelado.";
                    return StatusCode(http.Code, http);
                }

                if (pedido.Status == PedidoStatusEnum.RETIRADO)
                {
                    http.Code = StatusCodes.Status401Unauthorized;
                    http.Message = "Pedido já foi retirado.";
                    return StatusCode(http.Code, http);
                }

                pedido.Status = PedidoStatusEnum.RETIRADO;
                _context.SaveChanges();

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