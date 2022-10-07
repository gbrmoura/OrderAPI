using AutoMapper;
using OrderAPI.Domain.Http.Request;
using OrderAPI.Domain.Http.Response;
using OrderAPI.Domain.Models;

namespace OrderAPI.API.Profiles {

    public class MetodoPagtoProfile : Profile {

        public MetodoPagtoProfile() {
            CreateMap<CriarMetodoPagtoRequest, MetodoPagamentoModel>();
            CreateMap<AlterarMetodoPagtoRequest, MetodoPagamentoModel>();
            CreateMap<MetodoPagamentoModel, ConsultarMetodoPagtoResponse>();
        }
    }
}
