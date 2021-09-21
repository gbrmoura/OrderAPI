using AutoMapper;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
using OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Profiles {

    public class MetodoPagtoProfile : Profile {

        public MetodoPagtoProfile() {
            CreateMap<CriarMetodoPagtoRequest, MMetodoPagamento>();
            CreateMap<AlterarMetodoPagtoRequest, MMetodoPagamento>();
            CreateMap<MMetodoPagamento, ConsultarMetodoPagtoResponse>();
        }
    }
}
