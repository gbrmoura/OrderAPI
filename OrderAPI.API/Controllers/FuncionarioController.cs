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
    [Route("api/Autenticacao/[controller]/")]
    public class FuncionarioController : ControllerBase
    {

        private OrderAPIContext _context;

        private IMapper _mapper;

        private TokenService _jwtService;

        public FuncionarioController(OrderAPIContext context, IMapper mapper, TokenService jwtService)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarFuncionarioRequest body)
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
    }
}