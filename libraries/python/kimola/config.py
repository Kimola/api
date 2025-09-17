from __future__ import annotations
from dataclasses import dataclass

@dataclass
class Config:
    base_url: str
    api_key: str
    timeout: float = 30.0
    user_agent: str = "kimola-python-sdk/0.2.0"
