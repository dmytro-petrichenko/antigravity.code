using System.Globalization;
using nutrition_blazor_app.Contracts;

namespace nutrition_blazor_app.Components;

public static class MacroBarWidgetLogic
{
    private const int MinComponentCount = 2;
    private const int MaxComponentCount = 3;
    private const decimal TotalPercentage = 100m;
    private const decimal AllowedDelta = 0.01m;

    private static readonly string[] SegmentColorOrder =
    [
        "green",
        "blue",
        "beige"
    ];

    public static void ValidateComponents(IReadOnlyList<NutritionDashboardDto.MacronutrientComponentDto> components)
    {
        ArgumentNullException.ThrowIfNull(components);

        if (components.Count is < MinComponentCount or > MaxComponentCount)
        {
            throw new InvalidOperationException(
                $"Macro widget must contain {MinComponentCount} or {MaxComponentCount} sections.");
        }

        if (components.Any(component => component.PercentOfParent <= 0))
        {
            throw new InvalidOperationException("Each section percentage must be greater than zero.");
        }

        var total = components.Sum(component => component.PercentOfParent);
        if (Math.Abs(total - TotalPercentage) > AllowedDelta)
        {
            throw new InvalidOperationException(
                $"Section percentages must sum to {TotalPercentage}%. Current total: {total:0.##}%.");
        }
    }

    public static string GetColorKey(int componentIndex)
    {
        if (componentIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(componentIndex), componentIndex, "Index must be non-negative.");
        }

        return SegmentColorOrder[Math.Min(componentIndex, SegmentColorOrder.Length - 1)];
    }

    public static string GetSegmentCssClass(string colorKey) => $"segment-{SanitizeColorKey(colorKey)}";

    public static string GetSwatchCssClass(string colorKey) => $"swatch-{SanitizeColorKey(colorKey)}";

    public static string GetWidthStyle(decimal percentage)
        => $"width: {percentage.ToString("0.##", CultureInfo.InvariantCulture)}%;";

    private static string SanitizeColorKey(string colorKey)
    {
        if (string.IsNullOrWhiteSpace(colorKey))
        {
            throw new ArgumentException("Color key must be provided.", nameof(colorKey));
        }

        return colorKey.Trim().ToLowerInvariant();
    }
}
