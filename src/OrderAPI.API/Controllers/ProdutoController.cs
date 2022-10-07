using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Domain.Http;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.API.Services;
using OrderAPI.Data;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class ProdutoController : ControllerBase
    {
        private OrderAPIContext _context;
        private IMapper _mapper;
        private ModelService _model;
        private FileService _file;
        private TokenService _token;

        public ProdutoController(OrderAPIContext context, IMapper mapper, ModelService model, FileService file, TokenService token)
        {   
            _context = context;
            _mapper = mapper;
            _model = model;
            _file = file;
            _token = token;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarProdutoRequest body) 
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes.";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            if (!_file.IsValidBase64(body.Imagem)) 
            {   
                http.Message = "Imagem é invalida.";
                http.Error = new List<ErrorResponse>()
                {
                    new ErrorResponse() { Field = "Imagem", Message = "Imagem nao contem cabeçalho base64, ou esta vazia." }
                };
                return StatusCode(http.Code, http);
            }

            try 
            {
                if (_context.Produto.Any(x => x.Titulo.Equals(body.Titulo) && x.Status == true)) 
                {
                    http.Message = "Produto ja cadastrado.";
                    return StatusCode(http.Code, http);
                }

                var categoria = _context.Categoria.FirstOrDefault((e) => e.Codigo == body.CategoriaCodigo);
                if (categoria == null) 
                {
                    http.Message = "Categoria não encontrada.";
                    return StatusCode(http.Code, http);
                }

                ProdutoModel produto = new ProdutoModel()
                {
                    Titulo = body.Titulo,
                    Valor = body.Valor,
                    Descricao = body.Descricao,
                    Categoria = categoria,
                };

                var name = Guid.NewGuid().ToString();
                var path = _file.SaveFile(body.Imagem, name, "png");
                ImageModel image = new ImageModel() 
                {
                    Produto = produto,
                    Nome = name,
                    Extensao = "png",
                    Caminho = path,
                };

                _context.Image.Add(image);
                _context.SaveChanges();

                http.Code = StatusCodes.Status201Created;
                http.Message = "Produto cadastrado com sucesso.";
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

        [HttpPost("Alterar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarProdutoRequest body)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
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
                var produto = _context.Produto
                    .Include(x => x.Imagem)
                    .Where((e) => e.Codigo == body.Codigo && e.Status == true)
                    .SingleOrDefault();
                
                if (produto == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Produto não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var categoria = _context.Categoria
                    .Where((e) => e.Codigo == body.CategoriaCodigo)
                    .SingleOrDefault();

                if (categoria == null) 
                {
                    http.Message = "Categoria não encontrada.";
                    return StatusCode(http.Code, http);
                }

                produto.Titulo = body.Titulo;
                produto.Descricao = body.Descricao;
                produto.Valor = body.Valor;
                produto.Categoria = categoria;
                
                if (_file.IsValidBase64(body.Imagem))
                {   
                    var imagem = _context.Image.SingleOrDefault(e => e.Codigo == produto.Imagem.Codigo);
                    var caminho = _file.SaveFile(body.Imagem, imagem.Nome, imagem.Extensao);

                    imagem.Caminho = caminho;
                    imagem.Produto = produto;
                }

                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Produto alterado com sucesso.";
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

        [HttpGet("Deletar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Deletar([FromQuery] int codigo)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (codigo <= 0) 
            {
                http.Message = "Parametros Ausentes.";
                http.Error = new List<ErrorResponse>()
                {
                    new ErrorResponse() { Field = "Codigo", Message = "Codigo nao pode ser menor ou igual a zero." }
                };
                return StatusCode(http.Code, http);
            }

            try 
            {
                var produto = _context.Produto
                    .Where((e) => e.Codigo == codigo && e.Status == true)
                    .SingleOrDefault();

                if (produto == null) 
                {
                    http.Message = "Produto não encontrada.";
                    return StatusCode(http.Code, http);
                }

                produto.Status = false;
                _context.SaveChanges();

                http.Code = StatusCodes.Status200OK;
                http.Message = "Produto deletado com sucesso.";
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

        [HttpGet("Consultar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Consultar([FromQuery] int codigo)
        {
            DefaultResponse http = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };  

            if (codigo <= 0) 
            {
                http.Message = "Parametros Ausentes.";
                http.Error = new List<ErrorResponse>()
                {
                    new ErrorResponse() { Field = "Codigo", Message = "Codigo nao pode ser menor ou igual a zero." }
                };
                return StatusCode(http.Code, http);
            }

            try 
            {
                var produto = _context.Produto
                    .Include((e) => e.Categoria)
                    .Where((e) => e.Codigo == codigo && e.Status == true)
                    .SingleOrDefault();

                if (produto == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = $"Produto de codigo {codigo} não encontrado.";
                    return StatusCode(http.Code, http);
                }

                http.Code = StatusCodes.Status200OK;
                http.Message = "Produto encontrado.";
                http.Response = _mapper.Map<ConsultarProdutoResponse>(produto);
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
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
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
                IQueryable<ProdutoModel> sql = _context.Produto;
                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) =>
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Titulo.Contains(query.CampoPesquisa) ||
                        e.Descricao.Contains(query.CampoPesquisa) ||
                        e.Valor.ToString().Contains(query.CampoPesquisa)
                    );
                }

                var produtos = sql.Where((e) => e.Status == true)
                    .Include((e) => e.Categoria)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .ToList();

                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = _context.Produto.Where(e => e.Status == true).Count(),
                    Dados = _mapper.Map<List<ConsultarProdutoResponse>>(produtos)
                };

                http.Code = StatusCodes.Status200OK;
                http.Message = "Produtos encontrado(s).";
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

        [HttpGet("Imagem/")]
        [AllowAnonymous]
        public IActionResult Imagem([FromQuery] ImagemRequest query) 
        {
            DefaultResponse http = new  DefaultResponse()
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!ModelState.IsValid) 
            {
                http.Message = "Parametros Ausentes.";
                http.Error = _model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            if (!_token.IsValidCurrentToken(query.Token))
            {
                http.Message = "Token inválido.";
                return StatusCode(http.Code, http);
            }

            try
            {
                var image = _context.Image
                    .Include(e => e.Produto)
                    .Where(e => e.ProductCodigo == query.Codigo)
                    .SingleOrDefault();

                if (image == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = $"Imagem não encontrada.";
                    return StatusCode(http.Code, http);
                }

                if (!System.IO.File.Exists(image.Caminho)) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = $"Imagem não encontrada nos arquivos.";
                    return StatusCode(http.Code, http);
                }

                var img = System.IO.File.OpenRead(image.Caminho);
                return File(img, $"image/{ image.Extensao }");
            }
            catch (Exception err)
            {
                http.Code = StatusCodes.Status500InternalServerError;
                http.Message = "Erro interno do servidor.";
                http.Error = err.Message;
                return StatusCode(http.Code, http);
            }
        }
    }
}