using nutrition_blazor_app.Models;

namespace nutrition_blazor_app.Services;

public interface INutritionDataService
{
    NutritionDashboardData GetDashboard();
}

public sealed class MockNutritionDataService : INutritionDataService
{
    private static readonly NutritionDashboardData Dashboard = new(
        MacroSections:
        [
            new MacroSection(
                Title: "Carbohydrates",
                TotalAmount: "50g",
                Segments:
                [
                    new NutrientSegment("Sugars", "35g", 30, "green"),
                    new NutrientSegment("Starch", "10g", 37, "blue"),
                    new NutrientSegment("Fiber", "5g", 33, "gray")
                ]),
            new MacroSection(
                Title: "Proteins",
                TotalAmount: "10g",
                Segments:
                [
                    new NutrientSegment("Amino Acid Profile (EAAs)", "35g", 62, "beige"),
                    new NutrientSegment("Remaining", "15g", 38, "gray")
                ]),
            new MacroSection(
                Title: "Fats",
                TotalAmount: "20g",
                Segments:
                [
                    new NutrientSegment("Saturated", "5g", 33, "green"),
                    new NutrientSegment("Monounsaturated", "10g", 30, "blue"),
                    new NutrientSegment("Polyunsaturated", "5g", 37, "gray")
                ])
        ],
        FlavorSlices:
        [
            new FlavorSlice("Chocolate", 55, "blue", "chocolate"),
            new FlavorSlice("Vanilla", 27, "lavender", "vanilla"),
            new FlavorSlice("Strawberry", 18, "light", "strawberry")
        ],
        Vitamins:
        [
            new MicronutrientItem("Vitamin A", "500 mcg ±10% (62% DV)", 62),
            new MicronutrientItem("Vitamin B", "500 mcg ±10% (62% DV)", 58)
        ],
        Minerals:
        [
            new MicronutrientItem("Magnesium", "500 mcg ±10% (62% DV)", 57),
            new MicronutrientItem("Zinc", "260 mcg ±10% (48% DV)", 48)
        ]);

    public NutritionDashboardData GetDashboard() => Dashboard;
}
