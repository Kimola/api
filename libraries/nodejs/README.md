# Kimola Node.js SDK

TypeScript-first Node.js SDK for the **Kimola API** (based on the spec you provided). Requires **Node 18+** (uses global `fetch`).

## Install deps
```bash
npm i
```

## Run the example (no build needed)
```bash
export KIMOLA_API_KEY=YOUR_API_KEY   # macOS/Linux
# $env:KIMOLA_API_KEY='YOUR_API_KEY'  # Windows PowerShell

npm run dev
```
`examples/quick.ts` imports the client directly from `src/index.ts` via `tsx`.

## Build the library (ESM + CJS + typings)
```bash
npm run build
```

## Use in your own app
After building and publishing (or linking), you can:
```ts
import { KimolaClient } from "@kimola/api";
const kimola = new KimolaClient({ apiKey: process.env.KIMOLA_API_KEY! });
```

## Endpoints implemented
- `GET /presets` → `getPresets`
- `GET /presets/{key}` → `getPreset`
- `GET /presets/{key}/labels` → `getPresetLabels`
- `POST /presets/{key}/predictions` → `createPrediction`
- `GET /queries` → `getQueries`
- `GET /queries/statistics` → `getQueriesStatistics`
- `GET /subscription/usage` → `getSubscriptionUsage`

### Auth errors
The SDK maps server text messages for 400/401/403 into clearer errors:
- 400 missing header → "You must provide an authorization header."
- 400 not Bearer → "You must provide a Bearer Token (Authorization: Bearer <apiKey>)."
- 401 invalid key → "The provided API key is invalid."
- 403 not allowed → "You are not allowed to access this model."

## Example snippets
```ts
import { KimolaClient } from "@kimola/api";

const kimola = new KimolaClient({ apiKey: process.env.KIMOLA_API_KEY! });

// 1) List presets
const presets = await kimola.getPresets({ pageSize: 10, type: "Classifier" });

// 2) Labels for a preset
const labels = await kimola.getPresetLabels("8kq3w1h2f0c7b5m9r2t6y4u1");

// 3) Prediction (dominant label)
const result = await kimola.createPrediction(
  "8kq3w1h2f0c7b5m9r2t6y4u1",
  "Loved the build quality!"
);

// 4) Aspect-based prediction
const aspects = await kimola.createPrediction(
  "8kq3w1h2f0c7b5m9r2t6y4u1",
  "Support was helpful; product quality is great.",
  { language: "en", aspectBased: true }
);

// 5) Queries & stats
const queries = await kimola.getQueries({ pageIndex: 0, pageSize: 10 });
const stats = await kimola.getQueriesStatistics({
  startDate: "2025-08-15T00:00:00Z",
  endDate:   "2025-09-15T23:59:59Z"
});

// 6) Subscription usage
const usage = await kimola.getSubscriptionUsage();
```
