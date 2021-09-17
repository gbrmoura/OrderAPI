using AutoMapper;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
using OrderAPI.Models;


namespace OrderAPI.Profiles {
    
    public class ProdutoProfile : Profile {

        public ProdutoProfile() {
            CreateMap<CriarProdutoRequest, MProduto>();   
        }

    }
}