using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.Services;
using OrderAPI.Data;

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
            
            
            return NotFound();
        }


    }
}