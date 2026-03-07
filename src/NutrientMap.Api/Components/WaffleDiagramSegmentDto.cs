namespace NutrientMap.Api.Components;

public sealed class WaffleDiagramSegmentDto
{
    public int CellCount { get; init; }
    public string Color { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
}
