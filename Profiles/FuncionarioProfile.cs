﻿using AutoMapper;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
using OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Profiles {

    public class FuncionarioProfile : Profile {

        public FuncionarioProfile() {
            CreateMap<CriarFuncionarioMasterRequest, MFuncionario>();
            CreateMap<CriarFuncionarioRequest, MFuncionario>();
            CreateMap<LoginFuncionarioRequest, MFuncionario>();
            CreateMap<MFuncionario, ConsultarUsuarioResponse>();
        }
    }
}
