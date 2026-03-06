namespace NutrientMap.Infrastructure.Tests.Support;

internal static class FixtureLoader
{
    public static string Load(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", fileName);
        return File.ReadAllText(path);
    }
}
