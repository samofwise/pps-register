using Microsoft.EntityFrameworkCore;
using PPSRegister.Data.Models;

namespace PPSRegister.Data;

public class PPSRegisterDbContext(DbContextOptions<PPSRegisterDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<PersonalPropertySecurityUpload> PersonalPropertySecurityUploads { get; set; } = null!;
    public DbSet<PersonalPropertySecurity> PersonalProperySecuries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
