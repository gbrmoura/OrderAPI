using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderAPI.Data.Request;
using OrderAPI.Enums;
using OrderAPI.Services;
using OrderAPI.Models;
using OrderAPI.Data.Response;

namespace OrderAPI.Controllers {

    [Route("api/metodoPagto/")]
    public class CMetodoPagamento : ControllerBase {

        private DBContext _context;
        private IMapper _mapper;
        private TokenService _jwtService;

        public CMetodoPagamento(DBContext context, IMapper mapper, TokenService jwtService) {
            this._context = context;
            this._mapper = mapper;
            this._jwtService = jwtService;
        }

        [HttpPost("registrar/")]
        [Authorize(Roles = "MASTER, GERENTE")]
        public ActionResult<HttpResponse> Registrar([FromBody] CriarMetodoPagtoRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                MMetodoPagamento MetodoPagamento = _context.MetodoPagamento.FirstOrDefault(pagto => pagto.Nome.Equals(dados.Nome));

                if (MetodoPagamento != null) {
                    response.Message = "Método de Pagamento já cadastrado!";
                    return StatusCode(response.Code, response);
                }    

                MMetodoPagamento pagtoDB = _mapper.Map<MMetodoPagamento>(dados); 

                _context.Add(pagtoDB); 
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Método de Pagamento cadastrado com sucesso!";
                return StatusCode(response.Code, response);   

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        } 
    }
}
