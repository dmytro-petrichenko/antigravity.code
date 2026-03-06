using System.Text.Json.Serialization;

namespace NutrientMap.Infrastructure.Usda;

public sealed class UsdaAbridgedFoodNutrientDto
{
    [JsonPropertyName("number")]
    public string? Number { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("amount")]
    public double? Amount { get; init; }

    [JsonPropertyName("unitName")]
    public string? UnitName { get; init; }

    [JsonPropertyName("derivationCode")]
    public string? DerivationCode { get; init; }

    [JsonPropertyName("derivationDescription")]
    public string? DerivationDescription { get; init; }
}
