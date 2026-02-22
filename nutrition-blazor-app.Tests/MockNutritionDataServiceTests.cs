using nutrition_blazor_app.Services;

namespace nutrition_blazor_app.Tests;

public sealed class MockNutritionDataServiceTests
{
    [Fact]
    public void Dashboard_HasExpectedMicronutrientCounts()
    {
        var service = new MockNutritionDataService();

        var dashboard = service.GetDashboard();

        Assert.Equal(13, dashboard.Vitamins.Count);
        Assert.Equal(16, dashboard.Minerals.Count);
    }

    [Fact]
    public void Dashboard_MicronutrientNamesAreUniqueWithinEachGroup()
    {
        var service = new MockNutritionDataService();

        var dashboard = service.GetDashboard();

        Assert.Equal(
            dashboard.Vitamins.Count,
            dashboard.Vitamins.Select(item => item.Name).Distinct(StringComparer.Ordinal).Count());

        Assert.Equal(
            dashboard.Minerals.Count,
            dashboard.Minerals.Select(item => item.Name).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Dashboard_MicronutrientPercentagesStayWithinDisplayRange()
    {
        var service = new MockNutritionDataService();

        var dashboard = service.GetDashboard();

        Assert.All(dashboard.Vitamins, item => Assert.InRange(item.Percentage, 0, 100));
        Assert.All(dashboard.Minerals, item => Assert.InRange(item.Percentage, 0, 100));
    }
}
