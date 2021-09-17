using AutoMapper;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
using OrderAPI.Models;


namespace OrderAPI.Profiles {
    public class ProdutoProfile {
        public class CategoriaProfile : Profile {

        public CategoriaProfile() {
            CreateMap<MProduto, CriarProdutoRequest>();
        }
    }
    }
}