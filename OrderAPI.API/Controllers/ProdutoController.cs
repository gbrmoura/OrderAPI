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
        private OrderAPIContext context;
        private IMapper mapper;
        private ModelService model;
        private FileService file;
        private TokenService token;

        public ProdutoController(OrderAPIContext context, IMapper mapper, ModelService model, FileService file, TokenService token)
        {   
            this.context = context;
            this.mapper = mapper;
            this.model = model;
            this.file = file;
            this.token = token;
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            if (!this.file.IsValidBase64(body.Imagem)) 
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
                if (this.context.Produto.Any(x => x.Titulo.Equals(body.Titulo) && x.Status == true)) 
                {
                    http.Message = "Produto ja cadastrado.";
                    return StatusCode(http.Code, http);
                }

                var categoria = this.context.Categoria.FirstOrDefault((e) => e.Codigo == body.CategoriaCodigo);
                if (categoria == null) 
                {
                    http.Message = "Categoria não encontrada.";
                    return StatusCode(http.Code, http);
                }

                MProduto produto = new MProduto()
                {
                    Titulo = body.Titulo,
                    Valor = body.Valor,
                    Descricao = body.Descricao,
                    Categoria = categoria,
                };

                var name = Guid.NewGuid().ToString();
                var path = this.file.SaveFile(body.Imagem, name, "png");
                MImage image = new MImage() 
                {
                    Produto = produto,
                    Nome = name,
                    Extensao = "png",
                    Caminho = path,
                };

                this.context.Image.Add(image);
                this.context.SaveChanges();

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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try
            {
                var produto = this.context.Produto
                    .Include(x => x.Imagem)
                    .Where((e) => e.Codigo == body.Codigo)
                    .Where((e) => e.Status == true)
                    .SingleOrDefault();
                
                if (produto == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = "Produto não encontrado.";
                    return StatusCode(http.Code, http);
                }

                var categoria = this.context.Categoria
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
                
                if (this.file.IsValidBase64(body.Imagem))
                {   
                    var imagem = this.context.Image.SingleOrDefault(e => e.Codigo == produto.Imagem.Codigo);
                    var caminho = this.file.SaveFile(body.Imagem, imagem.Nome, imagem.Extensao);

                    imagem.Caminho = caminho;
                    imagem.Produto = produto;
                }

                this.context.SaveChanges();

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
                var produto = this.context.Produto
                    .Where((e) => e.Codigo == codigo)
                    .SingleOrDefault();

                if (produto == null) 
                {
                    http.Message = "Produto não encontrada.";
                    return StatusCode(http.Code, http);
                }

                produto.Status = false;
                this.context.SaveChanges();

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
                var produto = this.context.Produto
                    .Include((e) => e.Categoria)
                    .Where((e) => e.Codigo == codigo)
                    .SingleOrDefault();

                if (produto == null) 
                {
                    http.Code = StatusCodes.Status404NotFound;
                    http.Message = $"Produto de codigo {codigo} não encontrado.";
                    return StatusCode(http.Code, http);
                }

                http.Code = StatusCodes.Status200OK;
                http.Message = "Produto encontrado.";
                http.Response = this.mapper.Map<ConsultarProdutoResponse>(produto);
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            try 
            {
                IQueryable<MProduto> sql = this.context.Produto;
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
                    NumeroRegistros = this.context.Produto.Where(e => e.Status == true).Count(),
                    Dados = this.mapper.Map<List<ConsultarProdutoResponse>>(produtos)
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
                http.Error = this.model.ErrorConverter(ModelState);
                return StatusCode(http.Code, http);
            }

            if (!this.token.IsValidRefreshToken(query.RefreshToken, query.Token))
                return StatusCode(http.Code);

            try
            {
                var image = this.context.Image
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