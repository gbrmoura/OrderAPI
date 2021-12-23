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
        private OrderAPIContext context;
        private IMapper mapper;
        private TokenService token;
        private ModelService model;
        private PasswordService password;

        public AutenticacaoController(OrderAPIContext context, IMapper mapper, TokenService token, PasswordService password, ModelService model)
        {
            this.context = context;
            this.mapper = mapper;
            this.token = token;
            this.model = model;
            this.password = password;
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                if (this.context.Funcionario.Count() > 0)
                {
                    http.Message = "Já existe usuario cadastrado.";
                    return StatusCode(http.Code, http);
                }

                FuncionarioModel funcionario = this.mapper.Map<FuncionarioModel>(body);
                funcionario.Token = Guid.NewGuid();
                funcionario.Senha = this.password.EncryptPassword(funcionario.Senha);
                funcionario.Previlegio = PrevilegioEnum.MASTER;
                funcionario.Status = true;

                this.context.Funcionario.Add(funcionario);
                this.context.SaveChanges();

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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                UsuarioModel usuario = this.context.Usuario
                    .Where((e) => e.Email.Equals(body.Login))
                    .SingleOrDefault();

                if (usuario != null) 
                {
                    if (!this.password.VerifyPassword(body.Senha, usuario.Senha))
                    {
                        http.Message = "Senhas não conferem.";
                        return StatusCode(http.Code, http);
                    }
                    
                    var userRefreshToken = this.token.GenerateRefreshToken();
                    var userToken = this.token.GenerateToken(new List<Claim>()
                    {
                        new Claim("codigo", usuario.Codigo.ToString()),
                        new Claim("nome", usuario.Nome),
                        new Claim("login", usuario.Email),
                        new Claim("token", usuario.Token.ToString()),
                        new Claim(ClaimTypes.Role, "USUARIO"),
                    });
                    
                    this.token.DeleteRefreshToken(usuario.Token);
                    this.token.SaveRefreshToken(usuario.Token, userRefreshToken, userToken);
                    this.context.SaveChanges();

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

                FuncionarioModel funcionario = this.context.Funcionario
                    .Where((e) => e.Login.Equals(body.Login))
                    .SingleOrDefault();
            
                if (funcionario != null)
                {
                    if (!this.password.VerifyPassword(body.Senha, funcionario.Senha))
                    {
                        http.Message = "Senhas não conferem.";
                        return StatusCode(http.Code, http);
                    }

                    var refreshToken = this.token.GenerateRefreshToken();
                    var token = this.token.GenerateToken(new List<Claim>()
                    {
                        new Claim("codigo", funcionario.Codigo.ToString()),
                        new Claim("nome", funcionario.Nome),
                        new Claim("login", funcionario.Login),
                        new Claim("token", funcionario.Token.ToString()),
                        new Claim(ClaimTypes.Role, funcionario.Previlegio.ToString())
                    });

                    
                    this.token.DeleteRefreshToken(funcionario.Token);
                    this.token.SaveRefreshToken(funcionario.Token, refreshToken, token);
                    this.context.SaveChanges();

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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                var claims = this.token.GetPrincipalFromExpiredToken(body.Token).Claims;
                var claim = claims.FirstOrDefault((x) => x.Type == "token");
                var value = Guid.Parse(claim.Value);

                var savedRefreshToken = this.token.GetRefreshToken(value);
                if (savedRefreshToken != body.RefreshToken)
                {
                    this.token.DeleteRefreshToken(value);
                    http.Message = "Refresh Token Invalido.";
                    return StatusCode(http.Code, http);
                }

                var newJwtToken = this.token.GenerateToken(claims);
                var newRefreshToken = this.token.GenerateRefreshToken();

                this.token.DeleteRefreshToken(value);
                this.token.SaveRefreshToken(value, newRefreshToken, newJwtToken);

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