using Domain.Product;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product, int>, IProductRepository
{
    public ProductRepository(EStoreContext context) : base(context)
    {
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
           .Include(p => p.Images)
           .Include(p => p.ProductItems)
           .ThenInclude(productItem => productItem.Image)
           .FirstOrDefaultAsync(p => p.Id == id);
    }
}
