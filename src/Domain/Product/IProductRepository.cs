﻿using Domain.Common;

namespace Domain.Product;

public interface IProductRepository : IRepository<Product, int>
{
    public Task<IEnumerable<Product>> GetProductByProductItemIds(IList<int> productItemIds);

    public Task<bool> IsAllProductsExist(IList<int> productIds);
}
