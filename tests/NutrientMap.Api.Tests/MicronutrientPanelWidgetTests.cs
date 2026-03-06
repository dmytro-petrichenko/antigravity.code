using Bunit;
using NutrientMap.Api.Components;
using NutrientMap.Domain.Contracts;

namespace NutrientMap.Api.Tests;

public sealed class MicronutrientPanelWidgetTests : TestContext
{
    [Fact]
    public void RendersSectionItemsAndAccessibleBars()
    {
        var items = new[]
        {
            new NutritionDashboardDto.MicronutrientDto
            {
                NutrientName = "Vitamin A",
                Amount = new NutritionDashboardDto.MeasuredValueDto { Value = 680m, Unit = "mcg RAE" },
                PercentDailyValue = 76m
            },
            new NutritionDashboardDto.MicronutrientDto
            {
                NutrientName = "Vitamin C",
                Amount = new NutritionDashboardDto.MeasuredValueDto { Value = 72m, Unit = "mg" },
                PercentDailyValue = 80m
            }
        };
        var group = new NutritionDashboardDto.MicronutrientGroupDto
        {
            GroupName = "Vitamins",
            Items = items
        };

        var cut = RenderComponent<MicronutrientPanelWidget>(parameters => parameters
            .Add(component => component.Group, group));

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
            new NutritionDashboardDto.MicronutrientDto
            {
                NutrientName = "Demo Mineral",
                Amount = new NutritionDashboardDto.MeasuredValueDto { Value = 123m, Unit = "mg" },
                PercentDailyValue = 62.345m
            }
        };
        var group = new NutritionDashboardDto.MicronutrientGroupDto
        {
            GroupName = "Minerals",
            Items = items
        };

        var cut = RenderComponent<MicronutrientPanelWidget>(parameters => parameters
            .Add(component => component.Group, group));

        var fill = cut.Find("span.micro-fill");

        Assert.Equal("width: 62.35%;", fill.GetAttribute("style"));
    }
}
