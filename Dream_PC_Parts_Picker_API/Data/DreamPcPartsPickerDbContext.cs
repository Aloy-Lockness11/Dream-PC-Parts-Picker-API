using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PartCategory> PartCategories => Set<PartCategory>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Build> Builds => Set<Build>();
    public DbSet<BuildPart> BuildParts => Set<BuildPart>();
    public DbSet<BuildBenchmark> BuildBenchmarks => Set<BuildBenchmark>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // BuildPart composite key
        modelBuilder.Entity<BuildPart>()
            .HasKey(bp => new { bp.BuildId, bp.PartId });

        modelBuilder.Entity<BuildPart>()
            .HasOne(bp => bp.Build)
            .WithMany(b => b.BuildParts)
            .HasForeignKey(bp => bp.BuildId);

        modelBuilder.Entity<BuildPart>()
            .HasOne(bp => bp.Part)
            .WithMany(p => p.BuildParts)
            .HasForeignKey(bp => bp.PartId);

        // User ↔ Builds (cascade is fine here)
        modelBuilder.Entity<Build>()
            .HasOne(b => b.User)
            .WithMany(u => u.Builds)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Build ↔ BuildBenchmarks (cascade is fine)
        modelBuilder.Entity<BuildBenchmark>()
            .HasOne(bb => bb.Build)
            .WithMany(b => b.Benchmarks)
            .HasForeignKey(bb => bb.BuildId)
            .OnDelete(DeleteBehavior.Cascade);

        // User ↔ BuildBenchmarks (NO cascade to avoid multiple cascade paths)
        modelBuilder.Entity<BuildBenchmark>()
            .HasOne(bb => bb.User)
            .WithMany(u => u.Benchmarks)
            .HasForeignKey(bb => bb.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique email for users
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        modelBuilder.Entity<PartCategory>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Part>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Build>()
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Part>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");
    }

}