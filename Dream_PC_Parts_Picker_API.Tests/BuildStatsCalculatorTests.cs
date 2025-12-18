using Dream_PC_Parts_Picker_API.Models;
using Dream_PC_Parts_Picker_API.Services;
using Xunit;

namespace Dream_PC_Parts_Picker_API.Tests;

public class BuildStatsCalculatorTests
{
    [Fact]
    public void ComputeTotals_SinglePart_SimpleValues()
    {
        // Arrange
        var calculator = new BuildStatsCalculator();

        var cpu = new Part
        {
            Id = 1,
            Name = "Test CPU",
            Price = 200m,
            TdpWatts = 65,
            PerformanceScore = 80
        };

        var build = new Build
        {
            Id = 1,
            BuildParts = new List<BuildPart>
            {
                new BuildPart
                {
                    BuildId = 1,
                    PartId = 1,
                    Quantity = 1,
                    Part = cpu
                }
            }
        };

        // Act
        var totals = calculator.ComputeTotals(build);

        // Assert
        Assert.Equal(200m, totals.TotalPrice);
        Assert.Equal(65, totals.TotalTdpWatts);
        Assert.Equal(80, totals.TotalPerformanceScore);
    }

    [Fact]
    public void ComputeTotals_MultipleParts_MultipliesByQuantity_HandlesNulls()
    {
        // Arrange
        var calculator = new BuildStatsCalculator();

        var cpu = new Part
        {
            Id = 1,
            Name = "CPU",
            Price = 250m,
            TdpWatts = 65,
            PerformanceScore = 90
        };

        var gpu = new Part
        {
            Id = 2,
            Name = "GPU",
            Price = 500m,
            TdpWatts = 200,
            PerformanceScore = 120
        };

        var ram = new Part
        {
            Id = 3,
            Name = "RAM",
            Price = 80m,
            TdpWatts = null,
            PerformanceScore = null
        };

        var build = new Build
        {
            Id = 1,
            BuildParts = new List<BuildPart>
            {
                new BuildPart { BuildId = 1, PartId = 1, Quantity = 1, Part = cpu },
                new BuildPart { BuildId = 1, PartId = 2, Quantity = 1, Part = gpu },
                new BuildPart { BuildId = 1, PartId = 3, Quantity = 2, Part = ram }
            }
        };

        // Expected:
        // Price: 250 + 500 + (80 * 2) = 910
        // TDP:   65 + 200 + (0  * 2) = 265
        // Perf:  90 + 120 + (0  * 2) = 210

        // Act
        var totals = calculator.ComputeTotals(build);

        // Assert
        Assert.Equal(910m, totals.TotalPrice);
        Assert.Equal(265, totals.TotalTdpWatts);
        Assert.Equal(210, totals.TotalPerformanceScore);
    }

    [Fact]
    public void ComputeTotals_EmptyBuild_ReturnsZeros()
    {
        // Arrange
        var calculator = new BuildStatsCalculator();
        var build = new Build
        {
            Id = 1,
            BuildParts = new List<BuildPart>()
        };

        // Act
        var totals = calculator.ComputeTotals(build);

        // Assert
        Assert.Equal(0m, totals.TotalPrice);
        Assert.Equal(0, totals.TotalTdpWatts);
        Assert.Equal(0, totals.TotalPerformanceScore);
    }
}
