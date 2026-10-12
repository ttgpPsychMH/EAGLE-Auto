// Optional visual check of our HTML mockup; never executes the automation app.
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const { chromium } = require(process.env.PLAYWRIGHT_MODULE_PATH || 'playwright');
const repo = path.resolve(__dirname, '..');
const work = path.join(repo, '.build', 'ui-preview');
const htmlFile = path.join(repo, 'docs', 'ui', 'EAGLE-Auto-v0.8-preview.html');
for (const name of ['', 'xdg-cache', 'xdg-config']) fs.mkdirSync(path.join(work, name), { recursive: true });
(async () => {
  let browser;
  try {
    browser = await chromium.launch({
      executablePath: process.env.CHROMIUM_EXECUTABLE || '/usr/bin/chromium', headless: true, timeout: 20000,
      args: ['--no-sandbox', '--disable-dev-shm-usage'],
      env: { ...process.env, XDG_CACHE_HOME: path.join(work, 'xdg-cache'), XDG_CONFIG_HOME: path.join(work, 'xdg-config') }
    });
    const page = await browser.newPage({ viewport: { width: 1100, height: 1000 } });
    const html = fs.readFileSync(htmlFile, 'utf8');
    // Render owned text in memory: no file navigation, external website or local server.
    await page.evaluate(() => { location.hash = '#selftest'; });
    await page.setContent(html, { timeout: 20000 });
    const result = await page.evaluate(() => ({ result: document.body.dataset.previewResult, tests: Number(document.body.dataset.previewTests) }));
    if (result.result !== 'passed' || result.tests !== 11) throw new Error('Mockup assertions did not complete.');
    const preview = await browser.newPage({ viewport: { width: 1100, height: 1000 } });
    await preview.setContent(html, { timeout: 20000 });
    await preview.waitForFunction(() => document.getElementById('state').textContent.includes('Chưa bật'));
    const screenshot = path.join(work, 'preview.png');
    await preview.screenshot({ path: screenshot, fullPage: true });
    fs.writeFileSync(path.join(work, 'summary.json'), JSON.stringify({ ...result,
      browser: browser.version(), html_sha256: crypto.createHash('sha256').update(html).digest('hex'),
      screenshot_sha256: crypto.createHash('sha256').update(fs.readFileSync(screenshot)).digest('hex'),
      scope: 'Owned HTML mockup with synthetic data only; not a WinForms or game test.' }, null, 2) + '\n');
    console.log('PASS: 11 HTML mockup checks; .build/ui-preview/preview.png. WinForms remains untested.');
  } finally { if (browser) await browser.close(); }
})().catch(error => { console.error(error.name + ': ' + error.message.slice(0, 500)); process.exitCode = 1; });
