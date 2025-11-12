using Catalogo.Domain.Models;

namespace Catalogo.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product student);
    Task<bool> UpdateAsync(int id, Product student);
    Task<bool> DeleteAsync(int id);
}