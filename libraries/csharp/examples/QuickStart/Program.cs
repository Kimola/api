using Kimola.Api;

Console.WriteLine("Kimola QuickStart");
var apiKey = Environment.GetEnvironmentVariable("KIMOLA_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine("Set KIMOLA_API_KEY env var first.");
    return;
}

var client = new KimolaClient(new KimolaClientOptions { ApiKey = apiKey! });

var list = await client.Presets.GetAsync(pageSize: 5);
Console.WriteLine($"Total presets (paged): {list.Total}; showing {list.Items.Count}");

if (list.Items.Count > 0)
{
    var preset = list.Items[0];
    Console.WriteLine($"Using preset: {preset.Name} ({preset.Key})");

    var labels = await client.Presets.GetLabelsAsync(preset.Key);
    Console.WriteLine("Labels: " + (labels is null ? "null" : string.Join(", ", labels.Select(l => l.Name))));

    var preds = await client.Presets.PredictAsync(preset.Key, "Loved the quality!", language: "en");
    Console.WriteLine("Prediction(s): " + string.Join(" | ", preds.Select(p => $"{p.Name}:{p.Probability?.ToString("0.00") ?? p.Sentiment}")));
}

var usage = await client.Subscription.GetUsageAsync();
Console.WriteLine($"Query used: {usage.Query.Count}/{usage.Query.Limit} ({usage.Query.Percentage}%)");
