from __future__ import annotations
from typing import Optional, Any, Dict

class Presets:
    """
    Endpoints:
      - GET /presets
      - GET /presets/{key}
      - GET /presets/{key}/labels
      - POST /presets/{key}/predictions
    """
    def __init__(self, transport):
        self._t = transport

    def list(self, *, page_size: int = 10, page_index: int = 0,
             type: Optional[str] = None, category: Optional[str] = None) -> Dict[str, Any]:
        params = {"pageSize": page_size, "pageIndex": page_index}
        if type is not None:
            params["type"] = type
        if category is not None:
            params["category"] = category
        return self._t.get("/presets", params=params)

    def get(self, key: str) -> Dict[str, Any]:
        return self._t.get(f"/presets/{key}")

    def labels(self, key: str):
        return self._t.get(f"/presets/{key}/labels")

# Optional: a forgiving helper that turns 404 into None instead of raising.
    def labels_or_none(self, key: str):
        try:
            return self.labels(key)
        except HTTPStatusError as e:
            if e.response.status_code == 404:
                return None
            raise

    def predict(
        self,
        key: str,
        *,
        text: str,
        language: Optional[str] = None,        # e.g., "en", "tr", "es", "de", "fr"
        aspect_based: bool = False
    ):
        """
        POST /v1/Presets/{key}/predictions
        - Body: raw JSON string (the input text)
        - Query: language?, aspectBased?
        """
        if not text or not isinstance(text, str):
            raise ValueError("`text` must be a non-empty string.")

        params: Dict[str, Any] = {}
        if language:        # pass-through; server expects ISO-639-1 lowercase
            params["language"] = language
        if aspect_based:    # omit when False (spec: omitted or false => dominant label)
            params["aspectBased"] = "true"

        # IMPORTANT: send the *raw* string as JSON, not an object
        # httpx will serialize a Python str as a JSON string (quoted) when passed via json=
        return self._t.post(f"/Presets/{key}/predictions", json_body=text, params=params)