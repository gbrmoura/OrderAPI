﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using OrderAPI.Models;
using OrderAPI.Enums;
using OrderAPI.Services;
using OrderAPI.Database;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;

namespace OrderAPI.Controllers {

    [Route("api/usuario/")]
    public class CUsuario : ControllerBase {

        private DBContext _context;
        private IMapper _mapper;

        public CUsuario(DBContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("registrar/")]
        [AllowAnonymous]
        public ActionResult<HttpResponse> Registrar([FromBody] CriarUsuarioRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes!";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                MUsuario usuario = _context.Usuario.FirstOrDefault(user => user.Email.Equals(dados.Email));

                if (usuario != null) {
                    response.Message = "Email ja cadastrado!";
                    return StatusCode(response.Code, response);
                }

                MUsuario usuarioDB = _mapper.Map<MUsuario>(dados);
                usuarioDB.Senha = PasswordService.EncryptPassword(dados.Senha);
                
                _context.Usuario.Add(usuarioDB);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Usuario cadastrado com sucesso!";
                return StatusCode(response.Code, response);
                
            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("login/")]
        [AllowAnonymous]
        public ActionResult<HttpResponse> Login([FromBody] LoginUsuarioRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                MUsuario usuario = _context.Usuario.FirstOrDefault(user => user.Email.Equals(dados.Email));

                if (usuario == null) {
                    response.Message = "Usuario não encontrado";
                    return StatusCode(response.Code, response);
                }

                if (!PasswordService.VerifyPassword(dados.Senha, usuario.Senha)) {
                    response.Message = "Senhas não conferem";
                    return StatusCode(response.Code, response);
                }

                string token = TokenService.GenerateToken(usuario);

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Logado com sucesso";
                response.Response = new {
                    email = usuario.Email,
                    token = token
                };

                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("consultar/{codigo}")]
        [Authorize]
        public ActionResult<HttpResponse> Consultar(int codigo) {
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

                ConsultarUsuarioResponse dtoUsuario = _mapper.Map<ConsultarUsuarioResponse>(usuario);

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
    }
}
