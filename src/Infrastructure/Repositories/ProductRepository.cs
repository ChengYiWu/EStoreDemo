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

    public async Task<IEnumerable<Product>> GetProductByProductItemIds(IList<int> productItemIds)
    {
        return await _dbSet
            .Include(p => p.ProductItems)
            .Where(p => p.ProductItems.Any(
                productItem => productItemIds.Any(id => productItem.Id == id))
            )
            .ToListAsync();
    }

    public async Task<bool> IsAllProductsExist(IList<int> productIds)
    {
        return await _query.CountAsync(p => productIds.Contains(p.Id)) == productIds.Count;
    }
}
