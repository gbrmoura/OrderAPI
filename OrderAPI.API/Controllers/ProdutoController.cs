using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.API.HTTP;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.API.Services;
using OrderAPI.Data;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Controllers
{
    [Route("api/[controller]/")]
    public class ProdutoController : ControllerBase
    {
        
        private OrderAPIContext _context;

        private IMapper _mapper;

        private TokenService _tokenService;

        public ProdutoController(OrderAPIContext context, IMapper mapper, TokenService jwtService)
        {   
            _context = context;
            _mapper = mapper;
            _tokenService = jwtService;
        }

        [HttpPost("Registrar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Registrar([FromBody] CriarProdutoRequest body) 
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
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
                
                if (_context.Produto.Any(x => x.Titulo.Equals(body.Titulo) && x.Status == true)) 
                {
                    response.Message = "Produto ja cadastrado.";
                    return StatusCode(response.Code, response);
                }

                var categoria = _context.Categoria.FirstOrDefault((e) => e.Codigo == body.CategoriaCodigo);

                if (categoria == null) 
                {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                if (!ImageService.ValidarBase64(body.Imagem)) 
                {   
                    response.Message = "Imagem é invalida.";
                    response.Error = "Imagem nao contem cabeçalho base64, ou esta vazia.";
                    return StatusCode(response.Code, response);
                }

                MProduto produto = new MProduto()
                {
                    Titulo = body.Titulo,
                    Valor = body.Valor,
                    Descricao = body.Descricao,
                    Categoria = categoria,
                };

                var imageName = Guid.NewGuid().ToString() + ".png";
                var path = ImageService.SaveImage(body.Imagem, imageName);
                MImage image = new MImage() 
                {
                    Produto = produto,
                    Nome = imageName,
                    Caminho = path,
                };

                _context.Image.Add(image);
                _context.SaveChanges();

                response.Code = StatusCodes.Status201Created;
                response.Message = "Produto cadastrado com sucesso.";
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

        [HttpPost("Alterar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Alterar([FromBody] AlterarProdutoRequest body) // TODO: alterar imagem
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
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
                var produto = _context.Produto
                    .Include(x => x.Imagem)
                    .Where((e) => e.Codigo == body.Codigo)
                    .Where((e) => e.Status == true)
                    .SingleOrDefault();
                
                if (produto == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = "Produto não encontrado.";
                    return StatusCode(response.Code, response);
                }

                var categoria = _context.Categoria.SingleOrDefault((e) => e.Codigo == body.CategoriaCodigo);

                if (categoria == null) 
                {
                    response.Message = "Categoria não encontrada.";
                    return StatusCode(response.Code, response);
                }

                produto.Titulo = body.Titulo;
                produto.Descricao = body.Descricao;
                produto.Valor = body.Valor;
                produto.Categoria = categoria;

                if (ImageService.ValidarBase64(body.Imagem))
                {   
                    var imagem = _context.Image.SingleOrDefault(e => e.Codigo == produto.Imagem.Codigo);
                    var caminho = ImageService.SaveImage(body.Imagem, imagem.Nome);

                    imagem.Caminho = caminho;
                    imagem.Produto = produto;
                }

                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto alterado com sucesso.";
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

        [HttpGet("Deletar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO")]
        public ActionResult<DefaultResponse> Deletar([FromQuery] int codigo)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            try 
            {
                var produto = _context.Produto.SingleOrDefault((e) => e.Codigo == codigo);

                if (produto == null) 
                {
                    response.Message = "Produto não encontrada.";
                    return StatusCode(response.Code, response);
                }

                produto.Status = false;
                _context.SaveChanges();

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto deletado com sucesso.";
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

        [HttpGet("Consultar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Consultar([FromQuery] int codigo)
        {
            DefaultResponse response = new DefaultResponse() 
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };  

            try 
            {
                var produto = _context.Produto.SingleOrDefault((e) => e.Codigo == codigo);

                if (produto == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = $"Produto de codigo {codigo} não encontrado.";
                    return StatusCode(response.Code, response);
                }

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produto encontrado.";
                response.Response = _mapper.Map<ConsultarProdutoResponse>(produto);
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

        [HttpGet("Listar/")]
        [Authorize(Roles = "MASTER, GERENTE, FUNCIONARIO, USUARIO")]
        public ActionResult<DefaultResponse> Listar([FromQuery] ListarRequest query) 
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
                IQueryable<MProduto> sql = _context.Produto;
                if (!String.IsNullOrEmpty(query.CampoPesquisa))
                {
                    sql = sql.Where((e) =>
                        e.Codigo.ToString().Contains(query.CampoPesquisa) ||
                        e.Titulo.Contains(query.CampoPesquisa) ||
                        e.Descricao.Contains(query.CampoPesquisa)
                    );
                }

                var produtos = sql
                    .Where((e) => e.Status == true)
                    .Include((e) => e.Categoria)
                    .Skip((query.NumeroPagina - 1) * query.TamanhoPagina)
                    .Take(query.TamanhoPagina)
                    .OrderBy((e) => e.Codigo)
                    .ToList();



                ListarResponse list = new ListarResponse 
                {
                    NumeroRegistros = _context.Produto.Where(e => e.Status == true).Count(),
                    Dados = _mapper.Map<List<ConsultarProdutoResponse>>(produtos)
                };

                response.Code = StatusCodes.Status200OK;
                response.Message = "Produtos encontrado(s).";
                response.Response = list;
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

        [HttpGet("Imagem/")]
        [AllowAnonymous]
        public IActionResult Imagem([FromQuery] ImagemRequest query) 
        {
            DefaultResponse response = new  DefaultResponse()
            {
                Code = StatusCodes.Status401Unauthorized,
                Message = "Rota não autorizada."
            };

            if (!_tokenService.ValidateRefreshToken(query.RefreshToken, query.Token))
            {
                return StatusCode(response.Code);
            }

            try
            {
                var image = _context.Image
                    .Include(e => e.Produto)
                    .Where(e => e.ProductCodigo == query.Codigo)
                    .SingleOrDefault();

                if (image == null) 
                {
                    response.Code = StatusCodes.Status404NotFound;
                    response.Message = $"Imagem não encontrada.";
                    return StatusCode(response.Code, response);
                }

                var img = System.IO.File.OpenRead(image.Caminho);
                return File(img, "image/png");
            }
            catch (Exception err)
            {
                response.Code = StatusCodes.Status500InternalServerError;
                response.Message = err.Message;
                return StatusCode(response.Code, response);
            }
        }
    }
}