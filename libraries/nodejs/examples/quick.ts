import { KimolaClient } from "../src/index.ts";

const kimola = new KimolaClient({ apiKey: process.env.KIMOLA_API_KEY! });

async function main() {
  const { items } = await kimola.getPresets({ pageSize: 5 });
  console.log("Presets:", items.map(p => p.slug));

  if (items[0]) {
    const labels = await kimola.getPresetLabels(items[0].key);
    console.log("Labels:", labels);
  }

  const usage = await kimola.getSubscriptionUsage();
  console.log("Usage:", usage.query);
}

main().catch(err => {
  console.error(err);
  process.exitCode = 1;
});
