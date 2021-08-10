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
using OrderAPI.Data.DTO;
using AutoMapper;

namespace OrderAPI.Controllers {

    [Route("api/usuario/")]
    public class CUsuario : ControllerBase, IControllerBase<MUsuario> {

        private DBContext _context;
        private IMapper _mapper;

        public CUsuario(DBContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("registrar/visitante/")]
        public ActionResult<HttpResponse> RegistrarVisitante([FromBody] DTOCriarUsuario dados) {
            SystemUtils.Log(EHTTPLog.POST, "route 'api/usuario/registrar/visitante' used");
            HttpResponse httpMessage = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) {
                httpMessage.Message = "Parametros Ausentes!";
                httpMessage.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(httpMessage.Code, httpMessage);
            }

            try {

                MUsuario usuario = _context.Usuario.
                    FirstOrDefault(user => user.Email.Equals(dados.Email));

                if (usuario != null) {
                    httpMessage.Message = "Email ja cadastrado!";
                    return StatusCode(httpMessage.Code, httpMessage);
                }

                MUsuario usuarioDB = _mapper.Map<MUsuario>(dados);
                usuarioDB.Senha = PasswordService.EncryptPassword(dados.Senha);
                usuarioDB.NivelAcesso = EPrevilegios.Visitante;
                usuarioDB.Status = EStatusRegistro.Ativo;
                usuarioDB.DataCadastro = DateTime.Now;
                
                _context.Usuario.Add(usuarioDB);
                _context.SaveChanges();

                httpMessage.Code = (int)EHttpResponse.OK;
                httpMessage.Message = "Visitante cadastrado com sucesso!";
                return StatusCode(httpMessage.Code, httpMessage);
                
            } catch (Exception E) {
                httpMessage.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                httpMessage.Message = "Erro interno do servidor!";
                httpMessage.Error = E.Message;
                return StatusCode(httpMessage.Code, httpMessage);
            }
        }
        public ActionResult<HttpResponse> Registrar([FromBody] MUsuario dados) {
            throw new NotImplementedException();
        }

        public ActionResult<HttpResponse> Alterar([FromBody] MUsuario daods) {
            throw new NotImplementedException();
        }
        public ActionResult<HttpResponse> Deletar(int codigo) {
            throw new NotImplementedException();
        }

        [HttpGet("consultar/{codigo}")]
        public ActionResult<HttpResponse> Consultar(int codigo) {
            SystemUtils.Log(EHTTPLog.GET, "route 'api/usuario/consultar/' used");
            HttpResponse httpMessage = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Todos usuario consultados."
            };

            try {

                MUsuario usuario = _context.Usuario
                    .FirstOrDefault(user => user.Codigo == codigo);

                if (usuario == null) {
                    httpMessage.Code = (int)EHttpResponse.NOT_FOUND;
                    httpMessage.Message = $"Usuario de codigo {codigo}, não encontrado.";
                }

                DTOConsultarUsuario dtoUsuario = _mapper.Map<DTOConsultarUsuario>(usuario);

                httpMessage.Code = (int)EHttpResponse.OK;
                httpMessage.Message = "Usuario encontrado.";
                httpMessage.Response = dtoUsuario;
                return StatusCode(httpMessage.Code, httpMessage);

            } catch (Exception E) {
                httpMessage.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                httpMessage.Message = "Erro interno do servidor.";
                httpMessage.Error = E.Message;
                return StatusCode(httpMessage.Code, httpMessage);
            }
        }

        [HttpGet("consultar/")]
        public ActionResult<HttpResponse> ConsultarTodos() {
            SystemUtils.Log(EHTTPLog.GET, "route 'api/usuario/consultar/{codigo}' used");
            HttpResponse httpMessage = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Todos usuario consultados"
            };

            try {
                
                List<MUsuario> usuarios = _context.Usuario.ToList();

                if (usuarios.Count <= 0) {
                    httpMessage.Code = (int)EHttpResponse.NOT_FOUND;
                    httpMessage.Message = "Usuario não encontrados";
                    return StatusCode(httpMessage.Code, httpMessage);
                }

                List<DTOConsultarUsuario> dtoUsuarios = new List<DTOConsultarUsuario>();
                usuarios.ForEach(usuario => {
                    dtoUsuarios.Add(_mapper.Map<DTOConsultarUsuario>(usuario));
                });

                httpMessage.Code = (int)EHttpResponse.OK;
                httpMessage.Message = "Usuario encontrados";
                httpMessage.Response = dtoUsuarios;
                return StatusCode(httpMessage.Code, httpMessage);

            } catch (Exception E) {
                httpMessage.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                httpMessage.Message = "Erro interno do servidor";
                httpMessage.Error = E.Message;
                return StatusCode(httpMessage.Code, httpMessage);
            }
        }
    }
}
