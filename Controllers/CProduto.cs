using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Database;
using OrderAPI.Enums;
using OrderAPI.Models;
using OrderAPI.Services;
using OrderAPI.Data.Request;
using OrderAPI.Data.Response;
namespace orderapi.Controllers {
    
    [Route("api/produto/")]
    public class CProduto : ControllerBase {
        
        private DBContext _context;

        private IMapper _mapper;
        public CProduto(DBContext context, IMapper mapper) {
            this._context = context;
            this._mapper = mapper;
        }

    }
}