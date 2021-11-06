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
using System.Security.Claims;

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

        [HttpPost("AtualizarToken/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> AtualizarToken([FromBody] RefreshTokenRequest body)
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
                var identity = _jwtService.GetPrincipalFromExpiredToken(body.Token);
                var claims = identity.Claims;
                var claim = claims.FirstOrDefault((x) => x.Type == ClaimTypes.Actor.ToString());
                var value = Guid.Parse(claim.Value);

                var savedRefreshToken = _jwtService.GetRefreshToken(value);
                if (savedRefreshToken != body.RefreshToken)
                {
                    _jwtService.DeleteRefreshToken(value);
                    response.Message = "Refresh Token Invalido.";
                    return StatusCode(response.Code, response);
                }

                var newJwtToken = _jwtService.GenerateToken(claims);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                _jwtService.DeleteRefreshToken(value);
                _jwtService.SaveRefreshToken(value, newRefreshToken, newJwtToken);

                response.Code = StatusCodes.Status200OK;
                response.Message = "Token Atualizado,";
                response.Response = new
                {
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken
                };

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
                MUsuario usuario = _context.Usuario
                    .FirstOrDefault(e => e.Email.Equals(body.Login));

                if (usuario != null) 
                {
                    if (!PasswordService.VerifyPassword(body.Senha, usuario.Senha))
                    {
                        response.Message = "Senhas não conferem.";
                        return StatusCode(response.Code, response);
                    }

                    var userToken = _jwtService.GenerateToken(new List<Claim>()
                    {
                        new Claim(ClaimTypes.Actor, usuario.Token.ToString()),
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim(ClaimTypes.Role, "USUARIO")
                    });
                    var userRefreshToken = _jwtService.GenerateRefreshToken();

                    _jwtService.DeleteRefreshToken(usuario.Token);
                    _jwtService.SaveRefreshToken(usuario.Token, userRefreshToken, userToken);
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
                        Token = userToken,
                        RefreshToken = userRefreshToken
                    };
                    return StatusCode(response.Code, response);
                } 

                MFuncionario funcionario = _context.Funcionario
                    .FirstOrDefault(e => e.Login.Equals(body.Login));
            
                if (funcionario != null)
                {
                    if (!PasswordService.VerifyPassword(body.Senha, funcionario.Senha))
                    {
                        response.Message = "Senhas não conferem.";
                        return StatusCode(response.Code, response);
                    }

                    var token = _jwtService.GenerateToken(new List<Claim>()
                    {
                        new Claim(ClaimTypes.Actor, funcionario.Token.ToString()),
                        new Claim(ClaimTypes.Name, funcionario.Login),
                        new Claim(ClaimTypes.Role, funcionario.Previlegio.ToString())
                    });

                    var refreshToken = _jwtService.GenerateRefreshToken();
                    _jwtService.DeleteRefreshToken(funcionario.Token);
                    _jwtService.SaveRefreshToken(funcionario.Token, refreshToken, token);
                    _context.SaveChanges();

                    response.Code = StatusCodes.Status200OK;
                    response.Message = "Logado com sucesso.";
                    response.Response = new
                    {
                        Codigo = funcionario.Codigo,
                        Nome = funcionario.Nome,
                        Login = funcionario.Login,
                        Previlegio = funcionario.Previlegio,
                        Token = token,
                        RefreshToken = refreshToken 
                    };

                    return StatusCode(response.Code, response);
                }

                response.Code = StatusCodes.Status404NotFound;
                response.Message = "Usuario/Funcionario Não encontrado.";
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

    }
}