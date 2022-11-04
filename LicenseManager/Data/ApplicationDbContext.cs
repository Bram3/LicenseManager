using LicenseManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<License> Licenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySql(_configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(10, 4)));
    }
}