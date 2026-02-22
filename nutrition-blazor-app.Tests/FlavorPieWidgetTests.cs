using Bunit;
using nutrition_blazor_app.Components;
using nutrition_blazor_app.Models;

namespace nutrition_blazor_app.Tests;

public sealed class FlavorPieWidgetTests : TestContext
{
    [Fact]
    public void RendersAccessibleLabelAndGradientFromSlices()
    {
        var slices = new[]
        {
            new FlavorSlice("Chocolate", 55, "blue", "chocolate"),
            new FlavorSlice("Vanilla", 27, "lavender", "vanilla"),
            new FlavorSlice("Strawberry", 18, "light", "strawberry")
        };

        var cut = RenderComponent<FlavorPieWidget>(parameters =>
            parameters.Add(component => component.Slices, slices));

        var chart = cut.Find("figure.donut-chart");
        var style = chart.GetAttribute("style");

        Assert.Equal("img", chart.GetAttribute("role"));
        Assert.Equal(
            "Flavor distribution: Chocolate 55 percent, Vanilla 27 percent, Strawberry 18 percent",
            chart.GetAttribute("aria-label"));
        Assert.NotNull(style);
        Assert.Contains("conic-gradient(", style);
        Assert.Contains("var(--blue) 0% 55%", style);
        Assert.Contains("var(--lavender) 55% 82%", style);
        Assert.Contains("var(--chart-light) 82% 100%", style);
    }

    [Fact]
    public void RendersCaptionPerSliceWithExpectedClasses()
    {
        var slices = new[]
        {
            new FlavorSlice("Chocolate", 60, "blue", "chocolate"),
            new FlavorSlice("Vanilla", 25, "lavender", "vanilla"),
            new FlavorSlice("Strawberry", 15, "light", "strawberry")
        };

        var cut = RenderComponent<FlavorPieWidget>(parameters =>
            parameters.Add(component => component.Slices, slices));

        var captions = cut.FindAll("figcaption.slice-label");

        Assert.Equal(3, captions.Count);
        Assert.Contains(captions, c => c.ClassList.Contains("label-chocolate"));
        Assert.Contains(captions, c => c.ClassList.Contains("label-vanilla"));
        Assert.Contains(captions, c => c.ClassList.Contains("label-strawberry"));

        var blueSliceCaption = captions.Single(c => c.ClassList.Contains("label-chocolate"));
        Assert.Contains("slice-light-text", blueSliceCaption.ClassName);
    }
}
