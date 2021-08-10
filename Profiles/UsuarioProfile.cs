using AutoMapper;
using OrderAPI.Data.DTO;
using OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Profiles {
    public class UsuarioProfile : Profile {
        public UsuarioProfile() {
            CreateMap<DTOCriarUsuario, MUsuario>();
            CreateMap<MUsuario, DTOConsultarUsuario>();
        }
    
    }
}
