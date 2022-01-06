using AutoMapper;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.Data.Models;

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