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
    public class FuncionarioController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;
        private PasswordService _password;

        public FuncionarioController(OrderAPIContext context, IMapper mapper, ModelService model, PasswordService password)
        {
            _context = context;
            _mapper = mapper;
            _model = model;
            _password = password;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarFuncionarioRequest body)
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
                if (_context.Funcionario.Any(e => e.Login.Equals(body.Login) && e.Status == true))
                {
                    http.Message = "Funcionario já cadastrado.";
                    return StatusCode(http.Code, http);
                }

                var funcionario = _mapper.Map<FuncionarioModel>(body);
                funcionario.Senha = _password.EncryptPassword(funcionario.Senha);
                funcionario.Token = Guid.NewGuid();

                _context.Funcionario.Add(funcionario);
                _context.SaveChanges();

                http.Code = StatusCodes.Status201Created;
                http.Message = "Funcionario cadastrado com sucesso.";
                return StatusCode(http.Code, http);
            }
            catch (Exception E)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor.";
                http.Error = E.Message;
                return StatusCode(http.Code, http);
            }
        }
    }
}