using AutoMapper;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
using OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Profiles {

    public class CategoriaProfile : Profile {

        public CategoriaProfile() {
            CreateMap<CriarCategoriaRequest, MCategoria>();
            CreateMap<AlterarCategoriaRequest, MCategoria>();
            CreateMap<MFuncionario, ConsultarCategoriaResponse>();
            CreateMap<List<MFuncionario>, List<ConsultarCategoriaResponse>>();
        }
    }
}