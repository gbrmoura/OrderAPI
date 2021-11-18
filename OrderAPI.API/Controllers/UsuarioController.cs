using System;
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
namespace OrderAPI.API.Controllers
{
    [Route("api/Autenticacao/[controller]/")]
    public class UsuarioController : ControllerBase
    {
        private OrderAPIContext context;
        private IMapper mapper;
        private ModelService model;
        private PasswordService password;

        public UsuarioController(OrderAPIContext context, IMapper mapper, ModelService model, PasswordService password)
        {
            this.context = context;
            this.mapper = mapper;
            this.model = model;
            this.password = password;
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                MUsuario usuario = this.context.Usuario
                    .Where(e => e.Email == body.Email)
                    .SingleOrDefault();

                if (usuario != null)
                {
                    http.Message = "Email ja cadastrado!";
                    return StatusCode(http.Code, http);
                }

                MUsuario usuarioDB = this.mapper.Map<MUsuario>(body);
                usuarioDB.Senha = this.password.EncryptPassword(usuarioDB.Senha);
                usuarioDB.Token = Guid.NewGuid();

                this.context.Usuario.Add(usuarioDB);
                this.context.SaveChanges();

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