namespace nutrition_blazor_app.Models;

public sealed record NutritionDashboardData(
    IReadOnlyList<MacroSection> MacroSections,
    IReadOnlyList<FlavorSlice> FlavorSlices,
    IReadOnlyList<MicronutrientItem> Vitamins,
    IReadOnlyList<MicronutrientItem> Minerals);

public sealed record MacroSection(
    string Title,
    string TotalAmount,
    IReadOnlyList<NutrientSegment> Segments);

public sealed record NutrientSegment(
    string Label,
    string Amount,
    double Percentage,
    string ColorKey);

public sealed record FlavorSlice(
    string Label,
    int Percentage,
    string ColorKey,
    string PositionKey);

public sealed record MicronutrientItem(
    string Name,
    string Details,
    double Percentage);
