using AutoMapper;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Profiles {
    
    public class ProdutoProfile : Profile {

        public ProdutoProfile() {
            CreateMap<CriarProdutoRequest, ProdutoModel>();   
            CreateMap<AlterarProdutoRequest, ProdutoModel>();
            CreateMap<ProdutoModel, ConsultarProdutoResponse>();
            CreateMap<ProdutoModel, ConsultarProdutoSimplesResponse>();
            CreateMap<ProdutoModel, ConsultarCardapioProdutoResponse>();
        }

    }
}