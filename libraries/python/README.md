# Kimola Python SDK

A lightweight, user-friendly Python SDK for the [Kimola API](https://docs.kimola.com/), closely aligned with the official documentation.

- **Authentication**: Uses a Bearer token in the `Authorization` header.
- **Base URL**: `https://api.kimola.com`
- **Version prefix**: `/v1`

## Installation

Install from PyPI (recommended):
```bash
pip install kimola==0.1.0
```

## Quickstart

```python
from kimola import Kimola

# Replace with your actual API key
sdk = Kimola(api_key="YOUR_API_KEY")

# Presets
presets = sdk.presets.list(page_size=10, page_index=0, type="Classifier", category="Sentiment Classifier")
one = sdk.presets.get("8kq3w1h2f0c7b5m9r2t6y4u1")
labels = sdk.presets.labels("8kq3w1h2f0c7b5m9r2t6y4u1")
pred = sdk.presets.predict(
    "8kq3w1h2f0c7b5m9r2t6y4u1",
    text="Great product!",
    language="en",
    aspect_based=False
)

# Queries
q = sdk.queries.list(
    page_index=0,
    page_size=10,
    start_date="2025-08-15T00:00:00Z",
    end_date="2025-09-15T23:59:59Z"
)
qs = sdk.queries.statistics(
    start_date="2025-08-15T00:00:00Z",
    end_date="2025-09-15T23:59:59Z"
)

# Subscription usage
usage = sdk.subscription.usage(date="2025-09-15T00:00:00Z")
print(usage)

# Don't forget to close the SDK connection when done
sdk.close()
```

## Async Usage

```python
import os
import asyncio
from kimola import KimolaAsync

api_key = os.environ["KIMOLA_API_KEY"]

async def main():
    async with KimolaAsync(api_key=api_key) as sdk:
        presets = await sdk.presets.list(page_size=5)
        print(presets)

asyncio.run(main())
```

## Security Note

**Never hardcode your API keys.** Store sensitive credentials like your Kimola API key in environment variables (e.g., `KIMOLA_API_KEY`) and read them securely in your code.

## Links

- [GitHub Repository](https://github.com/Kimola/api/tree/main/libraries/python)
- [Kimola Privacy Policy](https://kimola.com/privacy-policy)
- [PyPI Package](https://pypi.org/project/kimola/0.1.0)