using Bunit;
using nutrition_blazor_app.Components;
using nutrition_blazor_app.Models;

namespace nutrition_blazor_app.Tests;

public sealed class MacronutrientPieWidgetTests : TestContext
{
    [Fact]
    public void RendersAccessibleLabelAndGradientFromSlices()
    {
        var slices = new[]
        {
            new MacronutrientPieSlice("Carbohydrates", 63, "blue"),
            new MacronutrientPieSlice("Fats", 25, "beige"),
            new MacronutrientPieSlice("Proteins", 12, "green")
        };

        var cut = RenderComponent<MacronutrientPieWidget>(parameters =>
            parameters.Add(component => component.Slices, slices));

        var chart = cut.Find("div.donut-chart");
        var style = chart.GetAttribute("style");

        Assert.Equal("img", chart.GetAttribute("role"));
        Assert.Equal(
            "Macronutrient distribution: Carbohydrates 63 percent, Fats 25 percent, Proteins 12 percent",
            chart.GetAttribute("aria-label"));
        Assert.NotNull(style);
        Assert.Contains("conic-gradient(", style);
        Assert.Contains("var(--blue) 0% 63%", style);
        Assert.Contains("var(--beige) 63% 88%", style);
        Assert.Contains("var(--green) 88% 100%", style);
    }

    [Fact]
    public void RendersLegendUnderChartWithColorSwatches()
    {
        var slices = new[]
        {
            new MacronutrientPieSlice("Carbohydrates", 60, "blue"),
            new MacronutrientPieSlice("Fats", 25, "beige"),
            new MacronutrientPieSlice("Proteins", 15, "green")
        };

        var cut = RenderComponent<MacronutrientPieWidget>(parameters =>
            parameters.Add(component => component.Slices, slices));

        var donut = cut.Find("div.donut-chart");
        var legendItems = cut.FindAll("li.pie-legend-item");
        var swatches = cut.FindAll("span.pie-legend-swatch");

        Assert.Empty(cut.FindAll("figcaption.slice-label"));
        Assert.Equal(3, legendItems.Count);
        Assert.Equal(3, swatches.Count);
        Assert.Contains("Carbohydrates - 60%", legendItems[0].TextContent);
        Assert.Contains("Fats - 25%", legendItems[1].TextContent);
        Assert.Contains("Proteins - 15%", legendItems[2].TextContent);

        Assert.Equal("background: var(--blue);", swatches[0].GetAttribute("style"));
        Assert.Equal("background: var(--beige);", swatches[1].GetAttribute("style"));
        Assert.Equal("background: var(--green);", swatches[2].GetAttribute("style"));
        Assert.NotNull(donut.NextElementSibling);
    }
}
