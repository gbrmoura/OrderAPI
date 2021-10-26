using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private OrderAPIContext _context;

        private IMapper _mapper;

        public PedidoController(OrderAPIContext context, IMapper mapper)
        {   
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "USUARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] PedidoRequest body)
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

            if (body.Items.Count <= 0) 
            {
                response.Message = "Pedido deve conter items para ser registrado.";
                return StatusCode(response.Code, response);
            }

            try
            {
                MUsuario usuario = _context.Usuario
                    .FirstOrDefault((x) => x.Codigo == body.UsuarioCodigo && x.Status == true);

                if (usuario == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Usuario não encontrado.";
                    return StatusCode(response.Code, response);
                }

                MMetodoPagamento metodoPagamento = _context.MetodoPagamento
                    .FirstOrDefault((x) => x.Codigo == body.MetodoPagamentoCodigo && x.Status == true);

                if (metodoPagamento == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Metodo de Pagamento não encontrado.";
                    return StatusCode(response.Code, response);
                }

                List<MPedidoItem> pedidoItems = new();
                foreach (PedidoItemRequest item in body.Items)
                {
                    MProduto produto = _context.Produto
                        .FirstOrDefault((x) => x.Codigo == item.ProdutoCodigo && x.Status == true);
                    
                    if (produto == null) 
                    {
                        response.Code = StatusCodes.Status404NotFound;
                        response.Message = $"Produto de codigo { item.ProdutoCodigo } não encontrado.";
                        return StatusCode(response.Code, response);
                    }
                
                    MPedidoItem pedidoItem = new MPedidoItem()
                    {
                        Produto = produto,
                        ProdutoCodigo = produto.Codigo,
                        Quantidade = item.Quantidade,
                        Valor = (item.Quantidade * produto.Valor)
                    };

                    pedidoItems.Add(pedidoItem);
                }

                var numeroPedido = _context.Pedido.ToList().Count() + 1;

                MPedido pedido = new MPedido()
                {
                    Data = DateTime.Now,
                    Numero = numeroPedido,
                    MetodoPagamento = metodoPagamento,
                    MetodoPagamentoCodigo = metodoPagamento.Codigo,
                    Status = Data.Helpers.PedidoStatusEnum.ABERTO,
                    Observacao = body.Obersavacao,
                    Usuario = usuario,
                    UsuarioCodigo = usuario.Codigo,
                    Items = pedidoItems
                };

                _context.Pedido.Add(pedido);
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Pedido criado com sucesso.";
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
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }
            
            try
            {
                var count = 0;
                var pedidos = new List<MPedido>();

                if (IdentityService.getRole(User.Claims) == PrevilegioEnum.USUARIO.ToString())
                {

                    if (String.IsNullOrEmpty(query.UsuarioCodigo) || !Guid.TryParseExact( query.UsuarioCodigo,"D",out var codigo))
                    {
                        response.Message = "Erro ao listar pedido(s)";
                        response.Error = new List<ErrorResponse>()
                        {
                            new ErrorResponse() { Field = "UsuarioCodigo", Message = "Codigo de usuario deve ser informado." }
                        };
                        return StatusCode(response.Code, response);
                    }

                    MUsuario usuario = _context.Usuario.FirstOrDefault((x) => x.Codigo == codigo && x.Status == true);

                    if (usuario == null) 
                    {
                        response.Code = StatusCodes.Status404NotFound;
                        response.Message = "Usuario não encontrado.";
                        return StatusCode(response.Code, response);
                    }

                    count = _context.Pedido.Where(e => e.UsuarioCodigo == codigo && e.Status == query.Status).Count();
                    pedidos = _context.Pedido
                        .Where(e => e.UsuarioCodigo == codigo && e.Status == query.Status)
                        .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                        .Take(query.TamanhoPagina)
                        .OrderBy(e => e.Numero)
                        .ToList();     
                }
                else
                {
                    count = _context.Pedido.Where(e => e.Status == query.Status).Count();
                    pedidos = _context.Pedido
                        .Where(e => e.Status == query.Status)
                        .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                        .Take(query.TamanhoPagina)
                        .OrderBy(e => e.Numero)
                        .ToList();
                }
                
                if (pedidos.Count <= 0) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Nenhum pedido encontrado.";
                    return StatusCode(response.Code, response);
                }

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = count,
                    Dados = _mapper.Map<List<ConsultarPedidoSimplesResponse>>(pedidos)
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
        public ActionResult<DefaultResponse> Consultar(
            [FromQuery] string codigo,
            [FromQuery] string usuarioCodigo)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (String.IsNullOrEmpty(codigo) || !Guid.TryParseExact(codigo,"D",out var pCodigo))
            {
                response.Message = "Parametros Ausentes.";
                response.Error = new List<ErrorResponse>() {
                    new ErrorResponse() { Field = "Codigo", Message = "Codigo do pedido deve ser informado." }
                };
                return StatusCode(response.Code, response);
            }
            
            try
            {
                MPedido pedido = new MPedido();

                if (IdentityService.getRole(User.Claims) == PrevilegioEnum.USUARIO.ToString())
                {
                    if (String.IsNullOrEmpty(usuarioCodigo) || !Guid.TryParseExact(usuarioCodigo,"D",out var uCodigo))
                    {
                        response.Message = "Erro ao consultar pedido";
                        response.Error = new List<ErrorResponse>() {
                            new ErrorResponse() { Field = "UsuarioCodigo", Message = "Codigo de usuario deve ser informado." }
                        };
                        return StatusCode(response.Code, response);
                    }

                    pedido = _context.Pedido.FirstOrDefault(e => e.Codigo == pCodigo && e.UsuarioCodigo == uCodigo);
                }
                else
                {
                    pedido = _context.Pedido.FirstOrDefault((e) => e.Codigo == pCodigo);
                }

                if (pedido == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Pedido não encontrado.";
                    return StatusCode(response.Code, response);
                }
                
                ConsultarPedidoResponse pedidoResponse = _mapper.Map<ConsultarPedidoResponse>(pedido);

                MMetodoPagamento metodoPagamento = _context.MetodoPagamento
                    .FirstOrDefault((e) => e.Codigo == pedido.MetodoPagamentoCodigo);
                
                pedidoResponse.MetodoPagamento = _mapper.Map<ConsultarMetodoPagtoResponse>(metodoPagamento);

                var items = _context.PedidoItem
                    .Where((e) => e.PedidoCodigo == pedido.Codigo && e.Status == true)
                    .Include((e) => e.Produto)
                    .ToList();
                
                foreach (var item in items)
                {
                    pedidoResponse.Items.Add(new ConsultarPedidoItemResponse(){
                        Codigo = item.Codigo,
                        Titulo = item.Produto.Titulo,
                        Descricao = item.Produto.Descricao,
                        Quantidade = item.Quantidade,
                        Valor = item.Valor
                    });
                }

                response.Code = StatusCodes.Status200OK;
                response.Message = "Pedido encontrado com sucesso.";
                response.Response = pedidoResponse;                
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

        [HttpGet("Cancelar/")]
        [Authorize]
        public ActionResult<DefaultResponse> Cancelar([FromQuery] string codigo)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (String.IsNullOrEmpty(codigo) || !Guid.TryParseExact(codigo,"D",out var pCodigo))
            {
                response.Message = "Parametros Ausentes.";
                response.Error = new List<ErrorResponse>() {
                    new ErrorResponse() { Field = "Codigo", Message = "Codigo do pedido deve ser informado." }
                };
                return StatusCode(response.Code, response);
            }
            
            try
            {
                MPedido pedido = _context.Pedido.FirstOrDefault((e) => e.Codigo == pCodigo && e.Status == PedidoStatusEnum.ABERTO);
                
                if (pedido == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Pedido não encontrado.";
                    return StatusCode(response.Code, response);
                }
                
                pedido.Status = PedidoStatusEnum.CANCELADO;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Pedido cancelado.";             
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

        [HttpGet("Retirar/")]
        [Authorize(Roles = "FUNCIONARIO, GERENTE, MASTER")]
        public ActionResult<DefaultResponse> Retirar([FromQuery] string codigo)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (String.IsNullOrEmpty(codigo) || !Guid.TryParseExact(codigo,"D",out var pCodigo))
            {
                response.Message = "Parametros Ausentes.";
                response.Error = new List<ErrorResponse>() {
                    new ErrorResponse() { Field = "Codigo", Message = "Codigo do pedido deve ser informado." }
                };
                return StatusCode(response.Code, response);
            }
            
            try
            {
                MPedido pedido = _context.Pedido.FirstOrDefault((e) => e.Codigo == pCodigo && e.Status == PedidoStatusEnum.ABERTO);
                
                if (pedido == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Pedido não encontrado.";
                    return StatusCode(response.Code, response);
                }
                
                pedido.Status = PedidoStatusEnum.RETIRADO;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Pedido cancelado.";             
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