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
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class AutenticacaoController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private TokenService _token;
        private ModelService _model;
        private PasswordService _password;

        public AutenticacaoController(OrderAPIContext context, IMapper mapper, TokenService token, PasswordService password, ModelService model)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
            _model = model;
            _password = password;
        }

        [HttpPost("PrimeiroRegistro/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> PrimeiroRegistro([FromBody] CriarFuncionarioMasterRequest body)
        {
            DefaultResponse http = new DefaultResponse()
            {
                Code = StatusCodes.Status403Forbidden,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid)
            {
                http.Message = "Parametros Ausentes.";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                if (_context.Funcionario.Count() > 0)
                {
                    http.Message = "Já existe usuario cadastrado.";
                    return StatusCode(http.Code, http);
                }

                FuncionarioModel funcionario = _mapper.Map<FuncionarioModel>(body);
                funcionario.Token = Guid.NewGuid();
                funcionario.Senha = _password.EncryptPassword(funcionario.Senha);
                funcionario.Previlegio = PrevilegioEnum.MASTER;
                funcionario.Status = true;

                _context.Funcionario.Add(funcionario);
                _context.SaveChanges();

                http.Code = StatusCodes.Status201Created;
                http.Message = "Funcionario cadastrado com sucesso.";
                return StatusCode(http.Code, http);

            }
            catch (Exception E)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor!";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpPost("Login/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> Login([FromBody] LoginUsuarioRequest body)
        {
            DefaultResponse http = new DefaultResponse()
            {
                Code = StatusCodes.Status403Forbidden,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid)
            {
                http.Message = "Parametros Ausentes.";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                UsuarioModel usuario = _context.Usuario
                    .Where((e) => e.Email.Equals(body.Login))
                    .SingleOrDefault();

                if (usuario != null) 
                {
                    if (!_password.VerifyPassword(body.Senha, usuario.Senha))
                    {
                        http.Message = "Senhas não conferem.";
                        return StatusCode(http.Code, http);
                    }
                    
                    var userRefreshToken = _token.GenerateRefreshToken();
                    var userToken = _token.GenerateToken(new List<Claim>()
                    {
                        new Claim("codigo", usuario.Codigo.ToString()),
                        new Claim("nome", usuario.Nome),
                        new Claim("login", usuario.Email),
                        new Claim("token", usuario.Token.ToString()),
                        new Claim(ClaimTypes.Role, "USUARIO"),
                    });
                    
                    _token.DeleteRefreshToken(usuario.Token);
                    _token.SaveRefreshToken(usuario.Token, userRefreshToken, userToken);
                    _context.SaveChanges();

                    http.Code = StatusCodes.Status200OK;
                    http.Message = "Logado com sucesso.";
                    http.Response = new {
                        Codigo = usuario.Codigo,
                        Nome = usuario.Nome,
                        Sobrenome = usuario.Sobrenome,
                        Prontuario = usuario.Prontuario,
                        Email = usuario.Email,
                        Token = userToken,
                        RefreshToken = userRefreshToken
                    };
                    return StatusCode(http.Code, http);
                } 

                FuncionarioModel funcionario = _context.Funcionario
                    .Where((e) => e.Login.Equals(body.Login))
                    .SingleOrDefault();
            
                if (funcionario != null)
                {
                    if (!_password.VerifyPassword(body.Senha, funcionario.Senha))
                    {
                        http.Message = "Senhas não conferem.";
                        return StatusCode(http.Code, http);
                    }

                    var refreshToken = _token.GenerateRefreshToken();
                    var token = _token.GenerateToken(new List<Claim>()
                    {
                        new Claim("codigo", funcionario.Codigo.ToString()),
                        new Claim("nome", funcionario.Nome),
                        new Claim("login", funcionario.Login),
                        new Claim("token", funcionario.Token.ToString()),
                        new Claim(ClaimTypes.Role, funcionario.Previlegio.ToString())
                    });

                    
                    _token.DeleteRefreshToken(funcionario.Token);
                    _token.SaveRefreshToken(funcionario.Token, refreshToken, token);
                    _context.SaveChanges();

                    http.Code = StatusCodes.Status200OK;
                    http.Message = "Logado com sucesso.";
                    http.Response = new {
                        Codigo = funcionario.Codigo,
                        Nome = funcionario.Nome,
                        Login = funcionario.Login,
                        Previlegio = funcionario.Previlegio,
                        Token = token,
                        RefreshToken = refreshToken 
                    };

                    return StatusCode(http.Code, http);
                }

                http.Code = StatusCodes.Status404NotFound;
                http.Message = "Usuario/Funcionario Não encontrado.";
                return StatusCode(http.Code, http);
            }
            catch (Exception E)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor!";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

        [HttpPost("AtualizarToken/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> AtualizarToken([FromBody] RefreshTokenRequest body)
        {
            DefaultResponse http = new DefaultResponse()
            {
                Code = StatusCodes.Status403Forbidden,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid)
            {
                http.Message = "Parametros Ausentes.";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                var claims = _token.GetPrincipalFromExpiredToken(body.Token).Claims;
                var claim = claims.FirstOrDefault((x) => x.Type == "token");
                var value = Guid.Parse(claim.Value);

                var savedRefreshToken = _token.GetRefreshToken(value);
                if (savedRefreshToken != body.RefreshToken)
                {
                    _token.DeleteRefreshToken(value);
                    http.Message = "Refresh Token Invalido.";
                    return StatusCode(http.Code, http);
                }

                var newJwtToken = _token.GenerateToken(claims);
                var newRefreshToken = _token.GenerateRefreshToken();

                _token.DeleteRefreshToken(value);
                _token.SaveRefreshToken(value, newRefreshToken, newJwtToken);

                http.Code = StatusCodes.Status200OK;
                http.Message = "Token Atualizado,";
                http.Response = new {
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken
                };

                return StatusCode(http.Code, http);
            }
            catch (Exception E)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor!";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }

    }
}