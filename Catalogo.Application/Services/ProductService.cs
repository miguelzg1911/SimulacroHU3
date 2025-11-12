using Catalogo.Application.Interfaces;
using Catalogo.Domain.Interfaces;
using Catalogo.Domain.Models;

namespace Catalogo.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    
    public ProductService(IProductRepository repository)
    {
        _productRepository = repository;
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            return await _productRepository.GetAllAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Error al obtener los productos", e);
        }
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) 
            {
                throw new Exception("No se encontro el producto");
            }
            
            return existing;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el producto", ex);
        }
    }

    public async Task<Product> CreateAsync(Product product)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(product.Name) ||
                (product.Quantity <= 0) ||
                (product.Price <= 0))
            {
                throw new Exception("Los campos son obligatorios, y el precio debe ser mayor a 0");
            }

            if (product.UserId <= 0)
            {
                throw new Exception("Debe estar asociado a un usuario");
            }

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al crear el producto: {ex.Message}");
        }
    }

    public async Task<bool> UpdateAsync(int id, Product product)
    {
        try
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("No se encontro el producto");
        
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new Exception("El nombre del producto es obligatorio");

            if (product.UserId <= 0)
                throw new Exception("El precio debe ser  mayor a 0");
        
            existing.Name = product.Name;
            existing.Quantity = product.Quantity;
            existing.Price = product.Price;
        
            await _productRepository.UpdateAsync(existing);
            await _productRepository.SaveChangesAsync();
        
            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Error al actualizar el producto: {e.Message}");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var delete = await _productRepository.GetByIdAsync(id);
            if (delete == null)
                throw new Exception("No se encontro el producto");

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar el producto: {ex.Message}");
        }
    }
}