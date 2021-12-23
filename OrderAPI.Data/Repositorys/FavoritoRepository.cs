using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys.Interfaces
{
    public class FavoritoRepository
    {
        private readonly OrderAPIContext _context;

        public FavoritoRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<FavoritoModel> Insert(FavoritoModel favorito)
        {
            _context.Favorito.Add(favorito);
            await _context.SaveChangesAsync();
            return favorito;
        }

        public async Task<FavoritoModel> Update(FavoritoModel favorito)
        {
            _context.Favorito.Update(favorito);
            await _context.SaveChangesAsync();
            return favorito;
        }

        public async Task Delete(FavoritoModel favorito)
        {
            favorito.Status = false;
            _context.Favorito.Update(favorito);
            await _context.SaveChangesAsync(); 
        }

        public async Task<FavoritoModel> Get(int id)
        {
            return await _context.Favorito
                .Where(c => c.Codigo == id && c.Status == true)                
                .SingleOrDefaultAsync();
        }

        public async Task<List<FavoritoModel>> GetAll()
        {
            return await _context.Favorito
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<FavoritoModel>> Search(string search)
        {
            return await _context.Favorito
                .Where(c => c.Codigo.ToString().Contains(search))
                .Where(c => c.Status == true)
                .ToListAsync();
        }
    }
}