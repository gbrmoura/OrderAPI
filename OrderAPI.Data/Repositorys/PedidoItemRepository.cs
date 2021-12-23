using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class PedidoItemRepository
    {
        private readonly OrderAPIContext _context;

        public PedidoItemRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<PedidoItemModel> Insert(PedidoItemModel item)
        {
            _context.PedidoItem.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<PedidoItemModel> Update(PedidoItemModel item)
        {
            _context.PedidoItem.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task Delete(PedidoItemModel item)
        {
            item.Status = false;
            _context.PedidoItem.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<PedidoItemModel> Get(int id)
        {
            return await _context.PedidoItem
                .Where(c => c.Codigo == id && c.Status == true)
                .SingleOrDefaultAsync();
        }

        public async Task<List<PedidoItemModel>> GetAll()
        {
            return await _context.PedidoItem
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<PedidoItemModel>> Search(string search)
        {
            return await _context.PedidoItem
                .Where(c => c.Codigo.ToString().Contains(search) ||
                            c.Valor.ToString().Contains(search) || 
                            c.Quantidade.ToString().Contains(search))
                .Where(c => c.Status == true)
                .ToListAsync();
        }
    }
}