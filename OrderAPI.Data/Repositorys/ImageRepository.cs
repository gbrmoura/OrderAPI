using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data.Repositorys
{
    public class ImageRepository
    {
        private readonly OrderAPIContext _context;

        public ImageRepository(OrderAPIContext context)
        {
            _context = context;
        }

        public async Task<ImageModel> Insert(ImageModel image)
        {
            _context.Image.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<ImageModel> Update(ImageModel image)
        {
            _context.Image.Update(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task Delete(ImageModel image)
        {
            image.Status = false;
            _context.Image.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task<ImageModel> Get(int id)
        {
            return await _context.Image
                .Where(c => c.Codigo == id && c.Status == true)
                .SingleOrDefaultAsync();
        }

        public async Task<List<ImageModel>> GetAll()
        {
            return await _context.Image
                .Where(c => c.Status == true)
                .ToListAsync();
        }

        public async Task<List<ImageModel>> Search(string search)
        {
            return await _context.Image
                .Where(c => c.Codigo.ToString().Contains(search) ||
                            c.Nome.Contains(search))
                .Where(c => c.Status == true)
                .ToListAsync();
        }
    }
}