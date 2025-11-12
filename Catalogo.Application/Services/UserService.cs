using System.ComponentModel.DataAnnotations;
using Catalogo.Application.Interfaces;
using Catalogo.Domain.Interfaces;
using Catalogo.Domain.Models;

namespace Catalogo.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository repository)
    {
        _userRepository = repository;
    }
    
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            return await _userRepository.GetAllUsersAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los usuarios  ", ex);
        }
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            var existing = await _userRepository.GetUserByIdAsync(id);
            if (existing == null) 
            {
                throw new Exception("No se encontro el usuario");
            }
            
            return existing;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el usuario", ex);
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(user.Username) ||
                string.IsNullOrWhiteSpace(user.PasswordHash) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Document))
            {
                throw new Exception("Los campos son obligatorios");
            }

            var existing = await _userRepository.GetAllUsersAsync();
            if (existing.Any(d => d.Document == user.Document))
                throw new Exception("El documento ingresado ya existe");

            if (existing.Any(e => e.Email == user.Email))
                throw new Exception("El correo ingresado ya existe");

            if (!Enum.IsDefined(typeof(User.UserRole), user.Role))
            {
                user.Role = User.UserRole.User;
            }
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al crear el usuario {ex.Message}");
        }
    }

    public async Task<bool> UpdateAsync(int id, User user)
    {
        try
        {
            var existing = await _userRepository.GetUserByIdAsync(id);
            if (existing == null) 
                throw new Exception("No se encontro el usuario");
            
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new Exception("El nombre de usuario es obligatorio");
            
            existing.Username = user.Username;
            existing.PasswordHash = user.PasswordHash;
            existing.Email = user.Email;
            existing.Document = user.Document;
            
            await _userRepository.UpdateAsync(existing);
            await _userRepository.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al actualizar el usuario {ex.Message}");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var delete = await _userRepository.GetUserByIdAsync(id);
            if (delete == null)
                throw new Exception("No se encontro el usuario");
            
            await _userRepository.DeleteAsync(id);
            await _userRepository.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar el usuario {ex.Message}");
        }
    }
}