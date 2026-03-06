using NutrientMap.Infrastructure.Usda;

namespace NutrientMap.Infrastructure.Tests.Integration.Usda;

public sealed class UsdaFoodHttpServiceIntegrationTests
{
    private const string BaseUrl = "https://api.nal.usda.gov/fdc/v1";
    private const string ApiKey = "6AWaRCviOJLOGw9BWfd7q478ViWdNXoAh4knRup5";
    private const int RiceFdcId = 2512381;

    [Fact]
    public async Task GetFoodByFdcIdAsync_ReturnsRiceFromLiveUsdaApi()
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        var options = new UsdaFoodApiOptions
        {
            BaseUrl = BaseUrl,
            ApiKey = ApiKey
        };

        var service = new UsdaFoodHttpService(httpClient, options);

        var food = await service.GetFoodByFdcIdAsync(RiceFdcId);

        Assert.NotNull(food);
        Assert.Equal(RiceFdcId, food.FdcId);
        Assert.False(string.IsNullOrWhiteSpace(food.Description));
        Assert.Contains("rice", food.Description, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(food.FoodNutrients);
        Assert.Contains(food.FoodNutrients, nutrient => !string.IsNullOrWhiteSpace(nutrient.Name));
    }
}
