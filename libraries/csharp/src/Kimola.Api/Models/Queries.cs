namespace Kimola.Api;

/// <summary>
/// Represents the report details included in a query entry.
/// </summary>
/// <remarks>
/// Each query is linked to a report that triggered the consumption.  
/// This contains identifying information about the report, which is useful for tracking where the queries originated.
/// </remarks>
/// <param name="Code">
/// The unique identifier (GUID) of the report.
/// </param>
/// <param name="Name">
/// The internal name of the report.
/// </param>
/// <param name="Title">
/// The user-friendly title of the report displayed in the dashboard or UI.
/// </param>
public sealed record QueryReport(string Code, string Name, string Title);

/// <summary>
/// Represents the specific item within a query transaction.
/// </summary>
/// <remarks>
/// A query item describes what entity was involved in the query, such as a preset or another resource.  
/// This helps categorize and understand what type of action consumed queries.
/// </remarks>
/// <param name="Code">
/// The unique identifier (GUID) of the item.
/// </param>
/// <param name="Name">
/// The display name of the item.
/// </param>
/// <param name="Type">
/// The type of the query item (e.g., <c>Classifier</c>, <c>Scraper</c>, <c>Tracker</c>).
/// </param>
public sealed record QueryItemRecord(string Code, string Name, string Type);

/// <summary>
/// Represents a single query consumption event.
/// </summary>
/// <remarks>
/// Each entry shows the report and item involved, along with metadata such as the amount consumed and the timestamp.  
/// Use this model to inspect consumption history when calling <c>GET /queries</c>.
/// </remarks>
/// <param name="Report">
/// The report associated with this query event.
/// </param>
/// <param name="Item">
/// The specific query item linked to this event.
/// </param>
/// <param name="Type">
/// The classification of the query event, such as <c>Classification</c>, <c>Tracking</c>, or <c>Scraping</c>.
/// </param>
/// <param name="Amount">
/// The number of queries consumed by this event.
/// </param>
/// <param name="Date">
/// The timestamp (UTC) indicating when the query was processed.  
/// Serialized in ISO 8601 format with a <c>Z</c> suffix.
/// </param>
public sealed record QueryItem(QueryReport Report, QueryItemRecord Item, string Type, int Amount, DateTimeOffset Date);

/// <summary>
/// Represents aggregated query usage statistics for a given category.
/// </summary>
/// <remarks>
/// This is returned by the <c>GET /queries/statistics</c> endpoint to show grouped consumption by category.
/// </remarks>
/// <param name="Name">
/// The category name, such as <c>Classification</c>, <c>Tracking</c>, or <c>Scraping</c>.
/// </param>
/// <param name="Count">
/// The total number of queries consumed for this category.
/// </param>
/// <param name="Ratio">
/// The ratio of this category relative to total consumption (e.g., 0.73 for 73%).
/// </param>
/// <param name="Percentage">
/// The ratio expressed as a whole number percentage (e.g., 73).
/// </param>
public sealed record QueryStat(string Name, int Count, double Ratio, int Percentage);