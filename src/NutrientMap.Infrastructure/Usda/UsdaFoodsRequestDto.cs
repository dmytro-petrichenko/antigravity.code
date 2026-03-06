using System.Text.Json.Serialization;

namespace NutrientMap.Infrastructure.Usda;

public sealed class UsdaFoodsRequestDto
{
    [JsonPropertyName("fdcIds")]
    public IReadOnlyList<int> FdcIds { get; init; } = [];

    [JsonPropertyName("format")]
    public string Format { get; init; } = "abridged";
}
