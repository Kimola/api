using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kimola.Api;

/// <summary>
/// Provides configuration options for initializing a <see cref="KimolaClient"/> instance.
/// </summary>
/// <remarks>
/// This class contains settings such as the API key, base URL, and a custom <see cref="HttpClient"/>.
/// </remarks>
public sealed class KimolaClientOptions
{
    /// <summary>
    /// The API key used to authenticate requests to the Kimola API.
    /// </summary>
    /// <remarks>
    /// This property is required. Requests will fail if a valid API key is not provided.
    /// </remarks>
    public required string ApiKey { get; init; }

    /// <summary>
    /// The base URL of the Kimola API. Defaults to <c>https://api.kimola.com/v1</c>.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.kimola.com/v1";

    /// <summary>
    /// An optional custom <see cref="HttpClient"/> instance to use for sending requests.
    /// </summary>
    /// <remarks>
    /// If this is not provided, a new <see cref="HttpClient"/> will be created and managed internally.
    /// </remarks>
    public HttpClient? HttpClient { get; init; }
}

/// <summary>
/// Entry point for interacting with the Kimola API using a shared <see cref="HttpClient"/>.
/// </summary>
/// <remarks>
/// This client configures Bearer authentication (<c>Authorization: Bearer &lt;apiKey&gt;</c>) and JSON
/// serialization, and exposes typed resource clients:
/// <list type="bullet">
/// <item>
/// <description><see cref="Presets"/> — catalog of pretrained models (list, get by key, labels, predictions).</description>
/// </item>
/// <item>
/// <description><see cref="Queries"/> — query-consumption history and aggregated statistics (UTC date range, pagination).</description>
/// </item>
/// <item>
/// <description><see cref="Subscription"/> — current or historical subscription usage (links, models, queries, keywords).</description>
/// </item>
/// </list>
/// Dates are treated as UTC (ISO-8601 <c>...Z</c>). Pagination defaults to <c>pageIndex=0</c>, <c>pageSize=10</c>
/// (capped at 10) where applicable.
/// </remarks>
public sealed class KimolaClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly bool _ownsHttp;

    /// <summary>
    /// The JSON serializer options used across resource clients (web defaults, case-insensitive, ignore nulls).
    /// </summary>
    internal readonly JsonSerializerOptions Json;

    /// <summary>
    /// Operations for Kimola Presets (pretrained AI models): list, get by key, fetch labels,
    /// and run predictions (standard or aspect-based).
    /// </summary>
    /// <remarks>
    /// For predictions, the API accepts a raw JSON string as the request body (<c>"text"</c> content),
    /// optional <c>language</c> (ISO-639-1), and <c>aspectBased</c> mode. Aspect-based predictions return
    /// per-aspect sentiments; standard predictions return a dominant label with probability.
    /// </remarks>
    public PresetsClient Presets { get; }

    /// <summary>
    /// Operations for inspecting Query consumption: paginated activity and aggregated statistics by category
    /// (e.g., Classification, Tracking, Scraping). Supports optional UTC date filtering.
    /// </summary>
    public QueriesClient Queries { get; }

    /// <summary>
    /// Operations for retrieving subscription usage (counts, limits, remaining, percentages) for resources such as
    /// Links, Models, Queries, and Keywords. Supports an optional UTC <c>date</c> to access past periods.
    /// </summary>
    public SubscriptionClient Subscription { get; }

    /// <summary>
    /// Initializes a new <see cref="KimolaClient"/> with the provided options (API key, base URL, optional <see cref="HttpClient"/>).
    /// </summary>
    /// <param name="options">
    /// Configuration for API access. <see cref="KimolaClientOptions.ApiKey"/> is required; <see cref="KimolaClientOptions.BaseUrl"/>
    /// defaults to <c>https://api.kimola.com/v1</c>. If <see cref="KimolaClientOptions.HttpClient"/> is omitted,
    /// the client manages its own <see cref="HttpClient"/> lifetime.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <see cref="KimolaClientOptions.ApiKey"/> is null, empty, or whitespace.
    /// </exception>
    public KimolaClient(KimolaClientOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException("ApiKey is required.", nameof(options));

        _ownsHttp = options.HttpClient is null;
        _http = options.HttpClient ?? new HttpClient();
        _http.BaseAddress = new Uri(AppendSlash(options.BaseUrl));
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        _http.DefaultRequestHeaders.Accept.ParseAdd("application/json");

        Json = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        Presets = new PresetsClient(_http, Json);
        Queries = new QueriesClient(_http, Json);
        Subscription = new SubscriptionClient(_http, Json);
    }

    /// <summary>
    /// Disposes the underlying <see cref="HttpClient"/> instance if it is owned by this class.
    /// </summary>
    public void Dispose()
    {
        if (_ownsHttp) 
            _http.Dispose();
    }

    /// <summary>
    /// Ensures the configured base URL ends with a trailing slash to form valid relative request URIs.
    /// </summary>
    /// <param name="s">The base URL.</param>
    /// <returns>The base URL with a trailing slash.</returns>
    private static string AppendSlash(string s) => s.EndsWith("/") ? s : s + "/";
}
