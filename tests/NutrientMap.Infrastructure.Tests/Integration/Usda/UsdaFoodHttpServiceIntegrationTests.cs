using NutrientMap.Infrastructure.Usda;
using NutrientMap.Infrastructure.Usda.Fats;

namespace NutrientMap.Infrastructure.Tests.Integration.Usda;

public sealed class UsdaFoodHttpServiceIntegrationTests
{
    private const string BaseUrl = "https://api.nal.usda.gov/fdc/v1";
    private const string ApiKey = "6AWaRCviOJLOGw9BWfd7q478ViWdNXoAh4knRup5";
    private const int RiceFdcId = 2512381;

    [Fact]
    public async Task GetFoodByFdcIdAsync_ReturnsRiceFromLiveUsdaApi()
    {
        using var httpClient = CreateHttpClient();
        var service = CreateFoodHttpService(httpClient);

        var food = await service.GetFoodByFdcIdAsync(RiceFdcId);

        Assert.NotNull(food);
        Assert.Equal(RiceFdcId, food.FdcId);
        Assert.False(string.IsNullOrWhiteSpace(food.Description));
        Assert.Contains("rice", food.Description, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(food.FoodNutrients);
        Assert.Contains(food.FoodNutrients, nutrient => !string.IsNullOrWhiteSpace(nutrient.Name));
    }

    [Fact]
    public async Task GetFatProfileOfProductByFdcIdAsync_ReturnsRiceFatProfileFromLiveUsdaApi()
    {
        using var httpClient = CreateHttpClient();
        var foodHttpService = CreateFoodHttpService(httpClient);
        var fatProfileService = new UsdaFatProfileService(foodHttpService, new FatProfileFilter());

        var fatProfile = await fatProfileService.GetFatProfileOfProductByFdcIdAsync(1750349);

        Assert.True(fatProfile.TotalFat > 0f);
        Assert.True(fatProfile.SaturatedFat >= 0f);
        Assert.True(fatProfile.MonounsaturatedFat >= 0f);
        Assert.True(fatProfile.PolyunsaturatedFat >= 0f);
        Assert.True(fatProfile.TransFat >= 0f);
    }

    private static HttpClient CreateHttpClient() => new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    private static UsdaFoodHttpService CreateFoodHttpService(HttpClient httpClient)
    {
        var options = new UsdaFoodApiOptions
        {
            BaseUrl = BaseUrl,
            ApiKey = ApiKey
        };

        return new UsdaFoodHttpService(httpClient, options);
    }
}
