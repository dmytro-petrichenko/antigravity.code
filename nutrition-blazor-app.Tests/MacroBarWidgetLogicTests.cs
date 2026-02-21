using nutrition_blazor_app.Components;
using nutrition_blazor_app.Models;

namespace nutrition_blazor_app.Tests;

public class MacroBarWidgetLogicTests
{
    [Theory]
    [InlineData(NutrientColor.Green, "segment-green", "swatch-green")]
    [InlineData(NutrientColor.Blue, "segment-blue", "swatch-blue")]
    [InlineData(NutrientColor.Gray, "segment-gray", "swatch-gray")]
    [InlineData(NutrientColor.Beige, "segment-beige", "swatch-beige")]
    public void ColorClassMapping_ReturnsExpectedCssClasses(
        NutrientColor color,
        string expectedSegmentClass,
        string expectedSwatchClass)
    {
        var segmentClass = MacroBarWidgetLogic.GetSegmentCssClass(color);
        var swatchClass = MacroBarWidgetLogic.GetSwatchCssClass(color);

        Assert.Equal(expectedSegmentClass, segmentClass);
        Assert.Equal(expectedSwatchClass, swatchClass);
    }

    [Fact]
    public void ValidateSections_AllowsTwoSectionsSummingToHundred()
    {
        var sections = new[]
        {
            new NutrientSegment("A", "10g", 10, NutrientColor.Green),
            new NutrientSegment("B", "90g", 90, NutrientColor.Gray)
        };

        MacroBarWidgetLogic.ValidateSections(sections);
    }

    [Fact]
    public void ValidateSections_AllowsThreeSectionsSummingToHundred()
    {
        var sections = new[]
        {
            new NutrientSegment("A", "30g", 30, NutrientColor.Green),
            new NutrientSegment("B", "40g", 40, NutrientColor.Blue),
            new NutrientSegment("C", "30g", 30, NutrientColor.Gray)
        };

        MacroBarWidgetLogic.ValidateSections(sections);
    }

    [Fact]
    public void ValidateSections_ThrowsWhenSectionCountOutsideAllowedRange()
    {
        var singleSection = new[]
        {
            new NutrientSegment("A", "100g", 100, NutrientColor.Green)
        };

        var tooManySections = new[]
        {
            new NutrientSegment("A", "25g", 25, NutrientColor.Green),
            new NutrientSegment("B", "25g", 25, NutrientColor.Blue),
            new NutrientSegment("C", "25g", 25, NutrientColor.Gray),
            new NutrientSegment("D", "25g", 25, NutrientColor.Beige)
        };

        Assert.Throws<InvalidOperationException>(() => MacroBarWidgetLogic.ValidateSections(singleSection));
        Assert.Throws<InvalidOperationException>(() => MacroBarWidgetLogic.ValidateSections(tooManySections));
    }

    [Fact]
    public void ValidateSections_ThrowsWhenTotalPercentageIsNotHundred()
    {
        var sections = new[]
        {
            new NutrientSegment("A", "35g", 35, NutrientColor.Green),
            new NutrientSegment("B", "35g", 35, NutrientColor.Blue),
            new NutrientSegment("C", "20g", 20, NutrientColor.Gray)
        };

        Assert.Throws<InvalidOperationException>(() => MacroBarWidgetLogic.ValidateSections(sections));
    }
}
