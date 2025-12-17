using DatrixFinances.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace DatrixFinances.API.Utils;

public class DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configuration) : DbContext(options)
{
    private readonly IConfiguration _configuration = configuration;
    
    public DbSet<APIActivityAspect> APIActivityLogs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer($"Server={_configuration["Database:Address"]};Database={_configuration["Database:DatabaseName"]};User Id={_configuration["Database:User:Name"]};Password={_configuration["Database:User:Password"]};TrustServerCertificate=True;Encrypt=True;");
    }
}