using Catalogo.Domain.Models;

namespace Catalogo.Domain.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task SaveChangesAsync();
}