# Kimola Node.js SDK

TypeScript-first Node.js SDK for the **Kimola API** (based on the spec you provided). Requires **Node 18+** (uses global `fetch`).

## Install (for app developers)
Install the published package from npm into **your application** (no local build required):
```bash
npm install @kimola/api
# or
pnpm add @kimola/api
# or
yarn add @kimola/api
```

> Requires **Node 18+** (for global `fetch`). The package ships with types and works in ESM or CommonJS projects.

## Quick start
```ts
// ESM / TypeScript
import { KimolaClient } from "@kimola/api";

const kimola = new KimolaClient({ apiKey: process.env.KIMOLA_API_KEY! });
const presets = await kimola.getPresets({ pageSize: 5 });
console.log(presets.items?.map(p => p.name));
```

**CommonJS example:**
```js
// CommonJS
const { KimolaClient } = require("@kimola/api");
const kimola = new KimolaClient({ apiKey: process.env.KIMOLA_API_KEY });
```

**Environment variable**
```bash
export KIMOLA_API_KEY=YOUR_API_KEY   # macOS/Linux
# $env:KIMOLA_API_KEY='YOUR_API_KEY'  # Windows PowerShell
```

**Security note:** Use the SDK on the **server side**. Do not expose your API key in browser code.

## Run the local example (contributors)
```bash
export KIMOLA_API_KEY=YOUR_API_KEY   # macOS/Linux
# $env:KIMOLA_API_KEY='YOUR_API_KEY'  # Windows PowerShell

npm run dev
```
`examples/quick.ts` imports the client directly from `src/index.ts` via `tsx`. This is only for local development and testing the SDK; application developers should install from npm as shown above.

## Build the library (ESM + CJS + typings)
```bash
npm run build
```

## Use in your own app
After installing from npm, you can use the client like this:
```ts
import { KimolaClient } from "@kimola/api";
const kimola = new KimolaClient({ apiKey: process.env.KIMOLA_API_KEY! });
```

> If you are working on the SDK itself, you can `npm link` this package into a test app, but most users should simply install from npm.

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
