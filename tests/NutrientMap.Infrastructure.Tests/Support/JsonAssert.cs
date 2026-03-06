using System.Text.Json;

namespace NutrientMap.Infrastructure.Tests.Support;

internal static class JsonAssert
{
    public static void Equal(string expectedJson, string actualJson)
    {
        using var expectedDocument = JsonDocument.Parse(expectedJson);
        using var actualDocument = JsonDocument.Parse(actualJson);

        Assert.True(
            JsonElement.DeepEquals(expectedDocument.RootElement, actualDocument.RootElement),
            $"Expected JSON:{Environment.NewLine}{expectedDocument.RootElement}{Environment.NewLine}{Environment.NewLine}" +
            $"Actual JSON:{Environment.NewLine}{actualDocument.RootElement}");
    }
}
