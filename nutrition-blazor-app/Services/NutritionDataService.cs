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
                    new NutrientSegment("Sugars", "35g", 30, NutrientColor.Green),
                    new NutrientSegment("Starch", "10g", 40, NutrientColor.Blue),
                    new NutrientSegment("Fiber", "5g", 30, NutrientColor.Gray)
                ]),
            new MacroSection(
                Title: "Proteins",
                TotalAmount: "10g",
                Segments:
                [
                    new NutrientSegment("Amino Acid Profile (EAAs)", "35g", 70, NutrientColor.Beige),
                    new NutrientSegment("Remaining", "15g", 30, NutrientColor.Gray, ShowInLegend: false)
                ]),
            new MacroSection(
                Title: "Fats",
                TotalAmount: "20g",
                Segments:
                [
                    new NutrientSegment("Saturated", "5g", 33, NutrientColor.Green),
                    new NutrientSegment("Monounsaturated", "10g", 30, NutrientColor.Blue),
                    new NutrientSegment("Polyunsaturated", "5g", 37, NutrientColor.Gray)
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
            new MicronutrientItem("Vitamin A", "680 mcg RAE (76% DV)", 76),
            new MicronutrientItem("Vitamin C", "72 mg (80% DV)", 80),
            new MicronutrientItem("Vitamin D", "12 mcg (60% DV)", 60),
            new MicronutrientItem("Vitamin E", "9 mg (60% DV)", 60),
            new MicronutrientItem("Vitamin K", "58 mcg (48% DV)", 48),
            new MicronutrientItem("Vitamin B1 (Thiamin)", "0.9 mg (75% DV)", 75),
            new MicronutrientItem("Vitamin B2 (Riboflavin)", "1.0 mg (77% DV)", 77),
            new MicronutrientItem("Vitamin B3 (Niacin)", "11 mg (69% DV)", 69),
            new MicronutrientItem("Vitamin B5 (Pantothenic Acid)", "3.8 mg (76% DV)", 76),
            new MicronutrientItem("Vitamin B6", "1.1 mg (65% DV)", 65),
            new MicronutrientItem("Vitamin B7 (Biotin)", "26 mcg (87% DV)", 87),
            new MicronutrientItem("Vitamin B9 (Folate)", "310 mcg DFE (78% DV)", 78),
            new MicronutrientItem("Vitamin B12", "2.0 mcg (83% DV)", 83)
        ],
        Minerals:
        [
            new MicronutrientItem("Calcium", "420 mg (32% DV)", 32),
            new MicronutrientItem("Phosphorus", "560 mg (45% DV)", 45),
            new MicronutrientItem("Magnesium", "210 mg (50% DV)", 50),
            new MicronutrientItem("Potassium", "1200 mg (26% DV)", 26),
            new MicronutrientItem("Sodium", "430 mg (19% DV)", 19),
            new MicronutrientItem("Chloride", "780 mg (34% DV)", 34),
            new MicronutrientItem("Sulfur", "180 mg (demo target 44%)", 44),
            new MicronutrientItem("Iron", "11 mg (61% DV)", 61),
            new MicronutrientItem("Zinc", "8.4 mg (76% DV)", 76),
            new MicronutrientItem("Copper", "0.7 mg (78% DV)", 78),
            new MicronutrientItem("Manganese", "1.5 mg (65% DV)", 65),
            new MicronutrientItem("Selenium", "38 mcg (69% DV)", 69),
            new MicronutrientItem("Iodine", "96 mcg (64% DV)", 64),
            new MicronutrientItem("Chromium", "28 mcg (80% DV)", 80),
            new MicronutrientItem("Molybdenum", "36 mcg (80% DV)", 80),
            new MicronutrientItem("Fluoride", "2.1 mg (53% DV)", 53)
        ]);

    public NutritionDashboardData GetDashboard() => Dashboard;
}
