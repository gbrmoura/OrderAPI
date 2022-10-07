using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderAPI.Data;
using AutoMapper;
using OrderAPI.API.Services;
using OrderAPI.Domain.Http;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Controllers
{
    [Route("api/Autenticacao/[controller]/")]
    public class UsuarioController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;
        private PasswordService _password;

        public UsuarioController(OrderAPIContext context, IMapper mapper, ModelService model, PasswordService password)
        {
            _context = context;
            _mapper = mapper;
            _model = model;
            _password = password;
        }

        [HttpPost("Registrar/")]
        [AllowAnonymous]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarUsuarioRequest body)
        {
            DefaultResponse http = new DefaultResponse()
            {
                Code = StatusCodes.Status403Forbidden,
                Message = "Rota não autorizada!"
            };

            if (!ModelState.IsValid)
            {
                http.Message = "Parametros Ausentes!";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                var usuario = _context.Usuario
                    .Where(e => e.Email == body.Email)
                    .SingleOrDefault();

                var funcionario = _context.Funcionario
                    .Where(e => e.Email == body.Email)
                    .SingleOrDefault();

                if (usuario != null || funcionario != null)
                {
                    http.Message = "Email ja cadastrado!";
                    return StatusCode(http.Code, http);
                }

                UsuarioModel usuarioDB = _mapper.Map<UsuarioModel>(body);
                usuarioDB.Senha = _password.EncryptPassword(usuarioDB.Senha);
                usuarioDB.Token = Guid.NewGuid();

                _context.Usuario.Add(usuarioDB);
                _context.SaveChanges();

                http.Code = StatusCodes.Status201Created;
                http.Message = "Usuario cadastrado com sucesso!";
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