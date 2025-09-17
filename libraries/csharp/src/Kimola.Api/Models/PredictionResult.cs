namespace Kimola.Api;

/// <summary>
/// Represents the result of a prediction returned by the Kimola API.
/// </summary>
/// <remarks>
/// For **standard predictions** (<c>aspectBased=false</c>):
/// - The <see cref="Probability"/> property will be populated with a confidence score between 0.0 and 1.0.
/// - The <see cref="Sentiment"/> property will be <c>null</c>.
///
/// For **aspect-based predictions** (<c>aspectBased=true</c>):
/// - The <see cref="Sentiment"/> property will indicate the detected sentiment, such as "Positive", "Negative", or "Neutral".
/// - The <see cref="Probability"/> property will be <c>null</c>.
/// </remarks>
/// <param name="Name">
/// The name or label associated with the prediction, such as a category or aspect.
/// </param>
/// <param name="Probability">
/// The confidence score for standard predictions.  
/// Only populated when <c>aspectBased=false</c>; otherwise, this is <c>null</c>.
/// </param>
/// <param name="Sentiment">
/// The sentiment result for aspect-based predictions.  
/// Only populated when <c>aspectBased=true</c>; otherwise, this is <c>null</c>.
/// </param>
public sealed record PredictionResult(string Name, double? Probability, string? Sentiment);