from __future__ import annotations
from typing import Dict, Any, Optional
import httpx

from .config import Config

class Transport:
    def __init__(self, config: Config, *, version_prefix: str = ""):
        self.config = config
        self.version_prefix = version_prefix.rstrip("/")
        self._client = httpx.Client(base_url=config.base_url, timeout=config.timeout)

    def _headers(self) -> Dict[str, str]:
        return {
            "Accept": "application/json",
            "Authorization": f"Bearer {self.config.api_key}",
            "User-Agent": self.config.user_agent,
        }

    def _url(self, path: str) -> str:
        path = path if path.startswith("/") else f"/{path}"
        return f"{self.version_prefix}{path}"

    def get(self, path: str, params: Optional[dict] = None) -> Any:
        r = self._client.get(self._url(path), headers=self._headers(), params=params or {})
        if r.status_code >= 400:
            raise httpx.HTTPStatusError(f"HTTP {r.status_code}: {r.text}", request=r.request, response=r)
        return r.json() if "application/json" in r.headers.get("Content-Type", "") else r.text

    def post(self, path: str, json_body: Any = None, params: Optional[dict] = None) -> Any:
        r = self._client.post(self._url(path), headers=self._headers(), params=params or {}, json=json_body)
        if r.status_code >= 400:
            raise httpx.HTTPStatusError(f"HTTP {r.status_code}: {r.text}", request=r.request, response=r)
        return r.json() if "application/json" in r.headers.get("Content-Type", "") else r.text

    def close(self): self._client.close()

class AsyncTransport:
    def __init__(self, config: Config, *, version_prefix: str = ""):
        self.config = config
        self.version_prefix = version_prefix.rstrip("/")
        self._client = httpx.AsyncClient(base_url=config.base_url, timeout=config.timeout)

    def _headers(self) -> Dict[str, str]:
        return {
            "Accept": "application/json",
            "Authorization": f"Bearer {self.config.api_key}",
            "User-Agent": self.config.user_agent,
        }

    def _url(self, path: str) -> str:
        path = path if path.startswith("/") else f"/{path}"
        return f"{self.version_prefix}{path}"

    async def get(self, path: str, params: Optional[dict] = None) -> Any:
        r = await self._client.get(self._url(path), headers=self._headers(), params=params or {})
        if r.status_code >= 400:
            raise httpx.HTTPStatusError(f"HTTP {r.status_code}: {r.text}", request=r.request, response=r)
        ct = r.headers.get("Content-Type", "")
        return await r.json() if "application/json" in ct else await r.text()

    async def post(self, path: str, json_body: Any = None, params: Optional[dict] = None) -> Any:
        r = await self._client.post(self._url(path), headers=self._headers(), params=params or {}, json=json_body)
        if r.status_code >= 400:
            raise httpx.HTTPStatusError(f"HTTP {r.status_code}: {r.text}", request=r.request, response=r)
        ct = r.headers.get("Content-Type", "")
        return await r.json() if "application/json" in ct else await r.text()

    async def aclose(self): await self._client.aclose()
