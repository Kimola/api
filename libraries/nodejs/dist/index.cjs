"use strict";
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// src/index.ts
var index_exports = {};
__export(index_exports, {
  KimolaClient: () => KimolaClient
});
module.exports = __toCommonJS(index_exports);
var KimolaApiError = class extends Error {
  status;
  bodyText;
  constructor(message, status, bodyText) {
    super(message);
    this.name = "KimolaApiError";
    this.status = status;
    this.bodyText = bodyText;
  }
};
var KimolaClient = class {
  apiKey;
  baseUrl;
  _fetch;
  constructor(opts) {
    if (!opts?.apiKey) throw new Error("KimolaClient: 'apiKey' is required.");
    this.apiKey = opts.apiKey;
    this.baseUrl = (opts.baseUrl ?? "https://api.kimola.com/v1").replace(/\/+$/, "");
    this._fetch = opts.fetch ?? globalThis.fetch;
    if (!this._fetch) throw new Error("KimolaClient: global 'fetch' not found. Use Node 18+ or provide opts.fetch.");
  }
  // ---- low-level request ----
  async request(path, init) {
    const url = `${this.baseUrl}${path}`;
    const res = await this._fetch(url, {
      ...init,
      headers: {
        "Authorization": `Bearer ${this.apiKey}`,
        ...init?.method && init.method !== "GET" ? { "Content-Type": "application/json" } : {},
        ...init?.headers ?? {}
      }
    });
    if (!res.ok) {
      const text = await res.text().catch(() => void 0);
      let msg = text || `HTTP ${res.status}`;
      const lower = (text || "").toLowerCase();
      if (res.status === 400 && lower.includes("authorization")) {
        msg = "You must provide an authorization header.";
      } else if (res.status === 400 && lower.includes("bearer")) {
        msg = "You must provide a Bearer Token (Authorization: Bearer <apiKey>).";
      } else if (res.status === 401) {
        msg = "The provided API key is invalid.";
      } else if (res.status === 403) {
        msg = "You are not allowed to access this model.";
      }
      throw new KimolaApiError(msg, res.status, text);
    }
    const ct = res.headers.get("content-type") || "";
    if (ct.includes("application/json")) {
      return res.json();
    }
    return await res.text();
  }
  // ---- Presets ----
  async getPresets(params = {}) {
    const qp = new URLSearchParams();
    if (params.pageSize != null) qp.set("pageSize", String(params.pageSize));
    if (params.pageIndex != null) qp.set("pageIndex", String(params.pageIndex));
    if (params.type) qp.set("type", params.type);
    if (params.category) qp.set("category", params.category);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/presets${qs}`);
  }
  async getPreset(key) {
    this.ensureKey(key);
    return this.request(`/presets/${encodeURIComponent(key)}`);
  }
  async getPresetLabels(key) {
    this.ensureKey(key);
    return this.request(`/presets/${encodeURIComponent(key)}/labels`);
  }
  async createPrediction(key, text, opts = {}) {
    this.ensureKey(key);
    if (typeof text !== "string") throw new Error("createPrediction: 'text' must be a string.");
    const qp = new URLSearchParams();
    if (opts.language) qp.set("language", opts.language);
    if (opts.aspectBased === true) qp.set("aspectBased", "true");
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    const body = JSON.stringify(text);
    return this.request(`/presets/${encodeURIComponent(key)}/predictions${qs}`, {
      method: "POST",
      body
    });
  }
  // ---- Queries ----
  async getQueries(params = {}) {
    const qp = new URLSearchParams();
    if (params.pageIndex != null) qp.set("pageIndex", String(params.pageIndex));
    if (params.pageSize != null) qp.set("pageSize", String(params.pageSize));
    if (params.startDate) qp.set("startDate", params.startDate);
    if (params.endDate) qp.set("endDate", params.endDate);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/queries${qs}`);
  }
  async getQueriesStatistics(params = {}) {
    const qp = new URLSearchParams();
    if (params.startDate) qp.set("startDate", params.startDate);
    if (params.endDate) qp.set("endDate", params.endDate);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/queries/statistics${qs}`);
  }
  // ---- Subscription ----
  async getSubscriptionUsage(date) {
    const qp = new URLSearchParams();
    if (date) qp.set("date", date);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/subscription/usage${qs}`);
  }
  // ---- Helpers ----
  async *iteratePresets(params = {}, maxPages = 100) {
    let pageIndex = 0, pages = 0;
    while (pages++ < maxPages) {
      const page = await this.getPresets({ ...params, pageIndex });
      for (const item of page.items) yield item;
      const size = params.pageSize ?? 10;
      pageIndex += 1;
      if (page.items.length < size) break;
    }
  }
  ensureKey(key) {
    if (!key || typeof key !== "string") throw new Error("Preset 'key' is required.");
  }
};
// Annotate the CommonJS export names for ESM import in node:
0 && (module.exports = {
  KimolaClient
});
