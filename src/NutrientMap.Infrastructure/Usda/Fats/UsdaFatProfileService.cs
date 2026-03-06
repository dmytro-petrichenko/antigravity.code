namespace NutrientMap.Infrastructure.Usda.Fats;

public sealed class UsdaFatProfileService
{
    private readonly UsdaFoodHttpService _foodHttpService;
    private readonly FatProfileFilter _fatProfileFilter;
    private readonly FatFilter _fatFilter;

    public UsdaFatProfileService(
        UsdaFoodHttpService foodHttpService,
        FatProfileFilter fatProfileFilter,
        FatFilter? fatFilter = null)
    {
        ArgumentNullException.ThrowIfNull(foodHttpService);
        ArgumentNullException.ThrowIfNull(fatProfileFilter);

        _foodHttpService = foodHttpService;
        _fatProfileFilter = fatProfileFilter;
        _fatFilter = fatFilter ?? FatFilter.CreateDefault();
    }

    public async Task<FatProfile> GetFatProfileOfProductByFdcIdAsync(
        int fdcId,
        CancellationToken cancellationToken = default)
    {
        var dto = await _foodHttpService.GetFoodByFdcIdAsync(fdcId, cancellationToken).ConfigureAwait(false);
        if (dto is null)
        {
            throw new InvalidOperationException(
                $"USDA returned no food for FDC ID '{fdcId}'.");
        }

        return _fatProfileFilter.ApplyFilter(_fatFilter, dto);
    }
}
