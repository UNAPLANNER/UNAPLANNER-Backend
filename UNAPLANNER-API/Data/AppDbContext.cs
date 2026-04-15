using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    
    public DbSet<User> Users => Set<User>();
}
