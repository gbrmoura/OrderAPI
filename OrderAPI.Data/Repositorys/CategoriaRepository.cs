using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class CategoriaRepository
    {
        private readonly OrderAPIContext _context;
        public CategoriaRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<CategoriaModel> Insert(CategoriaModel categoria) 
        {
            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<CategoriaModel> Update(CategoriaModel categoria) 
        {
            _context.Categoria.Update(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task Delete(CategoriaModel categoria) 
        {
            categoria.Status = false;
            _context.Categoria.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task<CategoriaModel> Get(int id) 
        {
            return await _context.Categoria
                .Where(c => c.Codigo == id && c.Status == true)                
                .SingleOrDefaultAsync();
        }

        public async Task<List<CategoriaModel>> GetAll() 
        {
            return await _context.Categoria
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<CategoriaModel>> Search(string search) 
        {
            return await _context.Categoria
                .Where(c => c.Codigo.ToString().Contains(search) ||
                            c.Titulo.Contains(search) || 
                            c.Descricao.Contains(search))
                .Where(c => c.Status == true)
                .ToListAsync();
        }

    }
}