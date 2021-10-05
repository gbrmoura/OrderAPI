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

namespace OrderAPI.Controllers
{

    [Route("api/metodoPagto/")]
    public class CMetodoPagamento : ControllerBase {

        private DBContext _context;
        private IMapper _mapper;

        public CMetodoPagamento(DBContext context, IMapper mapper) {
            this._context = context;
            this._mapper = mapper;
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
                pagtoDB.Status = true;

                _context.Add(pagtoDB);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.CREATED;
                response.Message = "Método de Pagamento cadastrado com sucesso!";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("consultar/")]
        [Authorize(Roles = "MASTER, GERENTE")]
        public ActionResult<HttpResponse> Consultar(int codigo) {
            HttpResponse httpMessage = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada!"
            };

            try {
                MMetodoPagamento metodoPagto = _context.MetodoPagamento.FirstOrDefault(pagto => pagto.Codigo == codigo);

                if (metodoPagto == null) {
                    httpMessage.Code = (int)EHttpResponse.NOT_FOUND;
                    httpMessage.Message = $"Método de Pagto. de código {codigo}, não encontrado.";
                    return StatusCode(httpMessage.Code, httpMessage);
                }

                ConsultarMetodoPagtoResponse dbPagto = _mapper.Map<ConsultarMetodoPagtoResponse>(metodoPagto);

                httpMessage.Code = (int)EHttpResponse.OK;
                httpMessage.Message = "Método de Pagto. encontrado.";
                httpMessage.Response = dbPagto;
                return StatusCode(httpMessage.Code, httpMessage);
            }
            catch (Exception E) {
                httpMessage.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                httpMessage.Message = "Erro interno do servidor.";
                httpMessage.Error = E.Message;
                return StatusCode(httpMessage.Code, httpMessage);
            }
        }

        [HttpPost("alterar/")]
        [Authorize(Roles = "MASTER, GERENTE")]
        public ActionResult<HttpResponse> Alterar([FromBody] AlterarMetodoPagtoRequest dados) {
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
                MMetodoPagamento metodoPagto = _context.MetodoPagamento.FirstOrDefault((pagto) => pagto.Codigo == dados.Codigo);

                if (metodoPagto == null) {
                    response.Code = (int)EHttpResponse.NOT_FOUND;
                    response.Message = "Método Pagto não encontrada.";
                    return StatusCode(response.Code, response);
                }

                _mapper.Map(dados, metodoPagto);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Método Pagto alterado com sucesso";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("deletar/{codigo}")]
        [Authorize(Roles = "MASTER, GERENTE")]
        public ActionResult<HttpResponse> Deletar(int codigo) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            try {
                MMetodoPagamento metodoPagto = _context.MetodoPagamento.FirstOrDefault((pagto) => pagto.Codigo == codigo);

                if (metodoPagto == null) {
                    response.Message = "Método de Pagto não encontrado.";
                    return StatusCode(response.Code, response);
                }

                metodoPagto.Status = false;
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Método de Pagto deletada com sucesso";
                return StatusCode(response.Code, response);
            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("listar/")]
        [Authorize(Roles = "MASTER, GERENTE")]
        public ActionResult<HttpResponse> Listar() {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada!"
            };

            try {
                List<MMetodoPagamento> metodoPagto = _context.MetodoPagamento.Where((pagto) => pagto.Status == true).ToList();

                if (metodoPagto.Count <= 0) {
                    response.Code = (int)EHttpResponse.NOT_FOUND;
                    response.Message = "Nenhum Método de Pagto encontrado";
                    return StatusCode(response.Code, response);
                }

                List<ConsultarMetodoPagtoResponse> metodoPagtoDB = _mapper.Map<List<ConsultarMetodoPagtoResponse>>(metodoPagto);

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Método(s) de Pagto encontrado(s).";
                response.Response = metodoPagtoDB;
                return StatusCode(response.Code, response);
            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor.";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }
    }
}
