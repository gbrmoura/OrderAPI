using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OrderAPI.API.HTTP.Response;
using OrderAPI.Data.Models;

namespace OrderAPI.API.Profiles
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile() {
            CreateMap<MPedido, ConsultarPedidoSimplesResponse>();
            CreateMap<MPedido, ConsultarPedidoResponse>();
        }
    }
}