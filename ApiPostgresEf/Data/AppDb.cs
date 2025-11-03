using Microsoft.EntityFrameworkCore;
using ProjetoLogin.Models;

namespace ProjetoLogin.Data;
public class AppDb(DbContextOptions<AppDb> opts) : DbContext(opts)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            e.Property(x => x.UserName).IsRequired().HasMaxLength(100);
            e.Property(x => x.SenhaHash).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Email).IsUnique();
        });
    }
}
