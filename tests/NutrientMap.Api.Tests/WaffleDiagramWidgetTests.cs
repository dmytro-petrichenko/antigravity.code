using Bunit;
using NutrientMap.Api.Components;

namespace NutrientMap.Api.Tests;

public sealed class WaffleDiagramWidgetTests : TestContext
{
    [Fact]
    public void RendersHundredCellsLegendAndAccessibleSummaryFromSegments()
    {
        var segments = BuildSegments();

        var cut = RenderComponent<WaffleDiagramWidget>(parameters =>
            parameters.Add(component => component.Segments, segments));

        var grid = cut.Find("div.waffle-grid");
        var cells = cut.FindAll("span.waffle-cell");
        var legendItems = cut.FindAll("li.waffle-legend-item");

        Assert.Equal("img", grid.GetAttribute("role"));
        Assert.Equal(
            "Waffle diagram: Saturated fats 33 cells, Monounsaturated fats 34 cells, Polyunsaturated fats 20 cells, Trans fats 13 cells",
            grid.GetAttribute("aria-label"));
        Assert.Equal(100, cells.Count);
        Assert.Equal(4, legendItems.Count);
        Assert.Equal(
            ["Saturated fats", "Monounsaturated fats", "Polyunsaturated fats", "Trans fats"],
            legendItems.Select(item => item.TextContent.Trim()).ToArray());
    }

    [Fact]
    public void UsesDtoColorsInCellOrderAndLegendSwatches()
    {
        var segments = BuildSegments();

        var cut = RenderComponent<WaffleDiagramWidget>(parameters =>
            parameters.Add(component => component.Segments, segments));

        var cells = cut.FindAll("span.waffle-cell");
        var swatches = cut.FindAll("span.waffle-legend-swatch");

        Assert.All(cells.Take(33), cell => Assert.Equal("background: #4CAF50;", cell.GetAttribute("style")));
        Assert.All(cells.Skip(33).Take(34), cell => Assert.Equal("background: #2196F3;", cell.GetAttribute("style")));
        Assert.All(cells.Skip(67).Take(20), cell => Assert.Equal("background: #FFFFFF;", cell.GetAttribute("style")));
        Assert.All(cells.Skip(87).Take(13), cell => Assert.Equal("background: #FFC107;", cell.GetAttribute("style")));

        Assert.Equal("background: #4CAF50;", swatches[0].GetAttribute("style"));
        Assert.Equal("background: #2196F3;", swatches[1].GetAttribute("style"));
        Assert.Equal("background: #FFFFFF;", swatches[2].GetAttribute("style"));
        Assert.Equal("background: #FFC107;", swatches[3].GetAttribute("style"));
    }

    [Fact]
    public void AppliesContrastClassToWhiteCellsAndSwatches()
    {
        var segments = BuildSegments();

        var cut = RenderComponent<WaffleDiagramWidget>(parameters =>
            parameters.Add(component => component.Segments, segments));

        var whiteCell = cut.FindAll("span.waffle-cell")[67];
        var whiteSwatch = cut.FindAll("span.waffle-legend-swatch")[2];

        Assert.Contains("waffle-cell-contrast", whiteCell.ClassList);
        Assert.Contains("waffle-legend-swatch-contrast", whiteSwatch.ClassList);
    }

    [Fact]
    public void RejectsTotalsOtherThanOneHundred()
    {
        var invalidSegments = new[]
        {
            new WaffleDiagramSegmentDto { CellCount = 50, Color = "#4CAF50", Label = "A" },
            new WaffleDiagramSegmentDto { CellCount = 49, Color = "#2196F3", Label = "B" }
        };

        var exception = Assert.Throws<InvalidOperationException>(() =>
            RenderComponent<WaffleDiagramWidget>(parameters =>
                parameters.Add(component => component.Segments, invalidSegments)));

        Assert.Equal("Waffle diagram requires exactly 100 cells but received 99.", exception.Message);
    }

    private static IReadOnlyList<WaffleDiagramSegmentDto> BuildSegments()
        =>
        [
            new() { CellCount = 33, Color = "#4CAF50", Label = "Saturated fats" },
            new() { CellCount = 34, Color = "#2196F3", Label = "Monounsaturated fats" },
            new() { CellCount = 20, Color = "#FFFFFF", Label = "Polyunsaturated fats" },
            new() { CellCount = 13, Color = "#FFC107", Label = "Trans fats" }
        ];
}
