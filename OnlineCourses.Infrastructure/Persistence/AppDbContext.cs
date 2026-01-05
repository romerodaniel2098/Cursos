using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Domain.Courses;
using OnlineCourses.Infrastructure.Auth;

using OnlineCourses.Application.Common.Interfaces;

namespace OnlineCourses.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Course =====
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            // Guardar enum como string (Draft/Published)
            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Soft delete global filter
            entity.HasQueryFilter(x => !x.IsDeleted);

            entity.HasMany(x => x.Lessons)
                .WithOne()
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Lesson =====
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("Lessons");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            // "Order" puede chocar con palabra reservada en SQL -> mapeamos columna con otro nombre
            entity.Property(x => x.Order)
                .HasColumnName("LessonOrder")
                .IsRequired();

            // Soft delete global filter
            entity.HasQueryFilter(x => !x.IsDeleted);

            // Regla: Order único por curso (reforzada en BD)
            entity.HasIndex(x => new { x.CourseId, x.Order })
                  .IsUnique();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is null) continue;

            // Actualiza timestamps si existen propiedades
            var createdAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
            var updatedAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");

            if (entry.State == EntityState.Added)
            {
                if (createdAt != null) createdAt.CurrentValue = now;
                if (updatedAt != null) updatedAt.CurrentValue = now;
            }
            if (entry.State == EntityState.Modified)
            {
                if (updatedAt != null) updatedAt.CurrentValue = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
