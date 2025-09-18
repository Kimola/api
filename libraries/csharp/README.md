# Kimola C# SDK

A lightweight .NET 9 client for the **Kimola API**.  
This SDK helps you integrate Kimola's features into your .NET apps quickly.

---

## Installation

Add the NuGet package to your project:

```bash
dotnet add package Kimola.Api
```

---

## Quick Start

Set your API key as an environment variable:

```bash
# macOS/Linux
export KIMOLA_API_KEY=YOUR_REAL_API_KEY

# Windows PowerShell
$env:KIMOLA_API_KEY='YOUR_REAL_API_KEY'
```

Create a new C# console app and use the SDK:

```csharp
using Kimola.Api;

var client = new KimolaClient(new KimolaClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("KIMOLA_API_KEY")!,
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

Console.WriteLine("\nSubscription Usage:");
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(usage));
```

Run your app:

```bash
dotnet run
```

---

## Features
- 🔹 List available presets
- 🔹 Get labels for a preset
- 🔹 Predict text classifications
- 🔹 Retrieve queries and query statistics
- 🔹 Check subscription usage

---

## Endpoints Covered
- `GET /presets`
- `GET /presets/{key}`
- `GET /presets/{key}/labels`
- `POST /presets/{key}/predictions`
- `GET /queries`
- `GET /queries/statistics`
- `GET /subscription/usage`

---

## Links
- [NuGet Package](https://www.nuget.org/packages/Kimola.Api)
- [Kimola API Docs](https://api.kimola.com/swagger/index.html)
- [GitHub Repository](https://github.com/Kimola/api/tree/main/libraries/csharp)