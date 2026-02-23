using Bunit;
using nutrition_blazor_app.Components;

namespace nutrition_blazor_app.Tests;

public sealed class NutritionFactsTestWidgetTests : TestContext
{
    [Fact]
    public void RendersSectionHeaderAndAccessibleLabel()
    {
        var cut = RenderComponent<NutritionFactsTestWidget>();

        var panel = cut.Find("section.nutrition-facts-test-panel");
        var heading = cut.Find("h2.nutrition-facts-title");

        Assert.Equal("Nutrition facts test section", panel.GetAttribute("aria-label"));
        Assert.Equal("Nutrition Facts", heading.TextContent.Trim());
        Assert.Empty(cut.FindAll("p.nutrition-facts-note"));
    }

    [Fact]
    public void RendersFourNutritionFactEntriesWithExpectedValues()
    {
        var cut = RenderComponent<NutritionFactsTestWidget>();

        var items = cut.FindAll(".nutrition-fact-item");
        var labels = cut.FindAll("dt.nutrition-fact-label").Select(x => x.TextContent.Trim()).ToArray();
        var values = cut.FindAll("dd.nutrition-fact-value").Select(x => x.TextContent.Trim()).ToArray();

        Assert.Equal(4, items.Count);
        Assert.Equal(["Product Name", "Portion", "Calories", "Energy"], labels);
        Assert.Equal(["Rice", "100 g", "130 kcal", "544 kJ"], values);
    }
}
