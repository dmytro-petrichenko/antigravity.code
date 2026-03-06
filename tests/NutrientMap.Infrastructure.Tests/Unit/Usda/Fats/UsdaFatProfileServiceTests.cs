using NutrientMap.Infrastructure.Tests.Support;
using NutrientMap.Infrastructure.Usda;
using NutrientMap.Infrastructure.Usda.Fats;

namespace NutrientMap.Infrastructure.Tests.Unit.Usda.Fats;

public sealed class UsdaFatProfileServiceTests
{
    [Fact]
    public async Task GetFatProfileOfProductByFdcIdAsync_ReturnsFilteredFatProfile()
    {
        var handler = new RecordingHttpMessageHandler(
            _ => RecordingHttpMessageHandler.JsonResponse(FixtureLoader.Load("sunflower_oil_fat_response.json")));
        var service = CreateService(handler);

        var profile = await service.GetFatProfileOfProductByFdcIdAsync(1750349);

        Assert.Equal(93.2f, profile.TotalFat, 2);
        Assert.Equal(8.99f, profile.SaturatedFat, 2);
        Assert.Equal(63.4f, profile.MonounsaturatedFat, 2);
        Assert.Equal(20.7f, profile.PolyunsaturatedFat, 2);
        Assert.Equal(0.116f, profile.TransFat, 3);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task GetFatProfileOfProductByFdcIdAsync_ThrowsWhenFoodIsMissing()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.JsonResponse("[]"));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetFatProfileOfProductByFdcIdAsync(2512381));
        Assert.Equal(1, handler.RequestCount);
    }

    private static UsdaFatProfileService CreateService(RecordingHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        var options = new UsdaFoodApiOptions
        {
            BaseUrl = "https://api.nal.usda.gov/fdc/v1",
            ApiKey = "test-api-key"
        };

        var foodHttpService = new UsdaFoodHttpService(httpClient, options);
        return new UsdaFatProfileService(foodHttpService, new FatProfileFilter());
    }
}
