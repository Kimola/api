using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Kimola.Api.HttpReadHelpers;

namespace Kimola.Api;

/// <summary>
/// Provides methods for interacting with the Kimola API's Presets endpoints.
/// </summary>
/// <remarks>
/// This client allows you to retrieve preset definitions, fetch their associated labels,
/// and run text predictions using a specific preset. It handles all HTTP communication,
/// serialization, and error handling.
/// </remarks>
/// <param name="http">
/// The <see cref="HttpClient"/> instance used to send HTTP requests to the Kimola API. 
/// This should be configured with the base URL and any required authentication headers.
/// </param>
/// <param name="json">
/// The <see cref="JsonSerializerOptions"/> used to control JSON serialization and deserialization 
/// for requests and responses.
/// </param>
public sealed class PresetsClient(HttpClient http, JsonSerializerOptions json)
{
    /// <summary>
    /// Retrieves a paginated list of presets from the Kimola API, optionally filtered by type and category.
    /// </summary>
    /// <param name="pageSize">
    /// The number of presets to include in each page. Defaults to 10.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve. Defaults to 0.
    /// </param>
    /// <param name="type">
    /// Optional filter to retrieve presets of a specific type.
    /// </param>
    /// <param name="category">
    /// Optional filter to retrieve presets within a specific category.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains a <see cref="PagedPresetsResponse"/> with the retrieved presets and pagination metadata.
    /// </returns>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API responds with a non-success status code.
    /// </exception>
    public async Task<PagedPresetsResponse> GetAsync(int pageSize = 10, int pageIndex = 0, string? type = null, string? category = null, CancellationToken ct = default)
    {
        var qs = new QueryStringBuilder()
            .Add("pageSize", pageSize)
            .Add("pageIndex", pageIndex)
            .AddIfNotEmpty("type", type)
            .AddIfNotEmpty("category", category)
            .Build();
        return await Send<PagedPresetsResponse>(HttpMethod.Get, $"presets{qs}", null, ct);
    }

    /// <summary>
    /// Retrieves a specific preset from the Kimola API by its unique key.
    /// </summary>
    /// <param name="key">
    /// The unique identifier of the preset to retrieve. Must be a valid, non-empty string with a minimum length of 8 characters.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains the <see cref="Preset"/> object corresponding to the specified key.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided <paramref name="key"/> is null, empty, or shorter than 8 characters.
    /// </exception>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API responds with a non-success status code.
    /// </exception>
    public async Task<Preset> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        EnsureKey(key);
        return await Send<Preset>(HttpMethod.Get, $"presets/{key}", null, ct);
    }

    /// <summary>
    /// Retrieves the list of labels associated with a specific preset from the Kimola API.
    /// </summary>
    /// <param name="key">
    /// The unique identifier of the preset whose labels are to be retrieved. Must be a valid, non-empty string with a minimum length of 8 characters.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains a read-only list of <see cref="PresetLabel"/> objects, or null if no labels are found.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided <paramref name="key"/> is null, empty, or shorter than 8 characters.
    /// </exception>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API responds with a non-success status code.
    /// </exception>
    public async Task<IReadOnlyList<PresetLabel>?> GetLabelsAsync(string key, CancellationToken ct = default)
    {
        EnsureKey(key);
        return await Send<IReadOnlyList<PresetLabel>?>(HttpMethod.Get, $"presets/{key}/labels", null, ct);
    }

    /// <summary>
    /// Sends text to a specified preset for prediction and retrieves the analysis results.
    /// </summary>
    /// <param name="key">
    /// The unique identifier of the preset to use for prediction. Must be a valid, non-empty string with a minimum length of 8 characters.
    /// </param>
    /// <param name="text">
    /// The input text to analyze. This field is required and cannot be null or empty.
    /// </param>
    /// <param name="language">
    /// Optional ISO language code (e.g., "en", "tr").  
    /// When provided, the preset will process the text using the specified language.
    /// </param>
    /// <param name="aspectBased">
    /// A flag indicating whether aspect-based analysis should be performed.  
    /// Set to <c>true</c> for detailed aspect-level predictions; defaults to <c>false</c>.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains a read-only list of <see cref="PredictionResult"/> objects representing the prediction outcomes.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="key"/> is invalid or when the <paramref name="text"/> parameter is null or empty.
    /// </exception>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API responds with a non-success status code.
    /// </exception>
    public async Task<IReadOnlyList<PredictionResult>> PredictAsync(string key, string text, string? language = null, bool aspectBased = false, CancellationToken ct = default)
    {
        EnsureKey(key);
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("text is required", nameof(text));
        var qs = new QueryStringBuilder()
            .AddIfNotEmpty("language", language)
            .Add("aspectBased", aspectBased)
            .Build();

        var payload = new StringContent(JsonSerializer.Serialize(text, json), Encoding.UTF8, "application/json");
        return await Send<IReadOnlyList<PredictionResult>>(HttpMethod.Post, $"presets/{key}/predictions{qs}", payload, ct);
    }

    /// <summary>
    /// Sends an HTTP request to the Kimola API and deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type to which the response body will be deserialized.
    /// </typeparam>
    /// <param name="method">
    /// The HTTP method to use for the request, such as GET, POST, PUT, or DELETE.
    /// </param>
    /// <param name="path">
    /// The relative URL path for the API endpoint, including query parameters if applicable.
    /// </param>
    /// <param name="content">
    /// The optional HTTP content to include in the request body, such as JSON payload for POST or PUT requests.
    /// Pass <c>null</c> for requests without a body.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that can be used to cancel the request.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains the deserialized response of type <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="KimolaHttpException">
    /// Thrown when the API returns a non-success HTTP status code, providing details of the error response.
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

    /// <summary>
    /// Validates that the provided preset key is not null, empty, or shorter than the required length.
    /// </summary>
    /// <param name="key">
    /// The preset key to validate. Must be a non-empty string with a minimum length of 8 characters.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="key"/> is null, empty, or shorter than 8 characters.
    /// </exception>
    private static void EnsureKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length < 8)
            throw new ArgumentException("A valid preset key is required.", nameof(key));
    }
}
