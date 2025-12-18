using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PartCategory> PartCategories => Set<PartCategory>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Build> Builds => Set<Build>();
    public DbSet<BuildPart> BuildParts => Set<BuildPart>();
    public DbSet<BuildBenchmark> BuildBenchmarks => Set<BuildBenchmark>();
    public DbSet<UserApiKey> UserApiKeys => Set<UserApiKey>();

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

        // User ↔ Builds
        modelBuilder.Entity<Build>()
            .HasOne(b => b.User)
            .WithMany(u => u.Builds)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Build ↔ Benchmarks
        modelBuilder.Entity<BuildBenchmark>()
            .HasOne(bb => bb.Build)
            .WithMany(b => b.Benchmarks)
            .HasForeignKey(bb => bb.BuildId)
            .OnDelete(DeleteBehavior.Cascade);

        // User ↔ Benchmarks (no cascade to avoid multiple paths)
        modelBuilder.Entity<BuildBenchmark>()
            .HasOne(bb => bb.User)
            .WithMany(u => u.Benchmarks)
            .HasForeignKey(bb => bb.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User unique email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        // PartCategory name
        modelBuilder.Entity<PartCategory>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Part name
        modelBuilder.Entity<Part>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Build name
        modelBuilder.Entity<Build>()
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Part price
        modelBuilder.Entity<Part>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // UserApiKey one-to-one with User
        modelBuilder.Entity<UserApiKey>()
            .HasIndex(k => k.Key)
            .IsUnique();

        modelBuilder.Entity<UserApiKey>()
            .HasOne(k => k.User)
            .WithOne(u => u.ApiKey)
            .HasForeignKey<UserApiKey>(k => k.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
