using AutoMapper;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Profiles {

    public class FuncionarioProfile : Profile {

        public FuncionarioProfile() {
            CreateMap<CriarFuncionarioMasterRequest, FuncionarioModel>();
            CreateMap<CriarFuncionarioRequest, FuncionarioModel>();
            CreateMap<AlterarFuncionarioRequest, FuncionarioModel>();
        }
    }
}
