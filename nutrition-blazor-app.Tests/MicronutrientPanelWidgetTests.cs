using Bunit;
using nutrition_blazor_app.Components;
using nutrition_blazor_app.Models;

namespace nutrition_blazor_app.Tests;

public sealed class MicronutrientPanelWidgetTests : TestContext
{
    [Fact]
    public void RendersSectionItemsAndAccessibleBars()
    {
        var items = new[]
        {
            new MicronutrientItem("Vitamin A", "680 mcg RAE (76% DV)", 76),
            new MicronutrientItem("Vitamin C", "72 mg (80% DV)", 80)
        };

        var cut = RenderComponent<MicronutrientPanelWidget>(parameters => parameters
            .Add(component => component.Title, "Vitamins")
            .Add(component => component.Items, items));

        var panel = cut.Find("section.micronutrient-panel");
        var entries = cut.FindAll("article.micro-item");
        var bars = cut.FindAll("div.micro-bar");
        var fills = cut.FindAll("span.micro-fill");

        Assert.Equal("Vitamins", panel.GetAttribute("aria-label"));
        Assert.Equal(2, entries.Count);
        Assert.Equal(2, bars.Count);
        Assert.Equal(2, fills.Count);

        Assert.Equal("Vitamin A", entries[0].QuerySelector("h2")!.TextContent.Trim());
        Assert.Equal("680 mcg RAE (76% DV)", entries[0].QuerySelector("p")!.TextContent.Trim());
        Assert.Equal("Vitamin A: 680 mcg RAE (76% DV)", bars[0].GetAttribute("aria-label"));
        Assert.Equal("width: 76%;", fills[0].GetAttribute("style"));
    }

    [Fact]
    public void WidthStyleRoundsToTwoDecimalPlacesMax()
    {
        var items = new[]
        {
            new MicronutrientItem("Demo Mineral", "123 mg (62.35% DV)", 62.345)
        };

        var cut = RenderComponent<MicronutrientPanelWidget>(parameters => parameters
            .Add(component => component.Title, "Minerals")
            .Add(component => component.Items, items));

        var fill = cut.Find("span.micro-fill");

        Assert.Equal("width: 62.35%;", fill.GetAttribute("style"));
    }
}
