using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace NutrientMap.Infrastructure.Usda;

public sealed class UsdaFoodHttpService
{
    private const string AbridgedFormat = "abridged";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly Uri _foodsEndpoint;

    public UsdaFoodHttpService(HttpClient httpClient, UsdaFoodApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            throw new ArgumentException("BaseUrl must be provided.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException("ApiKey must be provided.", nameof(options));
        }

        _httpClient = httpClient;
        _foodsEndpoint = BuildFoodsEndpoint(options.BaseUrl, options.ApiKey);
    }

    public async Task<UsdaAbridgedFoodResponseDto?> GetFoodByFdcIdAsync(
        int fdcId,
        CancellationToken cancellationToken = default)
    {
        if (fdcId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fdcId), fdcId, "FdcId must be positive.");
        }

        var payload = new UsdaFoodsRequestDto
        {
            FdcIds = [fdcId],
            Format = AbridgedFormat
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _foodsEndpoint)
        {
            Content = JsonContent.Create(payload, options: JsonOptions)
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var foods = await response.Content
            .ReadFromJsonAsync<List<UsdaAbridgedFoodResponseDto>>(JsonOptions, cancellationToken)
            .ConfigureAwait(false) ?? [];

        return foods.Count switch
        {
            0 => null,
            1 => foods[0],
            _ => throw new InvalidOperationException(
                "Expected a single food item response for a single FDC ID request.")
        };
    }

    private static Uri BuildFoodsEndpoint(string baseUrl, string apiKey)
    {
        var trimmedBaseUrl = baseUrl.TrimEnd('/');
        var escapedApiKey = Uri.EscapeDataString(apiKey);

        return new Uri($"{trimmedBaseUrl}/foods?api_key={escapedApiKey}", UriKind.Absolute);
    }
}
