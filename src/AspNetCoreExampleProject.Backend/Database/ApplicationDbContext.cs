using AspNetCoreExampleProject.Backend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreExampleProject.Backend.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
}
