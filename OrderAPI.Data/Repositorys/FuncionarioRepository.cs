using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class FuncionarioRepository
    {
        private readonly OrderAPIContext _context;

        public FuncionarioRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<FuncionarioModel> Insert(FuncionarioModel funcionario)
        {
            _context.Funcionario.Add(funcionario);
            await _context.SaveChangesAsync();
            return funcionario;
        }

        public async Task<FuncionarioModel> Update(FuncionarioModel funcionario)
        {
            _context.Funcionario.Update(funcionario);
            await _context.SaveChangesAsync();
            return funcionario;
        }

        public async Task Delete(FuncionarioModel funcionario)
        {
            funcionario.Status = false;
            _context.Funcionario.Update(funcionario);
            await _context.SaveChangesAsync(); 
        }

        public async Task<FuncionarioModel> Get(int id)
        {
            return await _context.Funcionario
                .Where(c => c.Codigo == id && c.Status == true)                
                .SingleOrDefaultAsync();
        }

        public async Task<List<FuncionarioModel>> GetAll()
        {
            return await _context.Funcionario
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<FuncionarioModel>> Search(string search)
        {
            return await _context.Funcionario
                .Where(c => c.Codigo.ToString().Contains(search) ||
                            c.Login.Contains(search) ||
                            c.Nome.Contains(search)) 
                .Where(c => c.Status == true)
                .ToListAsync();
        }

    }
}