type PresetType = "Extractor" | "Classifier";
type PresetCategory = "Sentiment Classifier" | "Content Classifier";
interface Paginated<T> {
    total: number;
    items: T[];
    pageIndex?: number;
    pageSize?: number;
}
interface PresetSummary {
    key: string;
    slug: string;
    name: string;
    [k: string]: unknown;
}
interface Preset extends PresetSummary {
}
interface Label {
    name: string;
    description: string;
}
interface QueryListParams {
    pageIndex?: number;
    pageSize?: number;
    startDate?: string;
    endDate?: string;
}
interface QueryItem {
    report?: {
        code: string;
        name?: string;
        title?: string;
    };
    item?: {
        code: string;
        name?: string;
        type?: string;
    };
    type: "Classification" | "Tracking" | "Scraping" | string;
    amount: number;
    date: string;
}
interface QueryStat {
    name: "Classification" | "Tracking" | "Scraping" | string;
    count: number;
    ratio: number;
    percentage: number;
}
interface SubscriptionCounter {
    count: number;
    limit: number;
    percentage: number;
    available: number;
}
interface SubscriptionUsage {
    link: SubscriptionCounter;
    model: SubscriptionCounter;
    query: SubscriptionCounter;
    keyword: SubscriptionCounter;
}
interface ClientOptions {
    apiKey: string;
    baseUrl?: string;
    fetch?: typeof fetch;
}
interface GetPresetsParams {
    pageSize?: number;
    pageIndex?: number;
    type?: PresetType;
    category?: PresetCategory;
}
interface PredictionOptions {
    language?: string;
    aspectBased?: boolean;
}
declare class KimolaClient {
    private readonly apiKey;
    private readonly baseUrl;
    private readonly _fetch;
    constructor(opts: ClientOptions);
    private request;
    getPresets(params?: GetPresetsParams): Promise<Paginated<PresetSummary>>;
    getPreset(key: string): Promise<Preset>;
    getPresetLabels(key: string): Promise<Label[] | null>;
    createPrediction(key: string, text: string, opts?: PredictionOptions): Promise<any>;
    getQueries(params?: QueryListParams): Promise<QueryItem[]>;
    getQueriesStatistics(params?: {
        startDate?: string;
        endDate?: string;
    }): Promise<QueryStat[]>;
    getSubscriptionUsage(date?: string): Promise<SubscriptionUsage>;
    iteratePresets(params?: Omit<GetPresetsParams, "pageIndex">, maxPages?: number): AsyncGenerator<PresetSummary, void, unknown>;
    private ensureKey;
}

export { type ClientOptions, type GetPresetsParams, KimolaClient, type Label, type Paginated, type PredictionOptions, type Preset, type PresetCategory, type PresetSummary, type PresetType, type QueryItem, type QueryListParams, type QueryStat, type SubscriptionCounter, type SubscriptionUsage };
