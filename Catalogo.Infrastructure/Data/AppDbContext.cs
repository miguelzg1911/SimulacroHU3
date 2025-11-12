using Catalogo.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ } 
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
}