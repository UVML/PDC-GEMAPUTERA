using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sistem_e_daftar_gemaputera.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pengguna> Pengguna {get; set;}
    
    public DbSet<Agensi> Agensi {get; set;}

    public DbSet<Ahli> Ahli {get; set;}
    
    public DbSet<Sukan> Sukan {get; set;}

    public DbSet<Setting> Setting {get; set;}

    public DbSet<Logger> Logger {get; set;}
}
