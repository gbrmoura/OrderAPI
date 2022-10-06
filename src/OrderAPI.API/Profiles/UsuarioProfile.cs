using AutoMapper;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Profiles {

    public class UsuarioProfile : Profile {

        public UsuarioProfile() {
            CreateMap<CriarUsuarioRequest, UsuarioModel>();
        }
    
    }
}
