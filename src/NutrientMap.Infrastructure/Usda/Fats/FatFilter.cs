namespace NutrientMap.Infrastructure.Usda.Fats;

public sealed class FatFilter
{
    // Candidate nutrient numbers are derived from usda_api/nutrient_groups.md.
    public IReadOnlyList<string> TotalFatNumbers { get; init; } = ["204", "298"];

    public IReadOnlyList<string> SaturatedFatNumbers { get; init; } = ["606", "690"];

    public IReadOnlyList<string> MonounsaturatedFatNumbers { get; init; } = ["645", "691"];

    public IReadOnlyList<string> PolyunsaturatedFatNumbers { get; init; } = ["646", "692"];

    public IReadOnlyList<string> TransFatNumbers { get; init; } = ["605"];

    public IReadOnlyList<string> TransFatFallbackComponentNumbers { get; init; } = ["693", "695"];

    public static FatFilter CreateDefault() => new();
}
