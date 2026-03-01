using nutrition_blazor_app.Contracts;

namespace nutrition_blazor_app.Services;

public interface INutritionDataService
{
    NutritionDashboardDto GetDashboard();
}

public sealed class MockNutritionDataService : INutritionDataService
{
    private static readonly NutritionDashboardDto Dashboard = CreateDashboard();

    public NutritionDashboardDto GetDashboard() => Dashboard;

    private static NutritionDashboardDto CreateDashboard()
    {
        return new NutritionDashboardDto
        {
            Title = "Rice",
            Energy = new NutritionDashboardDto.EnergySummaryDto
            {
                Calories = Value(130m, "kcal")
            },
            Macronutrients = new NutritionDashboardDto.MacronutrientOverviewDto
            {
                Distribution =
                [
                    Share("Carbohydrates", 63m),
                    Share("Fats", 25m),
                    Share("Proteins", 12m)
                ]
            },
            MacronutrientDetails =
            [
                Macronutrient(
                    macronutrientName: "Carbohydrates",
                    totalValue: 50m,
                    totalUnit: "g",
                    components:
                    [
                        Component("Sugars", 35m, "g", 30m),
                        Component("Starch", 10m, "g", 40m),
                        Component("Fiber", 5m, "g", 30m)
                    ]),
                Macronutrient(
                    macronutrientName: "Proteins",
                    totalValue: 10m,
                    totalUnit: "g",
                    components:
                    [
                        Component("Amino Acid Profile (EAAs)", 35m, "g", 70m),
                        Component("Remaining", 15m, "g", 30m)
                    ]),
                Macronutrient(
                    macronutrientName: "Fats",
                    totalValue: 20m,
                    totalUnit: "g",
                    components:
                    [
                        Component("Saturated", 5m, "g", 33m),
                        Component("Monounsaturated", 10m, "g", 30m),
                        Component("Polyunsaturated", 5m, "g", 37m)
                    ])
            ],
            Vitamins = new NutritionDashboardDto.MicronutrientGroupDto
            {
                GroupName = "Vitamins",
                Items =
                [
                    Micronutrient("Vitamin A", 680m, "mcg RAE", 76m),
                    Micronutrient("Vitamin C", 72m, "mg", 80m),
                    Micronutrient("Vitamin D", 12m, "mcg", 60m),
                    Micronutrient("Vitamin E", 9m, "mg", 60m),
                    Micronutrient("Vitamin K", 58m, "mcg", 48m),
                    Micronutrient("Vitamin B1 (Thiamin)", 0.9m, "mg", 75m),
                    Micronutrient("Vitamin B2 (Riboflavin)", 1.0m, "mg", 77m),
                    Micronutrient("Vitamin B3 (Niacin)", 11m, "mg", 69m),
                    Micronutrient("Vitamin B5 (Pantothenic Acid)", 3.8m, "mg", 76m),
                    Micronutrient("Vitamin B6", 1.1m, "mg", 65m),
                    Micronutrient("Vitamin B7 (Biotin)", 26m, "mcg", 87m),
                    Micronutrient("Vitamin B9 (Folate)", 310m, "mcg DFE", 78m),
                    Micronutrient("Vitamin B12", 2.0m, "mcg", 83m)
                ]
            },
            Minerals = new NutritionDashboardDto.MicronutrientGroupDto
            {
                GroupName = "Minerals",
                Items =
                [
                    Micronutrient("Calcium", 420m, "mg", 32m),
                    Micronutrient("Phosphorus", 560m, "mg", 45m),
                    Micronutrient("Magnesium", 210m, "mg", 50m),
                    Micronutrient("Potassium", 1200m, "mg", 26m),
                    Micronutrient("Sodium", 430m, "mg", 19m),
                    Micronutrient("Chloride", 780m, "mg", 34m),
                    Micronutrient("Sulfur", 180m, "mg", 44m),
                    Micronutrient("Iron", 11m, "mg", 61m),
                    Micronutrient("Zinc", 8.4m, "mg", 76m),
                    Micronutrient("Copper", 0.7m, "mg", 78m),
                    Micronutrient("Manganese", 1.5m, "mg", 65m),
                    Micronutrient("Selenium", 38m, "mcg", 69m),
                    Micronutrient("Iodine", 96m, "mcg", 64m),
                    Micronutrient("Chromium", 28m, "mcg", 80m),
                    Micronutrient("Molybdenum", 36m, "mcg", 80m),
                    Micronutrient("Fluoride", 2.1m, "mg", 53m)
                ]
            }
        };
    }

    private static NutritionDashboardDto.MacronutrientShareDto Share(string name, decimal percentOfCalories)
        => new()
        {
            MacronutrientName = name,
            PercentOfCalories = percentOfCalories
        };

    private static NutritionDashboardDto.MacronutrientDetailsDto Macronutrient(
        string macronutrientName,
        decimal totalValue,
        string totalUnit,
        IReadOnlyList<NutritionDashboardDto.MacronutrientComponentDto> components)
        => new()
        {
            MacronutrientName = macronutrientName,
            TotalAmount = Value(totalValue, totalUnit),
            Components = components
        };

    private static NutritionDashboardDto.MacronutrientComponentDto Component(
        string name,
        decimal value,
        string unit,
        decimal percentOfParent)
        => new()
        {
            Name = name,
            Amount = Value(value, unit),
            PercentOfParent = percentOfParent
        };

    private static NutritionDashboardDto.MicronutrientDto Micronutrient(
        string name,
        decimal value,
        string unit,
        decimal percentDailyValue)
        => new()
        {
            NutrientName = name,
            Amount = Value(value, unit),
            PercentDailyValue = percentDailyValue
        };

    private static NutritionDashboardDto.MeasuredValueDto Value(decimal value, string unit)
        => new()
        {
            Value = value,
            Unit = unit
        };
}
