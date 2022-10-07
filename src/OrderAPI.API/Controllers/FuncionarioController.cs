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
using OrderAPI.API.EntensionMethods;

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

                var IsValidFuncionario = _context.Funcionario
                    .Where(e => e.Email == body.Email)
                    .SingleOrDefault();

                var IsValidUsuario = _context.Usuario
                    .Where(e => e.Email == body.Email)
                    .SingleOrDefault();
                
                if (IsValidFuncionario != null || IsValidUsuario != null)
                {
                    http.Message = "Email ja cadastrado.";
                    return StatusCode(http.Code, http);
                }

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

        [HttpPost("Alterar/")]
        [Authorize(Roles = "MASTER")]
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarFuncionarioRequest body)
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
                var funcionario = _context.Funcionario.FirstOrDefault(e => e.Codigo == body.Codigo);

                if (funcionario == null)
                {
                    http.Message = "Funcionario não encontrado.";
                    return StatusCode(http.Code, http);
                }

                if (!funcionario.Login.Equals(body.Login) && _context.Funcionario.Any(e => e.Login.Equals(body.Login) && e.Status == true))
                {
                    http.Message = "Nome de usuario já cadastrado.";
                    return StatusCode(http.Code, http);
                }

                
                if (funcionario.Email.Equals(body.Email))
                {
                    var IsValidFuncionario = _context.Funcionario
                    .Where(e => e.Email == body.Email)
                    .SingleOrDefault();

                    var IsValidUsuario = _context.Usuario
                        .Where(e => e.Email == body.Email)
                        .SingleOrDefault();
                    
                    if (IsValidFuncionario != null || IsValidUsuario != null)
                    {
                        http.Message = "Email ja cadastrado.";
                        return StatusCode(http.Code, http);
                    }
                }

                funcionario.Nome = body.Nome;
                funcionario.Email = body.Email;
                funcionario.Login = body.Login;
                funcionario.Previlegio = body.Previlegio;
                funcionario.Status = true;

                _context.Funcionario.Update(funcionario);
                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Funcionario alterado com sucesso.";
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

        [HttpGet("Listar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Listar([FromQuery] ListarRequest query) 
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada"
            };
            
            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                IQueryable<FuncionarioModel> sql = _context.Funcionario;
                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) => 
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Email.Contains(query.CampoPesquisa) || 
                        e.Login.Contains(query.CampoPesquisa) || 
                        e.Nome.Contains(query.CampoPesquisa));
                }

                var count = sql.Where(e => e.Status == true).Count();
                var funcionarios = sql
                    .Where(e => e.Status == true)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                var result = funcionarios.Select(e => new 
                {
                    Codigo = e.Codigo,
                    Nome = e.Nome,
                    Email = e.Email,
                    Login = e.Login,
                    Previlegio = new {
                        Codigo = e.Previlegio,
                        Descricao = e.Previlegio.ToString().ToLower().Capitalize()
                    },
                    Status = e.Status
                });

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = count, 
                    Dados = result
                };

                http.Code = StatusCodes.Status200OK;
                http.Message = "Funcionario(s) encontrado(s).";
                http.Response = list;
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