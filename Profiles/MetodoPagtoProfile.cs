using AutoMapper;
using OrderAPI.API.HTTP.Request;
using OrderAPI.API.HTTP.Response;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Profiles {

    public class MetodoPagtoProfile : Profile {

        public MetodoPagtoProfile() {
            CreateMap<CriarMetodoPagtoRequest, MMetodoPagamento>();
            CreateMap<AlterarMetodoPagtoRequest, MMetodoPagamento>();
            CreateMap<MMetodoPagamento, ConsultarMetodoPagtoResponse>();
        }
    }
}
