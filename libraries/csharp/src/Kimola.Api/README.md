# Kimola.Api (C# · .NET 9)

Lightweight, dependency-free C# client for the Kimola API.

## Install (from source)

```bash
dotnet build
```

or pack:

```bash
dotnet pack -c Release
```

## Quick Start

```csharp
using Kimola.Api;

var client = new KimolaClient(new KimolaClientOptions 
{ 
    ApiKey = "KIMOLA_API_KEY", 
    BaseUrl = "https://api.kimola.com/v1" 
});

// 1) List presets
var list = await client.Presets.GetAsync(pageSize: 10);

// 2) Get labels for a preset
var labels = await client.Presets.GetLabelsAsync(list.Items[0].Key);

// 3) Predict
var result = await client.Presets.PredictAsync(list.Items[0].Key, "Loved the quality!", language: "en");

// 4) Usage
var usage = await client.Subscription.GetUsageAsync();
```

## Endpoints covered

- `GET /presets`, `GET /presets/{key}`, `GET /presets/{key}/labels`, `POST /presets/{key}/predictions`
- `GET /queries`, `GET /queries/statistics`
- `GET /subscription/usage`
