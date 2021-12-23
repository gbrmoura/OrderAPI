using AutoMapper;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Profiles {

    public class FuncionarioProfile : Profile {

        public FuncionarioProfile() {
            CreateMap<CriarFuncionarioMasterRequest, FuncionarioModel>();
            CreateMap<CriarFuncionarioRequest, FuncionarioModel>();
        }
    }
}
