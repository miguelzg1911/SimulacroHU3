using Catalogo.Domain.Interfaces;
using Catalogo.Domain.Models;
using Catalogo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbContext.Products.FindAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        _dbContext.Products.Update(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _dbContext.Products.FindAsync(id);
        if (existing == null) return false;
        
        _dbContext.Products.Remove(existing);
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}