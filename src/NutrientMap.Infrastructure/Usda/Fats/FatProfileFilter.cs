namespace NutrientMap.Infrastructure.Usda.Fats;

public sealed class FatProfileFilter
{
    public FatProfile ApplyFilter(FatFilter filter, UsdaAbridgedFoodResponseDto dto)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(dto);

        return new FatProfile
        {
            TotalFat = GetFirstMatchingAmount(dto.FoodNutrients, filter.TotalFatNumbers),
            SaturatedFat = GetFirstMatchingAmount(dto.FoodNutrients, filter.SaturatedFatNumbers),
            MonounsaturatedFat = GetFirstMatchingAmount(dto.FoodNutrients, filter.MonounsaturatedFatNumbers),
            PolyunsaturatedFat = GetFirstMatchingAmount(dto.FoodNutrients, filter.PolyunsaturatedFatNumbers),
            TransFat = GetTransFat(dto.FoodNutrients, filter)
        };
    }

    private static float GetFirstMatchingAmount(
        IReadOnlyList<UsdaAbridgedFoodNutrientDto> nutrients,
        IReadOnlyList<string> nutrientNumbers)
    {
        foreach (var nutrientNumber in nutrientNumbers)
        {
            var nutrient = FindByNumber(nutrients, nutrientNumber);
            if (nutrient is not null)
            {
                return (float)(nutrient.Amount ?? 0d);
            }
        }

        return 0f;
    }

    private static float GetTransFat(IReadOnlyList<UsdaAbridgedFoodNutrientDto> nutrients, FatFilter filter)
    {
        var directTransFat = GetFirstMatchingNutrient(nutrients, filter.TransFatNumbers);
        if (directTransFat is not null)
        {
            return (float)(directTransFat.Amount ?? 0d);
        }

        double transFatFallback = 0d;
        var foundAnyComponent = false;

        foreach (var nutrientNumber in filter.TransFatFallbackComponentNumbers)
        {
            var nutrient = FindByNumber(nutrients, nutrientNumber);
            if (nutrient is null)
            {
                continue;
            }

            transFatFallback += nutrient.Amount ?? 0d;
            foundAnyComponent = true;
        }

        return foundAnyComponent ? (float)transFatFallback : 0f;
    }

    private static UsdaAbridgedFoodNutrientDto? GetFirstMatchingNutrient(
        IReadOnlyList<UsdaAbridgedFoodNutrientDto> nutrients,
        IReadOnlyList<string> nutrientNumbers)
    {
        foreach (var nutrientNumber in nutrientNumbers)
        {
            var nutrient = FindByNumber(nutrients, nutrientNumber);
            if (nutrient is not null)
            {
                return nutrient;
            }
        }

        return null;
    }

    private static UsdaAbridgedFoodNutrientDto? FindByNumber(
        IReadOnlyList<UsdaAbridgedFoodNutrientDto> nutrients,
        string nutrientNumber)
    {
        foreach (var nutrient in nutrients)
        {
            if (string.Equals(nutrient.Number, nutrientNumber, StringComparison.Ordinal))
            {
                return nutrient;
            }
        }

        return null;
    }
}
