using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories;

public class ProductsRepository(ApplicationDbContext dbContext) : IProductsRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    public async Task<Product?> AddProduct(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProduct(Guid productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
        {
            return false;
        }

        _dbContext.Products.Remove(product);
        int count = await _dbContext.SaveChangesAsync();
        return count > 0;
    }

    public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> predicate)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> predicate)
    {
        return await _dbContext.Products.Where(predicate).ToListAsync();
    }

    public async Task<Product?> UpdateProduct(Product product)
    {
        var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

        if (existingProduct == null)
        {
            return null;
        }

        existingProduct.ProductName = product.ProductName;
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.QuantityInStock = product.QuantityInStock;
        existingProduct.Category = product.Category;

        await _dbContext.SaveChangesAsync();

        return existingProduct;
    }
}
