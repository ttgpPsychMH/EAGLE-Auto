// Optional visual check of our HTML mockup; never executes the automation app.
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const { chromium } = require(process.env.PLAYWRIGHT_MODULE_PATH || 'playwright');
const repo = path.resolve(__dirname, '..');
const work = path.join(repo, '.build', 'ui-preview');
const htmlFile = path.join(repo, 'docs', 'ui', 'EAGLE-Auto-v0.9-preview.html');
for (const name of ['', 'xdg-cache', 'xdg-config']) fs.mkdirSync(path.join(work, name), { recursive: true });
(async () => {
  let browser;
  try {
    browser = await chromium.launch({
      executablePath: process.env.CHROMIUM_EXECUTABLE || '/usr/bin/chromium', headless: true, timeout: 20000,
      args: ['--no-sandbox', '--disable-dev-shm-usage'],
      env: { ...process.env, XDG_CACHE_HOME: path.join(work, 'xdg-cache'), XDG_CONFIG_HOME: path.join(work, 'xdg-config') }
    });
    const page = await browser.newPage({ viewport: { width: 620, height: 930 } });
    const html = fs.readFileSync(htmlFile, 'utf8');
    // Owned text in memory; reject any accidental network request from this mockup.
    await page.route('**/*', route => route.abort());
    await page.setContent(html, { timeout: 20000 });
    const result = await page.evaluate(() => window.eaglePreview.runSelfTests());
    if (result.result !== 'passed' || !Number.isInteger(result.tests) || result.tests < 15 ||
        !Array.isArray(result.checks) || result.checks.length !== result.tests)
      throw new Error('Mockup assertions did not complete or their reported count is inconsistent.');
    const screenshots = {};
    for (const mode of ['selected', 'empty']) {
      await page.evaluate(value => window.eaglePreview.prepareScreenshot(value), mode);
      const screenshot = path.join(work, `v0.9-${mode}.png`);
      await page.locator('#window').screenshot({ path: screenshot });
      screenshots[mode] = { file: path.basename(screenshot),
        sha256: crypto.createHash('sha256').update(fs.readFileSync(screenshot)).digest('hex') };
    }
    fs.writeFileSync(path.join(work, 'v0.9-summary.json'), JSON.stringify({ ...result,
      browser: browser.version(), html_sha256: crypto.createHash('sha256').update(html).digest('hex'), screenshots,
      scope: 'Owned HTML mockup with synthetic data only; not a WinForms, native DLL or game test.' }, null, 2) + '\n');
    console.log(`PASS: ${result.tests} HTML mockup checks; .build/ui-preview/v0.9-{selected,empty}.png. WinForms remains untested.`);
  } finally { if (browser) await browser.close(); }
})().catch(error => { console.error(error.name + ': ' + error.message.slice(0, 500)); process.exitCode = 1; });
