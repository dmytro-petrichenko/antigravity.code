using System.Net;
using System.Text;

namespace NutrientMap.Infrastructure.Tests.Support;

internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

    public RecordingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public HttpMethod? LastMethod { get; private set; }
    public Uri? LastRequestUri { get; private set; }
    public string? LastRequestBody { get; private set; }
    public string? LastAcceptHeader { get; private set; }
    public int RequestCount { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        RequestCount++;
        LastMethod = request.Method;
        LastRequestUri = request.RequestUri;
        LastAcceptHeader = request.Headers.Accept.SingleOrDefault()?.MediaType;
        LastRequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return _responseFactory(request);
    }

    public static HttpResponseMessage JsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
}
