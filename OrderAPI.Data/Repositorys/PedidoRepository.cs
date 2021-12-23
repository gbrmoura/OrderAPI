using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class PedidoRepository
    {
        private readonly OrderAPIContext _context;

        public PedidoRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<PedidoModel> Insert(PedidoModel pedido)
        {
            _context.Pedido.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<PedidoModel> Update(PedidoModel pedido)
        {
            _context.Pedido.Update(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<PedidoModel> Get(int id)
        {
            return await _context.Pedido
                .Where(c => c.Codigo == id)
                .SingleOrDefaultAsync();
        }

        public async Task<List<PedidoModel>> GetAll()
        {
            return await _context.Pedido.ToListAsync();
        }

        public async Task<List<PedidoModel>> Search(string search)
        {
            return await _context.Pedido
                .Where(c => c.Codigo.ToString().Contains(search) ||
                            c.Data.ToString().Contains(search) ||
                            c.Observacao.ToString().Contains(search))
                .ToListAsync();
        }
    }
}