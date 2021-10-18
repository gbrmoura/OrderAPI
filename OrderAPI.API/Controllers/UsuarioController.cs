using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Data;
using AutoMapper;
using OrderAPI.API.Services;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.Data.Models;
using OrderAPI.Data.Helpers;
using OrderAPI.API.Helpers;
using Microsoft.Extensions.Logging;

namespace OrderAPI.API.Controllers
{
    [Route("api/Autenticacao/[controller]/")]
    public class UsuarioController : ControllerBase
    {

        private OrderAPIContext _context;

        private IMapper _mapper;

        private TokenService _jwtService;

        private ILogger<AutenticacaoController> _logger;

        public UsuarioController(OrderAPIContext context, IMapper mapper, TokenService jwtService, ILogger<AutenticacaoController> logger)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("Registrar/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarUsuarioRequest body)
        {
            DefaultResponse response = new DefaultResponse()
            {
                Code = StatusCodes.Status403Forbidden,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid)
            {
                response.Message = "Parametros Ausentes!";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                MUsuario usuario = _context.Usuario
                    .FirstOrDefault(user => user.Email.Equals(body.Email));

                if (usuario != null)
                {
                    response.Message = "Email ja cadastrado!";
                    return StatusCode(response.Code, response);
                }

                MUsuario usuarioDB = _mapper.Map<MUsuario>(body);
                usuarioDB.Senha = PasswordService.EncryptPassword(usuarioDB.Senha);

                _context.Usuario.Add(usuarioDB);
                _context.SaveChanges();

                response.Code = StatusCodes.Status201Created;
                response.Message = "Usuario cadastrado com sucesso!";
                return StatusCode(response.Code, response);

            }
            catch (Exception E)
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro interno do servidor!";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost("Login/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> Login([FromBody] LoginUsuarioRequest body)
        {
            DefaultResponse response = new DefaultResponse()
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };

            if (!ModelState.IsValid)
            {
                response.Message = "Parametros Ausentes";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                MUsuario usuario = _context.Usuario.FirstOrDefault(e => e.Email.Equals(body.Login));

                if (usuario == null)
                {
                    response.Message = "Usuario não encontrado.";
                    return StatusCode(response.Code, response);
                }

                if (!PasswordService.VerifyPassword(body.Senha, usuario.Senha))
                {
                    response.Message = "Senhas não conferem.";
                    return StatusCode(response.Code, response);
                }

                usuario.Token = _jwtService.GenerateToken(usuario);
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Logado com sucesso.";
                response.Response = new
                {
                    Codigo = usuario.Codigo,
                    Nome = usuario.Nome,
                    Sobrenome = usuario.Sobrenome,
                    Prontuario = usuario.Prontuario,
                    Email = usuario.Email,
                    Token = usuario.Token
                };

                return StatusCode(response.Code, response);
            }
            catch (Exception E)
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = "Erro interno do servidor";
                response.Error = E.Message;
                return StatusCode(response.Code, response);
            }
        }    

        // [HttpPost("Logout/")]
        // [Authorize]
        // public ActionResult<DefaultResponse> Logout()
        // {
        //     return NotFound(); // TODO: Fazer metodo de logout
        // }
    }
}