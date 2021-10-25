using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                List<MPedido> pedidos = new List<MPedido>();
                if (Guid.TryParse(query.UsuarioCodigo, out var codigo ))
                {
                    MUsuario usuario = _context.Usuario
                        .FirstOrDefault((x) => x.Codigo == codigo && x.Status == true);

                    if (usuario == null) 
                    {
                        response.Code = StatusCodes.Status404NotFound;
                        response.Message = "Usuario não encontrado.";
                        return StatusCode(response.Code, response);
                    }

                    pedidos = _context.Pedido
                        .Where(e => e.UsuarioCodigo == codigo && e.Status == query.Status)
                        .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                        .Take(query.TamanhoPagina)
                        .OrderBy(e => e.Numero)
                        .ToList();
                }
                else 
                {
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
                    NumeroRegistros = pedidos.Count,
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
        public ActionResult<DefaultResponse> Consultar([FromQuery] Guid codigo)
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
                MPedido pedido = _context.Pedido
                    .FirstOrDefault((e) => e.Codigo == codigo);
                
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
        public ActionResult<DefaultResponse> Cancelar([FromQuery] Guid codio)
        {
            return NotFound(); // TODO: Fazer metodo de cancelar
        }
    }
}