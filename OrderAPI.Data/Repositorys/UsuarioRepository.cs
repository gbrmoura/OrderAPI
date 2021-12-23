using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class UsuarioRepository
    {
        private readonly OrderAPIContext _context;

        public UsuarioRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<UsuarioModel> Insert(UsuarioModel usuario)
        {
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<UsuarioModel> Update(UsuarioModel usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task Delete(UsuarioModel usuario)
        {
            usuario.Status = false;
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<UsuarioModel> Get(int id)
        {
            return await _context.Usuario
                .Where(c => c.Codigo == id && c.Status == true)
                .SingleOrDefaultAsync();
        }

        public async Task<List<UsuarioModel>> GetAll()
        {
            return await _context.Usuario
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<UsuarioModel>> Search(string search)
        {
            return await _context.Usuario
                .Where(c => c.Email.Contains(search) ||
                            c.Nome.Contains(search) ||
                            c.Sobrenome.Contains(search) ||
                            c.Prontuario.Contains(search))
                .Where(c => c.Status == true)
                .ToListAsync();
        }
    }
}