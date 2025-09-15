# Kimola API

Kimola is a research and analysis platform that helps businesses understand customer feedback and market data at scale. The Kimola API provides programmatic access to its core features, making it possible to integrate data collection, analysis, and insights directly into your own applications.

This repository hosts the official documentation for the Kimola API. It includes details about available endpoints, authentication, usage examples, and guidelines for integrating Kimola’s research features into your applications.

&nbsp;

## Getting Started

To begin using the Kimola API, you’ll need a Kimola account. A free plan is available at [kimola.com/sign-up](https://kimola.com/sign-up), and some plans include API access by default. After signing in, you can find your package details and API key in the **Account** menu of the dashboard.  

The API is intended to extend your own applications with Kimola’s research and analysis features — not to build a competing service. In other words, you can integrate Kimola’s capabilities into your products or workflows, but creating a product that replicates Kimola itself through the API is not permitted.  

Any application that uses data or analysis results from Kimola must also respect our [Privacy Policy](https://kimola.com/privacy-policy). This ensures that data collected, processed, or displayed through the API is handled responsibly and in line with user expectations.  

If you notice something missing in the documentation or have suggestions to improve it, feel free to [open an issue](../../issues) here on GitHub — contributions and feedback are always welcome.

&nbsp;

## Authentication

Requests to endpoints that require authentication must include a valid API key. This ensures that only verified users can access their own data, models, and analysis results. If a request is sent without valid authentication, it will be rejected.  

You can locate your personal API key in the **Account** menu once you are logged into the Kimola dashboard. This page also shows your current subscription plan, so you can check whether API access is included.  

Kimola uses the **Bearer token** method for authentication. In this method, your API key is attached to each request inside the `Authorization` header. The API then validates the token before processing the request. This is a widely adopted standard for securing APIs, as it keeps credentials separate from request parameters and works across any HTTP client.  

Here’s an example request using the Bearer scheme:  

```bash
curl -X GET "https://api.kimola.com/v1/subscription/usage" \
     -H "Authorization: Bearer YOUR_API_KEY"
```


When authentication fails, the API returns plaintext error messages with the following HTTP status codes:

| HTTP&nbsp;Status | When it happens? | Response body |
|---|---|---|
| 400 | The `Authorization` header is missing. | `You must provide an authorization header.` |
| 400 | The header exists but does not include a Bearer token (e.g., not in the form `Authorization: Bearer <apiKey>`). | `You must provide a Bearer Token (Authorization: Bearer <apiKey>)` |
| 401 | The provided API key is invalid (not recognized). | `The provided API key is invalid.` |
| 403 | The request targets a route that requires a specific `:secret` and the key does not grant access to that secret. | `You are not allowed to access this model.` |


**Notes**

- Authentication is enforced only on endpoints marked as requiring an API key (internally, via an endpoint attribute).
- API keys are validated against Kimola’s key service and are cached for performance; invalid keys will always yield **401**.
- Always include the full Bearer header. Omitting it or sending an incorrect scheme will result in **400**.

&nbsp;

## Resources
This section introduces the core resources available in the Kimola API.

**Preset**: Presets are ready-to-use AI models hosted by Kimola that you can call directly—no training required. They power tasks like text classification and entity extraction and are intended to be embedded into your product workflows.  

**Query**: Provides API endpoints to inspect Query consumption in Kimola. Each transaction such as text classification, scraping, or tracking (e.g., retrieving customer feedback from X) consumes from the user’s Query limit.

&nbsp;

## Endpoints
Below is the full list of available endpoints in the Kimola API. Click on an endpoint to see details, parameters, and example responses.

[GET /presets](#get-presets) — Retrieve a paginated list of Presets.  
[GET /presets/{key}](#get-presetskey) — Retrieve a single Preset by key.  
[GET /presets/{key}/labels](#get-presetskeylabels) — Retrieve the labels defined for a Preset.  
[POST /presets/{key}/predictions](#post-presetskeypredictions) — Run predictions on input text using a Preset.
&nbsp;  

[GET /queries](#get-queries) — Retrieve a paginated list of Queries with optional date range.  
[GET /queries/statistics](#get-queriesstatistics) — Retrieve aggregated Query consumption statistics by category.
&nbsp;  

[GET /subscription/usage](#get-subscriptionusage) — Retrieve subscription usage for the current or past billing periods.

&nbsp;

### GET /presets

Retrieve a **paginated list of Presets** (Kimola’s pretrained AI models). Use this to discover available models, optionally filtering by type or category. Results are ordered by an internal rank for a consistent catalog view.

> **Auth required:** `Authorization: Bearer <apiKey>`

#### Query parameters

| Name        | In     | Type    | Required | Default | Notes |
|-------------|--------|---------|----------|---------|-------|
| `pageSize`  | query  | int     | no       | `10`    | Max `10`. Number of items per page. |
| `pageIndex` | query  | int     | no       | `0`     | Zero-based page index. |
| `type`      | query  | string  | no       | —       | Filter by preset type. Supported: `Extractor`, `Classifier`. |
| `category`  | query  | string  | no       | —       | Filter by preset category. Supported: `Sentiment Classifier`, `Content Classifier`. |


#### Example request

```bash
curl -X GET "https://api.kimola.com/v1/presets?pageSize=10&pageIndex=0&type=Classifier&category=Sentiment%20Classifier" \
  -H "Authorization: Bearer YOUR_API_KEY"
```

#### Example response (200)
```json
{
  "total": 2,
  "items": [
    {
      "key": "8kq3w1h2f0c7b5m9r2t6y4u1",
      "slug": "consumer-sentiment-classifier",
      "name": "Consumer Sentiment Classifier",
      "...": "other preset fields"
    },
    {
      "key": "3p9d7x1l0a2s4f6g8h0j2k4m",
      "slug": "brand-sentiment-classifier",
      "name": "Brand Sentiment Classifier",
      "...": "other preset fields"
    }
  ]
}
```

&nbsp;

### GET /presets/{key}

Retrieve a **single Preset** (Kimola’s pretrained AI model) by its unique key. Use this to fetch metadata for a specific model before requesting labels or predictions.

> **Auth required:** `Authorization: Bearer <apiKey>`

#### Path parameters

| Name | In   | Type   | Required | Default | Notes |
|------|------|--------|----------|---------|-------|
| `key` | path | string | yes      | —       | 24-character unique identifier of the preset. |

#### Example request

```bash
curl -X GET "https://api.kimola.com/v1/presets/8kq3w1h2f0c7b5m9r2t6y4u1" \
  -H "Authorization: Bearer YOUR_API_KEY"
```

#### Example response (200)
```json
{
  "key": "8kq3w1h2f0c7b5m9r2t6y4u1",
  "slug": "consumer-sentiment-classifier",
  "name": "Consumer Sentiment Classifier",
  "...": "other preset fields"
}
```

&nbsp;

### GET /presets/{key}/labels

Retrieve the list of **labels** defined for a Preset (Kimola’s pretrained AI model). Labels represent the possible outputs when a classifier predicts based on input text. Each label has a `name` and a `description`.  

For models that do not use labels (e.g., Extractors), this endpoint returns `null`. If labels are expected but none are defined, it returns an empty array.  

> **Auth required:** `Authorization: Bearer <apiKey>`

#### Path parameters

| Name | In   | Type   | Required | Default | Notes |
|------|------|--------|----------|---------|-------|
| `key` | path | string | yes      | —       | 24-character unique key of the Preset. |

#### Example request

```bash
curl -X GET "https://api.kimola.com/v1/presets/8kq3w1h2f0c7b5m9r2t6y4u1/labels" \
  -H "Authorization: Bearer YOUR_API_KEY"
```

#### Example response (200)
```json
[
  { "name": "Positive", "description": "Favorable opinion or sentiment." },
  { "name": "Negative", "description": "Unfavorable opinion or sentiment." }
]
```

&nbsp;

### POST /presets/{key}/predictions

Run **predictions** on a given text using a specific Preset (Kimola’s pretrained AI model).  
Use this to classify customer feedback or other short texts. When `aspectBased=true`, the API performs **aspect-based classification** (per-topic sentiment) and consumes **2 queries**; otherwise it returns the dominant label and consumes **1 query**.

> **Auth required:** `Authorization: Bearer <apiKey>`

#### Path parameters

| Name | In   | Type   | Required | Default | Notes |
|------|------|--------|----------|---------|-------|
| `key` | path | string | yes      | —       | 24-character unique key of the Preset to use. |

#### Query parameters

| Name         | In    | Type    | Required | Default | Notes |
|--------------|-------|---------|----------|---------|-------|
| `language`   | query | string  | no       | —       | ISO-639-1 two-letter lowercase code (e.g., `en`, `tr`, `es`). Skips auto-detection for better accuracy on short/noisy texts. |
| `aspectBased`| query | boolean | no       | `false` | If `true`, returns per-aspect labels **with sentiment** (aspect-based classification). If `false` or omitted, returns the **dominant** label with probability. |

#### Request body

| Field | In   | Type   | Required | Notes |
|------|------|--------|----------|-------|
| `text` | body | string (raw JSON string) | yes | The input text to classify. Send the raw string as a JSON value (not an object). |

#### Example request
```bash
curl -X POST "https://api.kimola.com/v1/presets/8kq3w1h2f0c7b5m9r2t6y4u1/predictions?language=en&aspectBased=true" \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  --data "\"Really pleased with the quality of the stand mixer. It looks and functions great with loads of optional accessories. I had a couple for questions first but solved via email very easily.\""
```

#### Example aspect-based response (200)
```json
[
  { "name": "Features and Quality", "sentiment": "Positive" },
  { "name": "Customer Service",     "sentiment": "Positive" }
]
```

#### Example standard response (200)
```json
[
  { "name": "Features and Quality", "probability": 0.95 }
]
```

&nbsp;

### GET /queries

Retrieve a **paginated list of Queries** consumed within a specified date range.  
Use this endpoint to review how your account spent Queries (e.g., classification, scraping, tracking) over time.  
All dates are interpreted as **UTC**.

> **Auth required:** `Authorization: Bearer <apiKey>`

#### Query parameters

| Name        | In    | Type   | Required | Default | Notes |
|-------------|-------|--------|----------|---------|-------|
| `pageIndex` | query | int    | no       | `0`     | Zero-based page index. |
| `pageSize`  | query | int    | no       | `10`    | Number of items per page. **Maximum 10**; higher values are capped to 10. |
| `startDate` | query | string (ISO 8601) | no | `now(UTC) - 1 month` | Start of the date range (UTC). If omitted, the API uses one month before current UTC time. |
| `endDate`   | query | string (ISO 8601) | no | `now(UTC)` | End of the date range (UTC). If omitted, the API uses current UTC time. |

#### Example request
```bash
curl -X GET "https://api.kimola.com/v1/queries?pageIndex=0&pageSize=10&startDate=2025-08-15T00:00:00Z&endDate=2025-09-15T23:59:59Z" \
  -H "Authorization: Bearer YOUR_API_KEY"
```

#### Example response (200)
```json
[
  {
    "report": {
      "code": "1064d7fc-801e-40dc-a91f-d51517a65203",
      "name": "...",
      "title": "..."
    },
    "item": {
      "code": "397c4d3c-67eb-4eb2-ad6d-613355939b9a",
      "name": "...",
      "type": "..."
    },
    "type": "Classification",
    "amount": 141,
    "date": "2025-08-30T00:00:00.000000Z"
  }
]
```

&nbsp;

### GET /queries/statistics

Return **Query consumption statistics grouped by category** within the specified date range.  
Categories include `Classification`, `Tracking`, and `Scraping`. Use this endpoint to see how your allocation is spent over time. All dates are interpreted as **UTC**.

> **Auth required:** `Authorization: Bearer <apiKey>`

#### Query parameters

| Name        | In    | Type                 | Required | Default            | Notes |
|-------------|-------|----------------------|----------|--------------------|-------|
| `startDate` | query | string (ISO 8601 UTC) | no       | `now(UTC) - 1 month` | Start of the date range. If omitted, the API uses one month before current UTC time. |
| `endDate`   | query | string (ISO 8601 UTC) | no       | `now(UTC)`         | End of the date range. If omitted, the API uses current UTC time. |

#### Example request

```bash
curl -X GET "https://api.kimola.com/v1/queries/statistics?startDate=2025-08-15T00:00:00Z&endDate=2025-09-15T23:59:59Z" \
  -H "Authorization: Bearer YOUR_API_KEY"
```

#### Example response (200)
```json
[
  { "name": "Classification", "count": 365, "ratio": 0.73, "percentage": 73 },
  { "name": "Tracking",       "count": 105,   "ratio": 0.21, "percentage": 21  },
  { "name": "Scraping",       "count": 30,   "ratio": 0.06, "percentage": 6  }
]
```

&nbsp;

### GET /subscription/usage

Retrieve the **current subscription usage** for the logged-in user.  
This endpoint reports how many resources have been consumed in the user’s plan, including Queries, Models, Keywords, and Links.  

The optional `date` parameter allows you to check usage for previous billing periods. If omitted, the endpoint defaults to the current UTC date and returns data for the active subscription period.

> **Auth required:** `Authorization: Bearer <apiKey>`

#### Query parameters

| Name  | In    | Type     | Required | Default           | Notes |
|-------|-------|----------|----------|-------------------|-------|
| `date` | query | string (ISO 8601 UTC) | no | `DateTime.UtcNow` | A UTC date used to fetch usage for a past subscription period. |

#### Example request

```bash
curl -X GET "https://api.kimola.com/v1/subscription/usage?date=2025-09-15T00:00:00Z" \
  -H "Authorization: Bearer YOUR_API_KEY"
```

#### Example response (200)
```json
{
  "link": {
    "count": 8,
    "limit": 1000,
    "percentage": 0.8,
    "available": 992
  },
  "model": {
    "count": 222,
    "limit": 1000,
    "percentage": 22.2,
    "available": 778
  },
  "query": {
    "count": 119502,
    "limit": 10000000,
    "percentage": 1.2,
    "available": 9880498
  },
  "keyword": {
    "count": 43,
    "limit": 1000,
    "percentage": 4.3,
    "available": 957
  }
}
```
