from __future__ import annotations
from typing import Optional, Dict, Any

class Subscription:
    """
    Resource for Subscription-related endpoints.
    - GET /v1/Subscription/usage
    """
    def __init__(self, transport):
        self._t = transport

    def usage(self, *, date: Optional[str] = None) -> Dict[str, Any]:
        """
        Retrieve subscription usage data.

        Args:
            date (str, optional): UTC date in ISO format. Defaults to current UTC if None.

        Returns:
            dict: Usage data for Queries, Models, Keywords, and Links.
        """
        params = {}
        if date:
            params["date"] = date
        return self._t.get("/Subscription/usage", params=params)
