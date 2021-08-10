using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using OrderAPI;
using OrderAPI.Interfaces;
using OrderAPI.Models;
using OrderAPI.Enums;
using OrderAPI.Services;
using OrderAPI.Utils;
using OrderAPI.Database;

namespace OrderAPI.Controllers {

    [Route("api/usuario/")]
    public class CUsuario : ControllerBase, IControllerBase<MUsuario> {

        private DBService _context = new DBService();

        public CUsuario(DBService context) {
            _context = context;
        }

        [HttpPost("registrar/visitante/")]
        public Task<ActionResult<HttpResponse>> RegistrarVisitante([FromBody] MUsuario dados) {
            SystemUtils.Log(EHTTPLog.POST, "route 'api/usuario/registrar/visitante' used");
            HttpResponse httpMessage = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) {
                httpMessage.Message = SystemUtils.Log(ETitleLog.SYSTEM, "Parametros Ausentes!");
                httpMessage.Error = ModelStateService.ErrorConverter(ModelState);
                return Task.FromResult<ActionResult<HttpResponse>>(StatusCode(httpMessage.Code, httpMessage));
            }

            try {

                List<MUsuario> usuarios = _context.Usuario
                    .Where(field => field.Email.Contains(dados.Email.Trim()))
                    .ToList();

                if (usuarios.Count >= 1) {
                    httpMessage.Message = "Email ja cadastrado!";
                    return Task.FromResult<ActionResult<HttpResponse>>(StatusCode(httpMessage.Code, httpMessage));
                }

                MUsuario usuarioDB = new MUsuario() {
                    Nome = dados.Nome.Trim(),
                    Sobrenome = dados.Sobrenome.Trim(),
                    Email = dados.Email.Trim(),
                    Senha = PasswordService.EncryptPassword(dados.Senha),
                    NivelAcesso = EPrevilegios.Visitante,
                    Status = EStatusRegistro.Ativo,
                    DataCadastro = DateTime.Now
                };

                _context.Usuario.Add(usuarioDB);
                _context.SaveChanges();

                httpMessage.Code = (int)EHttpResponse.OK;
                httpMessage.Message = "Visitante cadastrado com sucesso!";
                return Task.FromResult<ActionResult<HttpResponse>>(StatusCode(httpMessage.Code, httpMessage));
                
            } catch (Exception E) {
                httpMessage.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                httpMessage.Message = "Erro interno do servidor!";
                httpMessage.Error = E.Message;
                return Task.FromResult<ActionResult<HttpResponse>>(StatusCode(httpMessage.Code, httpMessage));
            }
        }

        [HttpPost("registrar/comunidade/")]
        public Task<ActionResult<HttpResponse>> RegistrarComunidade([FromBody] MUsuario dados) {
            SystemUtils.Log(EHTTPLog.POST, "route 'api/usuario/registrar/comunidade' used");
            HttpResponse httpMessage = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) {
                httpMessage.Message = SystemUtils.Log(ETitleLog.SYSTEM, "Parametros Ausentes!");
                httpMessage.Error = ModelStateService.ErrorConverter(ModelState);
                return Task.FromResult<ActionResult<HttpResponse>>(StatusCode(httpMessage.Code, httpMessage));
            }

            return Task.FromResult<ActionResult<HttpResponse>>(StatusCode(httpMessage.Code, httpMessage));
        }

        public Task<ActionResult<HttpResponse>> Registrar([FromBody] MUsuario dados) {
            throw new NotImplementedException();
        }

        public Task<ActionResult<HttpResponse>> Alterar([FromBody] MUsuario daods) {
            throw new NotImplementedException();
        }

        public Task<ActionResult<HttpResponse>> Deletar([FromBody] int codigo) {
            throw new NotImplementedException();
        }

        public Task<ActionResult<HttpResponse>> Consultar([FromBody] int codigo) {
            throw new NotImplementedException();
        }

        public Task<ActionResult<HttpResponse>> ConsultarTodos() {
            throw new NotImplementedException();
        }
    }
}
