using System.Text.Json;
using NutrientMap.Infrastructure.Tests.Support;
using NutrientMap.Infrastructure.Usda;

namespace NutrientMap.Infrastructure.Tests.Unit.Usda;

public sealed class UsdaJsonContractTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void MagnesiumNutrient_DeserializesUsingRawDtoContract()
    {
        var nutrient = JsonSerializer.Deserialize<UsdaAbridgedFoodNutrientDto>(
            FixtureLoader.Load("magnesium_nutrient.json"),
            JsonOptions);

        Assert.NotNull(nutrient);
        Assert.Equal("304", nutrient.Number);
        Assert.Equal("Magnesium, Mg", nutrient.Name);
        Assert.Equal(26.5d, nutrient.Amount);
        Assert.Equal("MG", nutrient.UnitName);
        Assert.Equal("A", nutrient.DerivationCode);
        Assert.Equal("Analytical", nutrient.DerivationDescription);
    }
}
