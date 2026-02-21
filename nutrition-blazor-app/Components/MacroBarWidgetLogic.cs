using nutrition_blazor_app.Models;

namespace nutrition_blazor_app.Components;

public static class MacroBarWidgetLogic
{
    private const int MinSectionCount = 2;
    private const int MaxSectionCount = 3;
    private const double TotalPercentage = 100d;
    private const double AllowedDelta = 0.01d;

    public static void ValidateSections(IReadOnlyList<NutrientSegment> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        if (sections.Count is < MinSectionCount or > MaxSectionCount)
        {
            throw new InvalidOperationException(
                $"Macro widget must contain {MinSectionCount} or {MaxSectionCount} sections.");
        }

        if (sections.Any(section => section.Percentage <= 0))
        {
            throw new InvalidOperationException("Each section percentage must be greater than zero.");
        }

        var total = sections.Sum(section => section.Percentage);
        if (Math.Abs(total - TotalPercentage) > AllowedDelta)
        {
            throw new InvalidOperationException(
                $"Section percentages must sum to {TotalPercentage}%. Current total: {total:0.##}%.");
        }
    }

    public static string GetSegmentCssClass(NutrientColor color) => $"segment-{ToCssToken(color)}";

    public static string GetSwatchCssClass(NutrientColor color) => $"swatch-{ToCssToken(color)}";

    public static string GetWidthStyle(double percentage) => $"width: {percentage:0.##}%;";

    private static string ToCssToken(NutrientColor color) => color switch
    {
        NutrientColor.Green => "green",
        NutrientColor.Blue => "blue",
        NutrientColor.Gray => "gray",
        NutrientColor.Beige => "beige",
        _ => throw new ArgumentOutOfRangeException(nameof(color), color, "Unsupported nutrient color.")
    };
}
