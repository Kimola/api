namespace Kimola.Api;

/// <summary>
/// Represents a **preset definition** in the Kimola API.
/// </summary>
/// <remarks>
/// A preset is a ready-to-use AI model hosted by Kimola that performs tasks such as
/// text classification or entity extraction.  
/// You can retrieve a list of presets using <c>GET /presets</c> and run predictions with <c>POST /presets/{key}/predictions</c>.
/// </remarks>
/// <param name="Key">
/// The unique identifier of the preset (24-character string).  
/// This is required when requesting labels or predictions.
/// </param>
/// <param name="Slug">
/// A URL-friendly string representing the preset name or category.
/// </param>
/// <param name="Name">
/// The display name of the preset, used in UI and dashboards.
/// </param>
public sealed record Preset(string Key, string Slug, string Name);

/// <summary>
/// Represents a paginated API response containing a collection of presets.
/// </summary>
/// <remarks>
/// Returned by <c>GET /presets</c> when listing available pretrained models.  
/// Includes the total count for pagination and the current page of <see cref="Preset"/> results.
/// </remarks>
/// <param name="Total">
/// The total number of presets available that match the current query filters.
/// </param>
/// <param name="Items">
/// A read-only list of <see cref="Preset"/> objects representing the current page of results.
/// </param>
public sealed record PagedPresetsResponse(int Total, IReadOnlyList<Preset> Items);

/// <summary>
/// Represents a label associated with a specific preset in the Kimola API.
/// </summary>
/// <remarks>
/// Labels define the possible outputs for a classification model.  
/// Use <c>GET /presets/{key}/labels</c> to retrieve these for a given preset.
/// </remarks>
/// <param name="Name">
/// The label name, such as <c>Positive</c>, <c>Negative</c>, or another category.
/// </param>
/// <param name="Description">
/// An optional description providing more context about the label, such as its use case or meaning.
/// </param>
public sealed record PresetLabel(string Name, string? Description);