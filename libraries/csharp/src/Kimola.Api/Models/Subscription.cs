using System.Text.Json.Serialization;

namespace Kimola.Api;

/// <summary>
/// Represents a single usage bucket within a subscription plan. 
/// Each bucket corresponds to a specific resource type tracked by Kimola, such as links, models, queries, or keywords.
/// </summary>
/// <param name="Count">
/// The number of units that have been consumed for this resource type in the current or specified billing period.
/// </param>
/// <param name="Limit">
/// The maximum number of units allowed for this resource type under the current subscription plan.
/// </param>
/// <param name="Percentage">
/// The percentage of the limit that has been used, ranging from 0.0 to 100.0.
/// </param>
/// <param name="Available">
/// The number of remaining units before this resource type reaches its limit.
/// </param>
public sealed record SubscriptionUsageBucket(int Count, int Limit, double Percentage, int Available);

/// <summary>
/// Represents the overall subscription usage for the current or a past billing period. 
/// Provides usage details for each tracked resource type: Links, Models, Queries, and Keywords.
/// </summary>
/// <param name="Link">
/// Usage information for link-based resources.  
/// These represent API actions related to link tracking within the Kimola platform.
/// </param>
/// <param name="Model">
/// Usage information for model-based resources.  
/// These represent API actions related to machine learning models, such as running predictions or classifications.
/// </param>
/// <param name="Query">
/// Usage information for query-based resources.  
/// This includes all transactions like classification, scraping, and tracking customer feedback.  
/// Serialized as <c>"query"</c> in the API response.
/// </param>
/// <param name="Keyword">
/// Usage information for keyword-based resources.  
/// These represent resources related to keyword tracking or analysis within the Kimola platform.
/// </param>
public sealed record SubscriptionUsage(
  SubscriptionUsageBucket Link,
  SubscriptionUsageBucket Model,
  [property: JsonPropertyName("query")] SubscriptionUsageBucket Query,
  SubscriptionUsageBucket Keyword
);