using AutoMapper;
using OrderAPI.Models;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;

namespace OrderAPI.Profiles {
    public class UsuarioProfile : Profile {
        public UsuarioProfile() {
            CreateMap<CriarUsuarioRequest, MUsuario>();
            CreateMap<LoginRequest, MUsuario>();
            CreateMap<MUsuario, ConsultarUsuarioResponse>();
        }
    
    }
}
