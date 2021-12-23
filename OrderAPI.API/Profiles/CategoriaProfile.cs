using AutoMapper;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Profiles {

    public class CategoriaProfile : Profile {

        public CategoriaProfile() {
            CreateMap<CriarCategoriaRequest, CategoriaModel>();
            CreateMap<AlterarCategoriaRequest, CategoriaModel>();
            CreateMap<CategoriaModel, ConsultarCategoriaResponse>();
            CreateMap<CategoriaModel, ConsultarCardapioCategoriaResponse>();
        }
    }
}