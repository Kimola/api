using System.Text.Json;
using static Kimola.Api.HttpReadHelpers;

namespace Kimola.Api;

/// <summary>
/// Provides methods for interacting with the subscription-related endpoints of the Kimola API.
/// </summary>
/// <remarks>
/// This client allows you to retrieve subscription information such as resource usage and quotas.  
/// It manages the HTTP communication, serialization, and error handling internally.
/// </remarks>
/// <param name="http">
/// The <see cref="HttpClient"/> instance used to send HTTP requests to the Kimola API.
/// It should be configured with the base address and any necessary authentication headers.
/// </param>
/// <param name="json">
/// The <see cref="JsonSerializerOptions"/> used to configure JSON serialization and deserialization
/// when processing API requests and responses.
/// </param>
public sealed class SubscriptionClient(HttpClient http, JsonSerializerOptions json)
{
    /// <summary>
    /// Retrieves the subscription usage details for the current Kimola account.
    /// </summary>
    /// <param name="dateUtc">
    /// Optional UTC date to retrieve the subscription usage for a specific day.  
    /// If not provided, the current date will be used.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains a <see cref="SubscriptionUsage"/> object with information about consumed resources and quotas.
    /// </returns>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API returns a non-success HTTP status code, providing details of the error response.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the response body is empty or cannot be deserialized into a <see cref="SubscriptionUsage"/> object.
    /// </exception>
    public async Task<SubscriptionUsage> GetUsageAsync(DateTimeOffset? dateUtc = null, CancellationToken ct = default)
    {
        var qs = new QueryStringBuilder()
            .AddIfNotEmpty("date", dateUtc?.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"))
            .Build();
        return await Send<SubscriptionUsage>(HttpMethod.Get, $"subscription/usage{qs}", null, ct);
    }

    /// <summary>
    /// Sends an HTTP request to the Kimola API and deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type to which the JSON response body will be deserialized.
    /// </typeparam>
    /// <param name="method">
    /// The HTTP method to use for the request (e.g., GET, POST, PUT, DELETE).
    /// </param>
    /// <param name="path">
    /// The relative URL path of the API endpoint, including any query string parameters.
    /// </param>
    /// <param name="content">
    /// The optional HTTP content to include in the request body. Pass <c>null</c> for requests without a body, such as GET or DELETE.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains the deserialized response of type <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API returns a non-success HTTP status code, containing details of the error response.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the response body is empty or cannot be deserialized into the specified type <typeparamref name="T"/>.
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
