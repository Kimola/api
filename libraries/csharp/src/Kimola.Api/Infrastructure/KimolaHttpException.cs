namespace Kimola.Api;

/// <summary>
/// Represents errors returned from the Kimola API when an HTTP request fails.
/// </summary>
/// <remarks>
/// This exception includes the HTTP status code and the raw response body for easier debugging.  
/// It is thrown whenever the API returns a non-success status code (4xx or 5xx).
/// </remarks>
public sealed class KimolaHttpException : Exception
{
    /// <summary>
    /// The HTTP status code returned by the API.
    /// </summary>
    public System.Net.HttpStatusCode StatusCode { get; }

    /// <summary>
    /// The raw body of the HTTP response, if available.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KimolaHttpException"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="responseBody">The raw HTTP response body returned by the API, if available.</param>
    /// <param name="message">A user-friendly error message describing the issue.</param>
    private KimolaHttpException(System.Net.HttpStatusCode statusCode, string? responseBody, string message) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    /// <summary>
    /// Creates a new <see cref="KimolaHttpException"/> based on the provided status code and response body.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="body">The raw response body returned by the API.</param>
    /// <returns>A <see cref="KimolaHttpException"/> with a descriptive error message.</returns>
    /// <remarks>
    /// Custom messages are provided for common API errors:
    /// <list type="bullet">
    /// <item><description>400 – Bad Request: Missing or invalid <c>Authorization</c> header.</description></item>
    /// <item><description>401 – Unauthorized: Invalid API key.</description></item>
    /// <item><description>403 – Forbidden: API key does not have permission to access this resource.</description></item>
    /// </list>
    /// For all other status codes, a generic message will be generated.
    /// </remarks>
    public static KimolaHttpException FromResponse(System.Net.HttpStatusCode statusCode, string? body)
    {
        string msg = statusCode switch
        {
            System.Net.HttpStatusCode.BadRequest => "Bad Request – missing/invalid Authorization header.",
            System.Net.HttpStatusCode.Unauthorized => "Unauthorized – invalid API key.",
            System.Net.HttpStatusCode.Forbidden => "Forbidden – your key cannot access this resource.",
            _ => $"HTTP {(int)statusCode} – API request failed."
        };

        if (!string.IsNullOrWhiteSpace(body))
            msg += $" Body: {body}";

        return new KimolaHttpException(statusCode, body, msg);
    }
}

/// <summary>
/// Helper methods for safely reading HTTP responses.
/// </summary>
internal static class HttpReadHelpers
{
    /// <summary>
    /// Safely reads the response body from an <see cref="HttpResponseMessage"/> as a string.  
    /// If reading fails, returns <c>null</c> instead of throwing an exception.
    /// </summary>
    /// <param name="res">The HTTP response message to read.</param>
    /// <returns>The response body as a string, or <c>null</c> if reading fails.</returns>
    public static async Task<string?> SafeReadAsync(HttpResponseMessage res)
    {
        try
        {
            return await res.Content.ReadAsStringAsync();
        }
        catch
        {
            return null;
        }
    }
}