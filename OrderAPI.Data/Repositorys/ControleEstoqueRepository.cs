using System.Threading.Tasks;
using System.Linq;
using OrderAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace OrderAPI.Data.Repositorys
{
    public class ControleEstoqueRepository
    {
        private readonly OrderAPIContext _context;

        public ControleEstoqueRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<ControleEstoqueModel> Insert(ControleEstoqueModel controle)
        {
            _context.ControleEstoque.Add(controle);
            await _context.SaveChangesAsync();
            return controle;
        }

        public async Task<ControleEstoqueModel> Update(ControleEstoqueModel controle)
        {
            _context.ControleEstoque.Update(controle);
            await _context.SaveChangesAsync();
            return controle;
        }

        public async Task Delete(ControleEstoqueModel controle)
        {
            controle.Status = false;
            _context.ControleEstoque.Update(controle);
            await _context.SaveChangesAsync();
        }

        public async Task<ControleEstoqueModel> Get(int id)
        {
            return await _context.ControleEstoque
                .Where(c => c.Codigo == id && c.Status == true)                
                .SingleOrDefaultAsync();
        }

        public async Task<List<ControleEstoqueModel>> GetAll()
        {
            return await _context.ControleEstoque
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<ControleEstoqueModel>> Search(string search)
        {
            return await _context.ControleEstoque
                .Where(c => c.Codigo.ToString().Contains(search) ||
                            c.Quantidade.ToString().Contains(search) ||
                            c.Data.ToString().Contains(search) || 
                            c.Tipo.ToString().Contains(search))
                .Where(c => c.Status == true)
                .ToListAsync();
        }

    }
    
}