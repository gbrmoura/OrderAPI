using AutoMapper;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Profiles {

    public class UsuarioProfile : Profile {

        public UsuarioProfile() {
            CreateMap<CriarUsuarioRequest, UsuarioModel>();
        }
    
    }
}
