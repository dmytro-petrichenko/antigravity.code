using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;

namespace nutrition_blazor_app.Tests;

public sealed class HomePageBrowserStructureTests
{
    [Fact]
    public async Task HomePage_ExposesExpectedWidgetStructure()
    {
        await using var host = await HomePageKestrelHost.StartAsync();

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = 1440,
                Height = 900
            }
        });
        var page = await context.NewPageAsync();
        var consoleMessages = new ConcurrentQueue<string>();
        var requestFailures = new ConcurrentQueue<string>();
        var errorResponses = new ConcurrentQueue<string>();
        var pageErrors = new ConcurrentQueue<string>();

        page.Console += (_, message) =>
        {
            if (message.Type is "error" or "warning")
            {
                consoleMessages.Enqueue($"{message.Type}: {message.Text}");
            }
        };
        page.PageError += (_, exception) =>
        {
            pageErrors.Enqueue(exception);
        };
        page.RequestFailed += (_, request) =>
        {
            requestFailures.Enqueue($"{request.Method} {request.Url} :: {request.Failure}");
        };
        page.Response += (_, response) =>
        {
            if (response.Status >= 400)
            {
                errorResponses.Enqueue($"{response.Status} {response.Request.Method} {response.Url}");
            }
        };

        await page.GotoAsync(host.BaseUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });
        try
        {
            await page.WaitForSelectorAsync("main.dashboard", new PageWaitForSelectorOptions
            {
                Timeout = 15_000
            });
        }
        catch (TimeoutException ex)
        {
            var html = await page.ContentAsync();
            var diagnostics = string.Join(Environment.NewLine, new[]
            {
                $"URL: {page.Url}",
                $"Console: {string.Join(" | ", consoleMessages)}",
                $"PageErrors: {string.Join(" | ", pageErrors)}",
                $"RequestFailed: {string.Join(" | ", requestFailures)}",
                $"ErrorResponses: {string.Join(" | ", errorResponses)}",
                $"HTML: {html}"
            });

            throw new Xunit.Sdk.XunitException($"Blazor app did not render main.dashboard.{Environment.NewLine}{diagnostics}", ex);
        }

        var dashboard = page.Locator("main.dashboard");
        Assert.Equal(1, await dashboard.CountAsync());
        Assert.Equal("Nutrition dashboard", await dashboard.GetAttributeAsync("aria-label"));
        Assert.Equal("grid", await dashboard.EvaluateAsync<string>("el => getComputedStyle(el).display"));

        var children = page.Locator("main.dashboard > *");
        Assert.Equal(6, await children.CountAsync());

        Assert.True(await children.Nth(0).EvaluateAsync<bool>("el => el.matches('h1.sr-only')"));
        Assert.True(await children.Nth(1).EvaluateAsync<bool>("el => el.matches('section.nutrition-facts-test-panel')"));
        Assert.True(await children.Nth(2).EvaluateAsync<bool>("el => el.matches('section.nutrition-panel')"));
        Assert.True(await children.Nth(3).EvaluateAsync<bool>("el => el.matches('section.macronutrient-pie-panel')"));
        Assert.True(await children.Nth(4).EvaluateAsync<bool>("el => el.matches('div.dashboard-divider')"));
        Assert.True(await children.Nth(5).EvaluateAsync<bool>("el => el.matches('div.micronutrient-row')"));

        Assert.Equal("Nutrition Facts", (await page.Locator("section.nutrition-facts-test-panel h2.nutrition-facts-title").TextContentAsync())?.Trim());
        Assert.Equal(4, await page.Locator(".nutrition-facts-test-panel .nutrition-fact-item").CountAsync());
        Assert.Equal(3, await page.Locator("section.nutrition-panel article.macro-widget").CountAsync());
        Assert.Equal(1, await page.Locator("section.macronutrient-pie-panel figure.macronutrient-pie-widget").CountAsync());
        Assert.Equal(2, await page.Locator("div.micronutrient-row section.micronutrient-panel").CountAsync());

        var micronutrientPanels = page.Locator("div.micronutrient-row section.micronutrient-panel");
        Assert.Equal("Vitamins", await micronutrientPanels.Nth(0).GetAttributeAsync("aria-label"));
        Assert.Equal("Minerals", await micronutrientPanels.Nth(1).GetAttributeAsync("aria-label"));
 
        var donutChartBounds = await page.Locator(".donut-chart").BoundingBoxAsync();
        Assert.NotNull(donutChartBounds);
        Assert.True(donutChartBounds!.Width > 100);
        Assert.True(donutChartBounds.Height > 100);
    }

    private sealed class HomePageKestrelHost : IAsyncDisposable
    {
        private readonly WebApplication _app;

        private HomePageKestrelHost(WebApplication app, string baseUrl)
        {
            _app = app;
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }

        public static async Task<HomePageKestrelHost> StartAsync(CancellationToken cancellationToken = default)
        {
            var port = GetRandomUnusedPort();
            var baseUrl = $"http://127.0.0.1:{port}";

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                EnvironmentName = Environments.Development
            });

            builder.WebHost.UseKestrel();
            builder.WebHost.UseUrls(baseUrl);
            builder.Configuration[WebHostDefaults.StaticWebAssetsKey] = GetStaticWebAssetsManifestPath();
            StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });
            app.MapFallbackToFile("index.html");

            await app.StartAsync(cancellationToken);

            return new HomePageKestrelHost(app, baseUrl);
        }

        public async ValueTask DisposeAsync()
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }

        private static int GetRandomUnusedPort()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static string GetStaticWebAssetsManifestPath()
        {
            var manifestPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "nutrition-blazor-app",
                "bin",
                "Debug",
                "net10.0",
                "nutrition-blazor-app.staticwebassets.runtime.json"));

            if (!File.Exists(manifestPath))
            {
                throw new FileNotFoundException(
                    "Expected Blazor static web assets manifest was not found. Build the app project before running browser tests.",
                    manifestPath);
            }

            return manifestPath;
        }
    }
}
