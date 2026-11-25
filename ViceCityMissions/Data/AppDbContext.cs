using Microsoft.EntityFrameworkCore;
using ViceCityMissions.Models;

namespace ViceCityMissions.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<MissionCompletion> Completions => Set<MissionCompletion>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<MissionCompletion>()
            .HasIndex(c => new { c.MissionId, c.CompletedOn })
            .IsUnique();

        b.Entity<Mission>().HasData(
            new Mission { Id = 1, Title = "Correr por Ocean Drive", Icon = "🏃", Points = 50, Description = "Entrenamiento matutino", CreatedAt = new DateTime(2026, 1, 1) },
            new Mission { Id = 2, Title = "Estudiar el plan del golpe", Icon = "📚", Points = 30, Description = "Lee 30 minutos", CreatedAt = new DateTime(2026, 1, 1) },
            new Mission { Id = 3, Title = "Mantener la calma (Meditar)", Icon = "🧘", Points = 20, CreatedAt = new DateTime(2026, 1, 1) },
            new Mission { Id = 4, Title = "Hidratación en el Keys", Icon = "💧", Points = 15, Description = "8 vasos de agua", CreatedAt = new DateTime(2026, 1, 1) },
            new Mission { Id = 5, Title = "Sin teléfono tras las 9pm", Icon = "🌙", Points = 40, Description = "Desaparecer del radar", CreatedAt = new DateTime(2026, 1, 1) }
        );
    }
}