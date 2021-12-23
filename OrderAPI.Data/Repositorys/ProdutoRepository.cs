using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class ProdutoRepository
    {
        private readonly OrderAPIContext _context;

        public ProdutoRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<ProdutoModel> Insert(ProdutoModel produto)
        {
            _context.Produto.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<ProdutoModel> Update(ProdutoModel produto)
        {
            _context.Produto.Update(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task Delete(ProdutoModel produto)
        {
            produto.Status = false;
            _context.Produto.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<ProdutoModel> Get(int id)
        {
            return await _context.Produto
                .Where(c => c.Codigo == id && c.Status == true)
                .SingleOrDefaultAsync();
        }

        public async Task<List<ProdutoModel>> GetAll()
        {
            return await _context.Produto
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<ProdutoModel>> Search(string search)
        {
            return await _context.Produto
                .Where(c => c.Codigo.ToString().Contains(search) ||
                            c.Descricao.Contains(search) ||
                            c.Titulo.Contains(search) ||
                            c.Valor.ToString().Contains(search))
                .Where(c => c.Status == true)
                .ToListAsync();
        }
    }
}