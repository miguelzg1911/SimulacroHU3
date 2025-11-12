using Catalogo.Domain.Models;

namespace Catalogo.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?>  GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task SaveChangesAsync();
}