namespace Kimola.Api;

internal sealed class QueryStringBuilder
{
    private readonly List<string> _parts = new();

    public QueryStringBuilder Add(string name, int value)
    { _parts.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value.ToString())}"); return this; }

    public QueryStringBuilder Add(string name, bool value)
    { _parts.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value.ToString().ToLowerInvariant())}"); return this; }

    public QueryStringBuilder AddIfNotEmpty(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _parts.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}");
        return this;
    }

    public string Build() => _parts.Count == 0 ? string.Empty : "?" + string.Join('&', _parts);
}
