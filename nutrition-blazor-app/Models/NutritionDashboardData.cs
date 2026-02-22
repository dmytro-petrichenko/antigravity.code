namespace nutrition_blazor_app.Models;

public sealed record NutritionDashboardData(
    IReadOnlyList<MacroSection> MacroSections,
    IReadOnlyList<MacronutrientPieSlice> MacronutrientPieSlices,
    IReadOnlyList<MicronutrientItem> Vitamins,
    IReadOnlyList<MicronutrientItem> Minerals);

public sealed record MacroSection(
    string Title,
    string TotalAmount,
    IReadOnlyList<NutrientSegment> Segments)
{
    public string Header => $"{Title} - {TotalAmount}";
}

public sealed record NutrientSegment(
    string Label,
    string Amount,
    double Percentage,
    NutrientColor Color,
    bool ShowInLegend = true);

public enum NutrientColor
{
    Green,
    Blue,
    Gray,
    Beige
}

public sealed record MacronutrientPieSlice(
    string Label,
    int Percentage,
    string ColorKey,
    string PositionKey);

public sealed record MicronutrientItem(
    string Name,
    string Details,
    double Percentage);
