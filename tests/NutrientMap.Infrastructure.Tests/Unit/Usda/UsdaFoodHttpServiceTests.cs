using System.Text.Json;
using NutrientMap.Infrastructure.Tests.Support;
using NutrientMap.Infrastructure.Usda;

namespace NutrientMap.Infrastructure.Tests.Unit.Usda;

public sealed class UsdaFoodHttpServiceTests
{
    [Fact]
    public async Task GetFoodByFdcIdAsync_SendsExpectedRiceRequest()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.JsonResponse("[]"));
        var service = CreateService(handler);

        var result = await service.GetFoodByFdcIdAsync(2512381);

        Assert.Null(result);
        Assert.Equal(1, handler.RequestCount);
        Assert.Equal(HttpMethod.Post, handler.LastMethod);
        Assert.Equal("application/json", handler.LastAcceptHeader);
        Assert.Equal(
            "https://api.nal.usda.gov/fdc/v1/foods?api_key=test-api-key",
            handler.LastRequestUri?.ToString());
        JsonAssert.Equal(FixtureLoader.Load("rice_foods_request.json"), handler.LastRequestBody!);
    }

    [Fact]
    public async Task GetFoodByFdcIdAsync_MapsSunflowerOilResponse()
    {
        var handler = new RecordingHttpMessageHandler(
            _ => RecordingHttpMessageHandler.JsonResponse(FixtureLoader.Load("sunflower_oil_response.json")));
        var service = CreateService(handler);

        var food = await service.GetFoodByFdcIdAsync(1750349);

        Assert.NotNull(food);
        Assert.Equal(1750349, food.FdcId);
        Assert.Equal("Oil, sunflower", food.Description);
        Assert.Equal("Foundation", food.DataType);
        Assert.Equal("2021-04-28", food.PublicationDate);
        Assert.Equal("100262", food.NdbNumber);
        Assert.Null(food.BrandOwner);
        Assert.Equal(3, food.FoodNutrients.Count);

        var firstNutrient = food.FoodNutrients[0];
        Assert.Equal("631", firstNutrient.Number);
        Assert.Equal("PUFA 22:5 n-3 (DPA)", firstNutrient.Name);
        Assert.Equal(0d, firstNutrient.Amount);
        Assert.Equal("G", firstNutrient.UnitName);

        var edgeCaseNutrient = food.FoodNutrients[1];
        Assert.Equal(string.Empty, edgeCaseNutrient.Number);
        Assert.Equal("Delta-7-Stigmastenol", edgeCaseNutrient.Name);
        Assert.Equal(59.1d, edgeCaseNutrient.Amount);
        Assert.Equal("MG", edgeCaseNutrient.UnitName);
        Assert.Equal("A", edgeCaseNutrient.DerivationCode);
        Assert.Equal("Analytical", edgeCaseNutrient.DerivationDescription);
    }

    [Fact]
    public async Task GetFoodByFdcIdAsync_ReturnsNullWhenResponseIsEmpty()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.JsonResponse("[]"));
        var service = CreateService(handler);

        var result = await service.GetFoodByFdcIdAsync(1750349);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetFoodByFdcIdAsync_ThrowsWhenMultipleFoodsAreReturned()
    {
        const string json = """
            [
              { "fdcId": 1, "description": "Food A", "dataType": "Foundation" },
              { "fdcId": 2, "description": "Food B", "dataType": "Foundation" }
            ]
            """;

        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.JsonResponse(json));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetFoodByFdcIdAsync(2512381));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetFoodByFdcIdAsync_ThrowsBeforeSendingRequestWhenFdcIdIsInvalid(int invalidFdcId)
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.JsonResponse("[]"));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetFoodByFdcIdAsync(invalidFdcId));
        Assert.Equal(0, handler.RequestCount);
    }

    private static UsdaFoodHttpService CreateService(RecordingHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        var options = new UsdaFoodApiOptions
        {
            BaseUrl = "https://api.nal.usda.gov/fdc/v1",
            ApiKey = "test-api-key"
        };

        return new UsdaFoodHttpService(httpClient, options);
    }
}
