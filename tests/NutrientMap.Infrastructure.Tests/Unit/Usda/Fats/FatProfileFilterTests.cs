using System.Text.Json;
using NutrientMap.Infrastructure.Tests.Support;
using NutrientMap.Infrastructure.Usda;
using NutrientMap.Infrastructure.Usda.Fats;

namespace NutrientMap.Infrastructure.Tests.Unit.Usda.Fats;

public sealed class FatProfileFilterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly FatProfileFilter _filter = new();
    private readonly FatFilter _fatFilter = FatFilter.CreateDefault();

    [Fact]
    public void ApplyFilter_MapsRiceTotalFatUsingPrimary204Number()
    {
        var dto = LoadSingleFoodFixture("rice_response.json");

        var profile = _filter.ApplyFilter(_fatFilter, dto);

        Assert.Equal(1.03f, profile.TotalFat, 2);
        Assert.Equal(0f, profile.SaturatedFat);
        Assert.Equal(0f, profile.MonounsaturatedFat);
        Assert.Equal(0f, profile.PolyunsaturatedFat);
        Assert.Equal(0f, profile.TransFat);
    }

    [Fact]
    public void ApplyFilter_MapsSunflowerOilUsingFallbackAndGroupedFatNumbers()
    {
        var dto = LoadSingleFoodFixture("sunflower_oil_fat_response.json");

        var profile = _filter.ApplyFilter(_fatFilter, dto);

        Assert.Equal(93.2f, profile.TotalFat, 2);
        Assert.Equal(8.99f, profile.SaturatedFat, 2);
        Assert.Equal(63.4f, profile.MonounsaturatedFat, 2);
        Assert.Equal(20.7f, profile.PolyunsaturatedFat, 2);
        Assert.Equal(0.116f, profile.TransFat, 3);
    }

    [Fact]
    public void ApplyFilter_UsesTransFatFallbackWhenDirectNumberIsMissing()
    {
        var dto = new UsdaAbridgedFoodResponseDto
        {
            FdcId = 1,
            FoodNutrients =
            [
                new UsdaAbridgedFoodNutrientDto
                {
                    Number = "693",
                    Name = "Fatty acids, total trans-monoenoic",
                    Amount = 0.0349,
                    UnitName = "G"
                },
                new UsdaAbridgedFoodNutrientDto
                {
                    Number = "695",
                    Name = "Fatty acids, total trans-polyenoic",
                    Amount = 0.0811,
                    UnitName = "G"
                }
            ]
        };

        var profile = _filter.ApplyFilter(_fatFilter, dto);

        Assert.Equal(0.116f, profile.TransFat, 3);
    }

    [Fact]
    public void ApplyFilter_ReturnsZeroForMissingConfiguredNutrients()
    {
        var dto = new UsdaAbridgedFoodResponseDto
        {
            FdcId = 1,
            FoodNutrients =
            [
                new UsdaAbridgedFoodNutrientDto
                {
                    Number = "303",
                    Name = "Iron, Fe",
                    Amount = 0.14,
                    UnitName = "MG"
                }
            ]
        };

        var profile = _filter.ApplyFilter(_fatFilter, dto);

        Assert.Equal(0f, profile.TotalFat);
        Assert.Equal(0f, profile.SaturatedFat);
        Assert.Equal(0f, profile.MonounsaturatedFat);
        Assert.Equal(0f, profile.PolyunsaturatedFat);
        Assert.Equal(0f, profile.TransFat);
    }

    private static UsdaAbridgedFoodResponseDto LoadSingleFoodFixture(string fileName)
    {
        var foods = JsonSerializer.Deserialize<List<UsdaAbridgedFoodResponseDto>>(
            FixtureLoader.Load(fileName),
            JsonOptions);

        Assert.NotNull(foods);
        Assert.Single(foods);
        return foods[0];
    }
}
