using System.Text.Json.Serialization;

namespace NutrientMap.Infrastructure.Usda;

public sealed class UsdaAbridgedFoodResponseDto
{
    [JsonPropertyName("fdcId")]
    public int FdcId { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("dataType")]
    public string? DataType { get; init; }

    [JsonPropertyName("publicationDate")]
    public string? PublicationDate { get; init; }

    [JsonPropertyName("ndbNumber")]
    public string? NdbNumber { get; init; }

    [JsonPropertyName("brandOwner")]
    public string? BrandOwner { get; init; }

    [JsonPropertyName("gtinUpc")]
    public string? GtinUpc { get; init; }

    [JsonPropertyName("foodCode")]
    public string? FoodCode { get; init; }

    [JsonPropertyName("foodNutrients")]
    public IReadOnlyList<UsdaAbridgedFoodNutrientDto> FoodNutrients { get; init; } = [];
}
