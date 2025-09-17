/* Kimola API Node.js SDK (TypeScript)
 * Node 18+ required (global fetch).
 */

export type PresetType = "Extractor" | "Classifier";
export type PresetCategory = "Sentiment Classifier" | "Content Classifier";

export interface Paginated<T> {
  total: number;
  items: T[];
  pageIndex?: number;
  pageSize?: number;
}

export interface PresetSummary {
  key: string;
  slug: string;
  name: string;
  [k: string]: unknown;
}
export interface Preset extends PresetSummary {}

export interface Label {
  name: string;
  description: string;
}

export interface QueryListParams {
  pageIndex?: number;
  pageSize?: number;
  startDate?: string; // ISO UTC
  endDate?: string;   // ISO UTC
}

export interface QueryItem {
  report?: { code: string; name?: string; title?: string };
  item?:   { code: string; name?: string; type?: string };
  type: "Classification" | "Tracking" | "Scraping" | string;
  amount: number;
  date: string; // ISO UTC
}

export interface QueryStat {
  name: "Classification" | "Tracking" | "Scraping" | string;
  count: number;
  ratio: number;
  percentage: number;
}

export interface SubscriptionCounter {
  count: number;
  limit: number;
  percentage: number;
  available: number;
}

export interface SubscriptionUsage {
  link: SubscriptionCounter;
  model: SubscriptionCounter;
  query: SubscriptionCounter;
  keyword: SubscriptionCounter;
}

export interface ClientOptions {
  apiKey: string;
  baseUrl?: string;        // default https://api.kimola.com/v1
  fetch?: typeof fetch;    // override for tests/polyfills
}

export interface GetPresetsParams {
  pageSize?: number;
  pageIndex?: number;
  type?: PresetType;
  category?: PresetCategory;
}

export interface PredictionOptions {
  language?: string;      // ISO-639-1 (en, tr, es, ...)
  aspectBased?: boolean;  // default false
}

class KimolaApiError extends Error {
  status: number;
  bodyText?: string;
  constructor(message: string, status: number, bodyText?: string) {
    super(message);
    this.name = "KimolaApiError";
    this.status = status;
    this.bodyText = bodyText;
  }
}

export class KimolaClient {
  private readonly apiKey: string;
  private readonly baseUrl: string;
  private readonly _fetch: typeof fetch;

  constructor(opts: ClientOptions) {
    if (!opts?.apiKey) throw new Error("KimolaClient: 'apiKey' is required.");
    this.apiKey = opts.apiKey;
    this.baseUrl = (opts.baseUrl ?? "https://api.kimola.com/v1").replace(/\/+$/, "");
    this._fetch = opts.fetch ?? globalThis.fetch;
    if (!this._fetch) throw new Error("KimolaClient: global 'fetch' not found. Use Node 18+ or provide opts.fetch.");
  }

  // ---- low-level request ----
  private async request<T>(path: string, init?: RequestInit): Promise<T> {
    const url = `${this.baseUrl}${path}`;
    const res = await this._fetch(url, {
      ...init,
      headers: {
        "Authorization": `Bearer ${this.apiKey}`,
        ...(init?.method && init.method !== "GET" ? { "Content-Type": "application/json" } : {}),
        ...(init?.headers ?? {})
      }
    });

    if (!res.ok) {
      const text = await res.text().catch(() => undefined);
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
      return res.json() as Promise<T>;
    }
    return (await res.text()) as unknown as T;
  }

  // ---- Presets ----
  async getPresets(params: GetPresetsParams = {}): Promise<Paginated<PresetSummary>> {
    const qp = new URLSearchParams();
    if (params.pageSize != null) qp.set("pageSize", String(params.pageSize));
    if (params.pageIndex != null) qp.set("pageIndex", String(params.pageIndex));
    if (params.type) qp.set("type", params.type);
    if (params.category) qp.set("category", params.category);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/presets${qs}`);
  }

  async getPreset(key: string): Promise<Preset> {
    this.ensureKey(key);
    return this.request(`/presets/${encodeURIComponent(key)}`);
  }

  async getPresetLabels(key: string): Promise<Label[] | null> {
    this.ensureKey(key);
    return this.request(`/presets/${encodeURIComponent(key)}/labels`);
  }

  async createPrediction(key: string, text: string, opts: PredictionOptions = {}): Promise<any> {
    this.ensureKey(key);
    if (typeof text !== "string") throw new Error("createPrediction: 'text' must be a string.");
    const qp = new URLSearchParams();
    if (opts.language) qp.set("language", opts.language);
    if (opts.aspectBased === true) qp.set("aspectBased", "true");
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    const body = JSON.stringify(text); // raw JSON string per docs
    return this.request(`/presets/${encodeURIComponent(key)}/predictions${qs}`, {
      method: "POST",
      body
    });
  }

  // ---- Queries ----
  async getQueries(params: QueryListParams = {}): Promise<QueryItem[]> {
    const qp = new URLSearchParams();
    if (params.pageIndex != null) qp.set("pageIndex", String(params.pageIndex));
    if (params.pageSize != null) qp.set("pageSize", String(params.pageSize));
    if (params.startDate) qp.set("startDate", params.startDate);
    if (params.endDate) qp.set("endDate", params.endDate);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/queries${qs}`);
  }

  async getQueriesStatistics(params: { startDate?: string; endDate?: string } = {}): Promise<QueryStat[]> {
    const qp = new URLSearchParams();
    if (params.startDate) qp.set("startDate", params.startDate);
    if (params.endDate) qp.set("endDate", params.endDate);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/queries/statistics${qs}`);
  }

  // ---- Subscription ----
  async getSubscriptionUsage(date?: string): Promise<SubscriptionUsage> {
    const qp = new URLSearchParams();
    if (date) qp.set("date", date);
    const qs = qp.toString() ? `?${qp.toString()}` : "";
    return this.request(`/subscription/usage${qs}`);
  }

  // ---- Helpers ----
  async *iteratePresets(params: Omit<GetPresetsParams, "pageIndex"> = {}, maxPages = 100) {
    let pageIndex = 0, pages = 0;
    while (pages++ < maxPages) {
      const page = await this.getPresets({ ...params, pageIndex });
      for (const item of page.items) yield item;
      const size = params.pageSize ?? 10;
      pageIndex += 1;
      if (page.items.length < size) break;
    }
  }

  private ensureKey(key: string) {
    if (!key || typeof key !== "string") throw new Error("Preset 'key' is required.");
  }
}
