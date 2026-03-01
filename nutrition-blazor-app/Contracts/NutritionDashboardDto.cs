namespace nutrition_blazor_app.Contracts;

public sealed class NutritionDashboardDto
{
    public string Title { get; init; } = string.Empty;
    public EnergySummaryDto Energy { get; init; } = new();
    public MacronutrientOverviewDto Macronutrients { get; init; } = new();
    public IReadOnlyList<MacronutrientDetailsDto> MacronutrientDetails { get; init; } = [];
    public MicronutrientGroupDto Vitamins { get; init; } = new() { GroupName = "Vitamins" };
    public MicronutrientGroupDto Minerals { get; init; } = new() { GroupName = "Minerals" };

    public sealed class EnergySummaryDto
    {
        public MeasuredValueDto Calories { get; init; } = new() { Unit = "kcal" };
    }

    public sealed class MacronutrientOverviewDto
    {
        public IReadOnlyList<MacronutrientShareDto> Distribution { get; init; } = [];
    }

    public sealed class MacronutrientShareDto
    {
        public string MacronutrientName { get; init; } = string.Empty;
        public decimal PercentOfCalories { get; init; }
    }

    public sealed class MacronutrientDetailsDto
    {
        public string MacronutrientName { get; init; } = string.Empty;
        public MeasuredValueDto TotalAmount { get; init; } = new();
        public IReadOnlyList<MacronutrientComponentDto> Components { get; init; } = [];
    }

    public sealed class MacronutrientComponentDto
    {
        public string Name { get; init; } = string.Empty;
        public MeasuredValueDto Amount { get; init; } = new();
        public decimal PercentOfParent { get; init; }
    }

    public sealed class MicronutrientGroupDto
    {
        public string GroupName { get; init; } = string.Empty;
        public IReadOnlyList<MicronutrientDto> Items { get; init; } = [];
    }

    public sealed class MicronutrientDto
    {
        public string NutrientName { get; init; } = string.Empty;
        public MeasuredValueDto Amount { get; init; } = new();
        public decimal PercentDailyValue { get; init; }
    }

    public sealed class MeasuredValueDto
    {
        public decimal Value { get; init; }
        public string Unit { get; init; } = string.Empty;
    }
}
