using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dream_PC_Parts_Picker_API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Apply any pending migrations
        await db.Database.MigrateAsync();

        await SeedCategoriesAsync(db);
        await SeedPartsAsync(db);
    }

    private static async Task SeedCategoriesAsync(AppDbContext db)
    {
        var wanted = new[]
        {
            "CPU",
            "GPU",
            "Motherboard",
            "RAM",
            "Storage",
            "PSU",
            "Case",
            "Cooler"
        };

        foreach (var name in wanted)
        {
            var exists = await db.PartCategories.AnyAsync(c => c.Name == name);
            if (!exists)
            {
                db.PartCategories.Add(new PartCategory
                {
                    Name = name,
                    Description = $"{name} components"
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedPartsAsync(AppDbContext db)
    {
        // Categories must exist first
        var cats = await db.PartCategories
            .ToDictionaryAsync(c => c.Name, c => c.Id);

        // if seeding runs too early and some categories are missing, bail out
        if (!cats.ContainsKey("CPU") || !cats.ContainsKey("GPU"))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var parts = new List<Part>
        {
            new()
            {
                PartCategoryId = cats["CPU"],
                Name = "Ryzen 5 5600",
                Manufacturer = "AMD",
                ModelNumber = "100-100000927BOX",
                Price = 129.99m,
                PerformanceScore = 7200,
                TdpWatts = 65,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["CPU"],
                Name = "Core i5-12400F",
                Manufacturer = "Intel",
                ModelNumber = "BX8071512400F",
                Price = 139.99m,
                PerformanceScore = 7600,
                TdpWatts = 65,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["GPU"],
                Name = "GeForce RTX 4060",
                Manufacturer = "NVIDIA",
                ModelNumber = "RTX4060-8G",
                Price = 299.99m,
                PerformanceScore = 8200,
                TdpWatts = 115,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["GPU"],
                Name = "Radeon RX 6700 XT",
                Manufacturer = "AMD",
                ModelNumber = "RX6700XT-12G",
                Price = 319.99m,
                PerformanceScore = 8800,
                TdpWatts = 230,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["Motherboard"],
                Name = "B550 Tomahawk",
                Manufacturer = "MSI",
                ModelNumber = "MSI-B550-TOMAHAWK",
                Price = 149.99m,
                PerformanceScore = 0,
                TdpWatts = 0,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["RAM"],
                Name = "16GB DDR4 3200 (2x8)",
                Manufacturer = "Corsair",
                ModelNumber = "CMK16GX4M2B3200C16",
                Price = 44.99m,
                PerformanceScore = 0,
                TdpWatts = 0,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["Storage"],
                Name = "1TB NVMe SSD",
                Manufacturer = "Samsung",
                ModelNumber = "MZ-V8V1T0",
                Price = 69.99m,
                PerformanceScore = 0,
                TdpWatts = 0,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["PSU"],
                Name = "650W 80+ Gold",
                Manufacturer = "Seasonic",
                ModelNumber = "SSR-650FX",
                Price = 89.99m,
                PerformanceScore = 0,
                TdpWatts = 0,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["Case"],
                Name = "Mid Tower ATX",
                Manufacturer = "NZXT",
                ModelNumber = "H510",
                Price = 79.99m,
                PerformanceScore = 0,
                TdpWatts = 0,
                CreatedAt = now
            },
            new()
            {
                PartCategoryId = cats["Cooler"],
                Name = "Air Cooler 120mm",
                Manufacturer = "Cooler Master",
                ModelNumber = "HYPER-212",
                Price = 29.99m,
                PerformanceScore = 0,
                TdpWatts = 0,
                CreatedAt = now
            }
        };

        foreach (var p in parts)
        {
            var exists = await db.Parts.AnyAsync(x => x.ModelNumber == p.ModelNumber);
            if (!exists)
            {
                db.Parts.Add(p);
            }
        }

        await db.SaveChangesAsync();
    }
}
