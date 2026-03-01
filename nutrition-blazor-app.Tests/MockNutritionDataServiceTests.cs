using nutrition_blazor_app.Services;

namespace nutrition_blazor_app.Tests;

public sealed class MockNutritionDataServiceTests
{
    [Fact]
    public void Dashboard_HasExpectedMicronutrientCounts()
    {
        var service = new MockNutritionDataService();

        var dashboard = service.GetDashboard();

        Assert.Equal(13, dashboard.Vitamins.Items.Count);
        Assert.Equal(16, dashboard.Minerals.Items.Count);
    }

    [Fact]
    public void Dashboard_MicronutrientNamesAreUniqueWithinEachGroup()
    {
        var service = new MockNutritionDataService();

        var dashboard = service.GetDashboard();

        Assert.Equal(
            dashboard.Vitamins.Items.Count,
            dashboard.Vitamins.Items.Select(item => item.NutrientName).Distinct(StringComparer.Ordinal).Count());

        Assert.Equal(
            dashboard.Minerals.Items.Count,
            dashboard.Minerals.Items.Select(item => item.NutrientName).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Dashboard_MicronutrientPercentagesStayWithinDisplayRange()
    {
        var service = new MockNutritionDataService();

        var dashboard = service.GetDashboard();

        Assert.All(dashboard.Vitamins.Items, item => Assert.InRange(item.PercentDailyValue, 0, 100));
        Assert.All(dashboard.Minerals.Items, item => Assert.InRange(item.PercentDailyValue, 0, 100));
    }
}
