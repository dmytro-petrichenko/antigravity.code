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
            new MacronutrientPieSlice("Carbohydrates", 63, "blue", "carbohydrates"),
            new MacronutrientPieSlice("Fats", 25, "beige", "fats"),
            new MacronutrientPieSlice("Proteins", 12, "green", "proteins")
        };

        var cut = RenderComponent<MacronutrientPieWidget>(parameters =>
            parameters.Add(component => component.Slices, slices));

        var chart = cut.Find("figure.donut-chart");
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
    public void RendersCaptionPerSliceWithExpectedClasses()
    {
        var slices = new[]
        {
            new MacronutrientPieSlice("Carbohydrates", 60, "blue", "carbohydrates"),
            new MacronutrientPieSlice("Fats", 25, "beige", "fats"),
            new MacronutrientPieSlice("Proteins", 15, "green", "proteins")
        };

        var cut = RenderComponent<MacronutrientPieWidget>(parameters =>
            parameters.Add(component => component.Slices, slices));

        var captions = cut.FindAll("figcaption.slice-label");

        Assert.Equal(3, captions.Count);
        Assert.Contains(captions, c => c.ClassList.Contains("label-carbohydrates"));
        Assert.Contains(captions, c => c.ClassList.Contains("label-fats"));
        Assert.Contains(captions, c => c.ClassList.Contains("label-proteins"));

        var blueSliceCaption = captions.Single(c => c.ClassList.Contains("label-carbohydrates"));
        Assert.Contains("slice-light-text", blueSliceCaption.ClassName);
    }
}
