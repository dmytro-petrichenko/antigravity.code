using NutrientMap.Api.Components;
using NutrientMap.Domain.Contracts;

namespace NutrientMap.Api.Tests;

public class MacroBarWidgetLogicTests
{
    [Theory]
    [InlineData(0, "segment-green", "swatch-green")]
    [InlineData(1, "segment-blue", "swatch-blue")]
    [InlineData(2, "segment-beige", "swatch-beige")]
    [InlineData(3, "segment-beige", "swatch-beige")]
    public void ColorClassMapping_ReturnsExpectedCssClasses(
        int componentIndex,
        string expectedSegmentClass,
        string expectedSwatchClass)
    {
        var colorKey = MacroBarWidgetLogic.GetColorKey(componentIndex);
        var segmentClass = MacroBarWidgetLogic.GetSegmentCssClass(colorKey);
        var swatchClass = MacroBarWidgetLogic.GetSwatchCssClass(colorKey);

        Assert.Equal(expectedSegmentClass, segmentClass);
        Assert.Equal(expectedSwatchClass, swatchClass);
    }

    [Fact]
    public void ValidateComponents_AllowsTwoSectionsSummingToHundred()
    {
        var components = new[]
        {
            Component("A", 10m),
            Component("B", 90m)
        };

        MacroBarWidgetLogic.ValidateComponents(components);
    }

    [Fact]
    public void ValidateComponents_AllowsThreeSectionsSummingToHundred()
    {
        var components = new[]
        {
            Component("A", 30m),
            Component("B", 40m),
            Component("C", 30m)
        };

        MacroBarWidgetLogic.ValidateComponents(components);
    }

    [Fact]
    public void ValidateComponents_ThrowsWhenSectionCountOutsideAllowedRange()
    {
        var singleComponent = new[]
        {
            Component("A", 100m)
        };

        var tooManyComponents = new[]
        {
            Component("A", 25m),
            Component("B", 25m),
            Component("C", 25m),
            Component("D", 25m)
        };

        Assert.Throws<InvalidOperationException>(() => MacroBarWidgetLogic.ValidateComponents(singleComponent));
        Assert.Throws<InvalidOperationException>(() => MacroBarWidgetLogic.ValidateComponents(tooManyComponents));
    }

    [Fact]
    public void ValidateComponents_ThrowsWhenTotalPercentageIsNotHundred()
    {
        var components = new[]
        {
            Component("A", 35m),
            Component("B", 35m),
            Component("C", 20m)
        };

        Assert.Throws<InvalidOperationException>(() => MacroBarWidgetLogic.ValidateComponents(components));
    }

    private static NutritionDashboardDto.MacronutrientComponentDto Component(string name, decimal percentOfParent)
        => new()
        {
            Name = name,
            Amount = new NutritionDashboardDto.MeasuredValueDto
            {
                Value = 1m,
                Unit = "g"
            },
            PercentOfParent = percentOfParent
        };
}
