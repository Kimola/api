from __future__ import annotations
from typing import Optional
from .transport import Transport, AsyncTransport
from .config import Config
from .resources.presets import Presets
from .resources.queries import Queries
from .resources.subscription import Subscription

class Kimola:
    def __init__(self, api_key: str, *, base_url: str = "https://api.kimola.com", timeout: float = 30.0):
        self.config = Config(base_url=base_url, api_key=api_key, timeout=timeout)
        self._transport = Transport(self.config, version_prefix="/v1")
        self.presets = Presets(self._transport)
        self.queries = Queries(self._transport)
        self.subscription = Subscription(self._transport)
    def close(self): self._transport.close()

class KimolaAsync:
    def __init__(self, api_key: str, *, base_url: str = "https://api.kimola.com", timeout: float = 30.0):
        self.config = Config(base_url=base_url, api_key=api_key, timeout=timeout)
        self._transport = AsyncTransport(self.config, version_prefix="/v1")
        self.presets = Presets(self._transport)
        self.queries = Queries(self._transport)
        self.subscription = Subscription(self._transport)
    async def aclose(self): await self._transport.aclose()
