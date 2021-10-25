using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.Services;
using OrderAPI.Data;
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
                response.Message = "Produtos são necessarios para fazer login.";
                return StatusCode(response.Code, response);
            }
            
            try
            {
                // TODO: usuario
                MUsuario usuario = _context.Usuario
                    .FirstOrDefault((x) => x.Codigo == body.UsuarioCodigo && x.Status == true);

                if (usuario == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Usuario não encontrado.";
                    return StatusCode(response.Code, response);
                }


                // TODO: metodo pagamento
                MMetodoPagamento metodoPagamento = _context.MetodoPagamento
                    .FirstOrDefault((x) => x.Codigo == body.MetodoPagamentoCodigo && x.Status == true);

                if (metodoPagamento == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Metodo de Pagamento não encontrado.";
                    return StatusCode(response.Code, response);
                }

                // TODO: pedido itens
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
                        Valor = produto.Valor
                    };

                    pedidoItems.Add(pedidoItem);
                }

                MPedido pedido = new MPedido()
                {
                    Data = DateTime.Now,
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


    }
}