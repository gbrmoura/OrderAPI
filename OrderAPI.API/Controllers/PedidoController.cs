using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Data;

namespace OrderAPI.API.Controllers
{
    public class PedidoController : ControllerBase
    {
        private OrderAPIContext _context;

        private IMapper _mapper;

        public PedidoController(OrderAPIContext context, IMapper mapper)
        {   
            _context = context;
            _mapper = mapper;
        }
    }
}