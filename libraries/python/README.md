# Kimola SDK (Docs-aligned)

A tiny, user-friendly Python SDK for the **Kimola API**, aligned with the endpoints in your docs.

- **Auth**: Bearer token in `Authorization` header
- **Base URL**: `https://api.kimola.com`
- **Version prefix**: `/v1`

## Install (editable)
```bash
pip install -e .
```

## Quickstart

```python
from kimola import Kimola

sdk = Kimola(api_key="YOUR_API_KEY")

# Presets
presets = sdk.presets.list(page_size=10, page_index=0, type="Classifier", category="Sentiment Classifier")
one = sdk.presets.get("8kq3w1h2f0c7b5m9r2t6y4u1")
labels = sdk.presets.labels("8kq3w1h2f0c7b5m9r2t6y4u1")
pred = sdk.presets.predict("8kq3w1h2f0c7b5m9r2t6y4u1", text="Great product!", language="en", aspect_based=False)

# Queries
q = sdk.queries.list(page_index=0, page_size=10, start_date="2025-08-15T00:00:00Z", end_date="2025-09-15T23:59:59Z")
qs = sdk.queries.statistics(start_date="2025-08-15T00:00:00Z", end_date="2025-09-15T23:59:59Z")

# Subscription usage
usage = sdk.subscription.usage(date="2025-09-15T00:00:00Z")
print(usage)

sdk.close()
```

For async usage, import `KimolaAsync` instead.
