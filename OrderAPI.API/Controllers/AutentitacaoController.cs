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
    [Route("api/[controller]/")]
    public class AutenticacaoController : ControllerBase
    {

        private OrderAPIContext _context;

        private IMapper _mapper;

        private TokenService _jwtService;

        private ILogger<AutenticacaoController> _logger;

        public AutenticacaoController(OrderAPIContext context, IMapper mapper, TokenService jwtService, ILogger<AutenticacaoController> logger)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("PrimeiroRegistro/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> PrimeiroRegistro([FromBody] CriarFuncionarioMasterRequest body)
        {
            DefaultResponse response = new DefaultResponse()
            {
                Code = StatusCodes.Status403Forbidden,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid)
            {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                List<MFuncionario> funcionarios = _context.Funcionario
                    .Where((element) => element.Status == true)
                    .ToList();

                if (funcionarios.Count > 0)
                {
                    response.Message = "Já existe usuario cadastrado.";
                    return StatusCode(response.Code, response);
                }

                MFuncionario dbFuncionario = _mapper.Map<MFuncionario>(body);
                dbFuncionario.Senha = PasswordService.EncryptPassword(dbFuncionario.Senha);
                dbFuncionario.Previlegio = PrevilegioEnum.MASTER;
                dbFuncionario.Status = true;

                _context.Funcionario.Add(dbFuncionario);
                _context.SaveChanges();

                response.Code = StatusCodes.Status201Created;
                response.Message = "Funcionario cadastrado com sucesso.";
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

        [HttpPost("Funcionario/Registrar/")]
        [Authorize(Roles = "MASTER")]
        public ActionResult<DefaultResponse> RegistrarFuncionario([FromBody] CriarFuncionarioRequest body)
        {
            DefaultResponse response = new DefaultResponse()
            {
                Code = StatusCodes.Status403Forbidden,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid)
            {
                response.Message = "Parametros Ausentes.";
                response.Error = ModelStateService.ErrorConverter(ModelState);
                return StatusCode(response.Code, response);
            }

            try
            {
                MFuncionario value = _context.Funcionario
                    .FirstOrDefault((element) => element.Login.Equals(body.Login) && element.Status == true);

                if (value != null)
                {
                    response.Message = "Funcionario já cadastrado.";
                    return StatusCode(response.Code, response);
                }

                MFuncionario funcionario = _mapper.Map<MFuncionario>(body);
                funcionario.Senha = PasswordService.EncryptPassword(funcionario.Senha);

                _context.Funcionario.Add(funcionario);
                _context.SaveChanges();

                response.Code = StatusCodes.Status201Created;
                response.Message = "Funcionario cadastrado com sucesso.";
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

        [HttpPost("Usuario/Registrar/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> RegistrarUsuario([FromBody] CriarUsuarioRequest body)
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

        [HttpPost("Usuario/Login/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> UsuarioLogin([FromBody] LoginUsuarioRequest body)
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

        [HttpPost("Funcionario/Login/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> FuncionarioLogin([FromBody] LoginUsuarioRequest body)
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
                MFuncionario funcionario = _context.Funcionario.FirstOrDefault(e => e.Login.Equals(body.Login));

                if (funcionario == null)
                {
                    response.Message = "Funcionario não encontrado.";
                    return StatusCode(response.Code, response);
                }

                if (!PasswordService.VerifyPassword(body.Senha, funcionario.Senha))
                {
                    response.Message = "Senhas não conferem.";
                    return StatusCode(response.Code, response);
                }

                funcionario.Token = _jwtService.GenerateToken(funcionario);
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Logado com sucesso.";
                response.Response = new
                {
                    Codigo = funcionario.Codigo,
                    Nome = funcionario.Nome,
                    Login = funcionario.Login,
                    Token = funcionario.Token
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

        [HttpPost("Logout/")]
        [Authorize]
        public ActionResult<DefaultResponse> Logout()
        {
            
            // _logger.LogInformation($"Nome: { User.Identity.Name }");
            // _logger.LogInformation($"Type: { User.Identity.AuthenticationType }");
            // _logger.LogInformation($"Auth: { User.Identity.IsAuthenticated }");

            // foreach (var claim in User.Claims)
            // {
            //     _logger.LogInformation($"Type: { claim.Type } - Value: { claim.Value }");  
            // }

            return NotFound(); // TODO: Fazer metodo de logout
        }
    }
}