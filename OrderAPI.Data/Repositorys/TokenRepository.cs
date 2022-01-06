using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class TokenRepository
    {
        private readonly OrderAPIContext _context;

        public TokenRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<TokenModel> Insert(TokenModel token)
        {
            _context.Token.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<TokenModel> Update(TokenModel token)
        {
            _context.Token.Update(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<TokenModel> Get(int id)
        {
            return await _context.Token
                .Where(c => c.Codigo == id)
                .SingleOrDefaultAsync();
        }

        public async Task<List<TokenModel>> GetAll()
        {
            return await _context.Token.ToListAsync();
        }

        public async Task<List<TokenModel>> Search(string search)
        {
            return await _context.Token
                .Where(c => c.Token.Contains(search) ||
                            c.RefreshToken.Contains(search))
                .ToListAsync();
        }
    }
}