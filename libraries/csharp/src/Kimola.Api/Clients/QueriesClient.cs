using System.Text.Json;
using static Kimola.Api.HttpReadHelpers;

namespace Kimola.Api;

/// <summary>
/// Initializes a new instance of the <see cref="QueriesClient"/> class, which provides methods to interact 
/// with the Kimola API for retrieving query data and statistics.
/// </summary>
/// <param name="http">
/// The <see cref="HttpClient"/> instance used to send HTTP requests to the Kimola API. 
/// This should be configured with the base URL and any required authentication headers.
/// </param>
/// <param name="json">
/// The <see cref="JsonSerializerOptions"/> used to control JSON serialization and deserialization 
/// behavior when processing API responses.
/// </param>
public sealed class QueriesClient(HttpClient http, JsonSerializerOptions json)
{
    /// <summary>
    /// Retrieves a paginated list of query items from the Kimola API, optionally filtered by date range.
    /// </summary>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve. Defaults to 0.
    /// </param>
    /// <param name="pageSize">
    /// The number of items to include in each page. Defaults to 10.
    /// </param>
    /// <param name="startDateUtc">
    /// Optional start date (UTC). When provided, only queries created on or after this date will be included.
    /// </param>
    /// <param name="endDateUtc">
    /// Optional end date (UTC). When provided, only queries created on or before this date will be included.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list of <see cref="QueryItem"/> objects.
    /// </returns>
    public async Task<IReadOnlyList<QueryItem>> GetAsync(int pageIndex = 0, int pageSize = 10,
        DateTimeOffset? startDateUtc = null, DateTimeOffset? endDateUtc = null, CancellationToken ct = default)
    {
        var qs = new QueryStringBuilder()
            .Add("pageIndex", pageIndex)
            .Add("pageSize", pageSize)
            .AddIfNotEmpty("startDate", startDateUtc?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"))
            .AddIfNotEmpty("endDate",   endDateUtc?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"))
            .Build();
        return await Send<IReadOnlyList<QueryItem>>(HttpMethod.Get, $"queries{qs}", null, ct);
    }
    
    /// <summary>
    /// Retrieves aggregated query statistics from the Kimola API, optionally filtered by a date range.
    /// </summary>
    /// <param name="startDateUtc">
    /// Optional start date (UTC). When provided, only statistics for queries created on or after this date will be included.
    /// </param>
    /// <param name="endDateUtc">
    /// Optional end date (UTC). When provided, only statistics for queries created on or before this date will be included.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a read-only list of <see cref="QueryStat"/> objects
    /// representing aggregated statistics about the queries.
    /// </returns>
    public async Task<IReadOnlyList<QueryStat>> GetStatisticsAsync(DateTimeOffset? startDateUtc = null, DateTimeOffset? endDateUtc = null, CancellationToken ct = default)
    {
        var qs = new QueryStringBuilder()
            .AddIfNotEmpty("startDate", startDateUtc?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"))
            .AddIfNotEmpty("endDate",   endDateUtc?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"))
            .Build();
        return await Send<IReadOnlyList<QueryStat>>(HttpMethod.Get, $"queries/statistics{qs}", null, ct);
    }

    /// <summary>
    /// Sends an HTTP request to the Kimola API and deserializes the response body to the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type to which the JSON response body will be deserialized.
    /// </typeparam>
    /// <param name="method">
    /// The HTTP method to use for the request (e.g., GET, POST, PUT, DELETE).
    /// </param>
    /// <param name="path">
    /// The relative URL path for the request, including any query parameters.
    /// </param>
    /// <param name="content">
    /// The HTTP content to send in the request body. Can be null for requests like GET or DELETE.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains the deserialized response object of type <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API returns a non-success status code, with details about the failure.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the response body is empty or cannot be deserialized to the specified type.
    /// </exception>
    private async Task<T> Send<T>(HttpMethod method, string path, HttpContent? content, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(method, path) { Content = content };
        using var res = await http.SendAsync(req, ct).ConfigureAwait(false);
        if (!res.IsSuccessStatusCode)
            throw KimolaHttpException.FromResponse(res.StatusCode, await SafeReadAsync(res));

        await using var stream = await res.Content.ReadAsStreamAsync(ct);
        var data = await JsonSerializer.DeserializeAsync<T>(stream, json, ct);
        return data ?? throw new InvalidOperationException("Empty response body.");
    }
}
