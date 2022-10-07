using AutoMapper;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.Domain.Models;

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