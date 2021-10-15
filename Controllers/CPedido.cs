// using System;
// using System.Collections.Generic;
// using System.Linq;
// using AutoMapper;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using OrderAPI.Database;
// using OrderAPI.Enums;
// using OrderAPI.Models;
// using OrderAPI.Services;
// using OrderAPI.Data.Request;
// using OrderAPI.Data.Response;

// namespace OrderAPI.Controllers {

//     [Route("api/pedido/")]
//     public class CPedido: ControllerBase {
//         private OrderAPIContext _context;
//         private IMapper _mapper;

//         public CPedido(OrderAPIContext context, IMapper mapper) {
//             this._context = context;
//             this._mapper = mapper;
//         }

//         [HttpPost("registrar")]
//         [Authorize(Roles = "USUARIO")]
//         public ActionResult<HttpResponse> Registrar() {
//             HttpResponse response = new HttpResponse() {
//                 Code = (int)EHttpResponse.UNAUTHORIZED,
//                 Message = "Rota não autorizada"
//             };

//             return StatusCode(response.Code, response);
//         } 

//         [HttpPost("cancelar")]
//         [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
//         public ActionResult<HttpResponse> Cancelar() {
//             HttpResponse response = new HttpResponse() {
//                 Code = (int)EHttpResponse.UNAUTHORIZED,
//                 Message = "Rota não autorizada"
//             };

//             return StatusCode(response.Code, response);
//         } 


//     }

// }