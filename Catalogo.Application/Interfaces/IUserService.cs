using Catalogo.Domain.Models;

namespace Catalogo.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User student);
    Task<bool> UpdateAsync(int id, User student);
    Task<bool> DeleteAsync(int id);
}