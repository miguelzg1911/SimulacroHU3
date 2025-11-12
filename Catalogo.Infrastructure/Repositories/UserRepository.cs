using Catalogo.Domain.Interfaces;
using Catalogo.Domain.Models;
using Catalogo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _dbContext.Users.FindAsync(id);
        if (existing == null) return false;
        
        _dbContext.Users.Remove(existing);
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}