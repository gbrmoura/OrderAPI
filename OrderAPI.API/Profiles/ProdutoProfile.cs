using AutoMapper;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Profiles {
    
    public class ProdutoProfile : Profile {

        public ProdutoProfile() {
            CreateMap<CriarProdutoRequest, MProduto>();   
            CreateMap<AlterarProdutoRequest, MProduto>();
            CreateMap<MProduto, ConsultarProdutoResponse>();
            CreateMap<MProduto, ConsultarProdutoSimplesResponse>();
            CreateMap<MProduto, ConsultarCardapioProdutoResponse>();
        }

    }
}