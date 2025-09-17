# kimola/resources/queries.py
from __future__ import annotations
from typing import Optional, Any, Dict, List

class Queries:
    """
    Endpoints:
      - GET /v1/Queries
      - GET /v1/Queries/statistics
    """
    def __init__(self, transport):
        self._t = transport

    def list(self, *, page_index: int = 0, page_size: int = 10,
             start_date: Optional[str] = None, end_date: Optional[str] = None):
        params: Dict[str, Any] = {"pageIndex": page_index, "pageSize": page_size}
        if start_date is not None:
            params["startDate"] = start_date
        if end_date is not None:
            params["endDate"] = end_date
        # Use capital Q to mirror the API docs
        return self._t.get("/Queries", params=params)

    def statistics(self, *, start_date: Optional[str] = None, end_date: Optional[str] = None) -> List[Dict[str, Any]]:
        """
        Returns Query consumption statistics grouped by category within the date range.
        - start_date / end_date: ISO-8601 UTC strings (e.g., '2025-09-15T00:00:00Z')
        - If omitted, API defaults to [now-1 month, now] in UTC.
        """
        params: Dict[str, Any] = {}
        if start_date is not None:
            params["startDate"] = start_date
        if end_date is not None:
            params["endDate"] = end_date
        return self._t.get("/Queries/statistics", params=params)
