﻿using AutoMapper;
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

    [Route("api/funcionario/")]
    public class CFuncionario : ControllerBase {

        private DBContext _context;
        private IMapper _mapper;
        private TokenService _jwtService;

        public CFuncionario(DBContext context, IMapper mapper, TokenService jwtService) {
            this._context = context;
            this._mapper = mapper;
            this._jwtService = jwtService;
        }

        [HttpPost("primeiroRegistro/")]
        [AllowAnonymous]
        public ActionResult<HttpResponse> PrimeiroRegistro([FromBody] CriarFuncionarioMasterRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada."
            };    

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try {
                List<MFuncionario> funcionarios = _context.Funcionario
                    .Where((element) => element.Status == true)
                    .ToList();

                if (funcionarios.Count  > 0) {
                    response.Message = "Já existe usuario cadastrado.";
                    return StatusCode(response.Code, response);
                }

                MFuncionario dbFuncionario = _mapper.Map<MFuncionario>(dados);
                dbFuncionario.Senha = PasswordService.EncryptPassword(dbFuncionario.Senha);
                dbFuncionario.Previlegio = EPrevilegio.MASTER;
                dbFuncionario.Status = true;

                _context.Funcionario.Add(dbFuncionario);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Funcionario cadastrado com sucesso.";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("registrar/")]
        [Authorize(Roles = "MASTER")]
        public ActionResult<HttpResponse> Registrar([FromBody] CriarFuncionarioRequest dados) {
            HttpResponse response = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            if (dados.Previlegio == EPrevilegio.MASTER) {
                response.Message = "Previlegio deve estar entre Gerente e Funcionario.";
                return StatusCode(response.Code, response);
            }

            try {
                MFuncionario value = _context.Funcionario
                    .FirstOrDefault((element) => element.Login.Equals(dados.Login) && element.Status == true);

                if (value != null) {
                    response.Message = "Funcionario já cadastrado.";
                    return StatusCode(response.Code, response);
                }

                MFuncionario funcionario = _mapper.Map<MFuncionario>(dados);
                funcionario.Senha = PasswordService.EncryptPassword(funcionario.Senha);

                _context.Funcionario.Add(funcionario);
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Funcionario cadastrado com sucesso.";
                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor.";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("login/")]
        [AllowAnonymous]
        public ActionResult<HttpResponse> Login([FromBody] LoginFuncionarioRequest dados) {
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
                MFuncionario funcionario = _context.Funcionario
                    .FirstOrDefault(func => func.Login.Equals(dados.Login));

                if (funcionario == null) {
                    response.Message = "Funcionario não encontrado";
                    return StatusCode(response.Code, response);
                }

                if (!PasswordService.VerifyPassword(dados.Senha, funcionario.Senha)) {
                    response.Message = "Senhas não conferem";
                    return StatusCode(response.Code, response);
                } 

                funcionario.Token = _jwtService.GenerateToken(funcionario); 
                _context.SaveChanges();

                response.Code = (int)EHttpResponse.OK;
                response.Message = "Logado com sucesso";
                response.Response = new {
                    nome = funcionario.Nome,
                    email = funcionario.Login,
                    token = funcionario.Token
                };

                return StatusCode(response.Code, response);

            } catch (Exception E) {
                response.Code = (int)EHttpResponse.INTERNAL_SERVER_ERROR;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("consultar/{codigo}")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<HttpResponse> Consultar(int codigo) {
            HttpResponse httpMessage = new HttpResponse() {
                Code = (int)EHttpResponse.UNAUTHORIZED,
                Message = "Todos usuario consultados."
            };

            try {

                MFuncionario funcionario = _context.Funcionario
                    .FirstOrDefault(user => user.Codigo == codigo);

                if (funcionario == null) {
                    httpMessage.Code = (int)EHttpResponse.NOT_FOUND;
                    httpMessage.Message = $"Funcionario de codigo {codigo}, não encontrado.";
                }

                ConsultarFuncionarioResponse dbFuncionario = _mapper.Map<ConsultarFuncionarioResponse>(funcionario);

                httpMessage.Code = (int)EHttpResponse.OK;
                httpMessage.Message = "Funcionario encontrado.";
                httpMessage.Response = dbFuncionario;
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
