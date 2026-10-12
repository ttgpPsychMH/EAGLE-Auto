// Validate only our owned HTML and privately uploaded font files in Chromium.
// Never runs the automation EXE, native DLL or game; never writes font bytes to artifacts.
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const { chromium } = require(process.env.PLAYWRIGHT_MODULE_PATH || 'playwright');
const repo = path.resolve(__dirname, '..');
const work = path.join(repo, '.build', 'ui-preview');
const htmlFile = path.join(repo, 'docs', 'ui', 'EAGLE-Auto-v0.10-preview.html');
const args = process.argv.slice(2);
if (args.length !== 2 || args[0] !== '--font-dir') {
  console.error('Usage: node tools/check_appearance_preview.cjs --font-dir PRIVATE_APTOS_FONT_DIRECTORY');
  process.exit(2);
}
const fontDir = path.resolve(args[1]);
const fontDefinitions = [
  { file: 'Aptos.ttf', style: 'normal', weight: '400' },
  { file: 'Aptos-Bold.ttf', style: 'normal', weight: '700' },
  { file: 'Aptos-Italic.ttf', style: 'italic', weight: '400' },
  { file: 'Aptos-Bold-Italic.ttf', style: 'italic', weight: '700' }
];
const sha256 = value => crypto.createHash('sha256').update(value).digest('hex');
for (const name of ['', 'xdg-cache', 'xdg-config']) fs.mkdirSync(path.join(work, name), { recursive: true });
(async () => {
  let browser;
  try {
    const html = fs.readFileSync(htmlFile, 'utf8');
    if (/data:[^;]*font|data:application\/(?:octet-stream|x-font)/i.test(html))
      throw new Error('Committed HTML must not embed font data.');
    const logoFile = path.join(repo, 'assets', 'branding', 'eagle-auto.png');
    const logoBytes = fs.readFileSync(logoFile);
    const logoData = 'data:image/png;base64,' + logoBytes.toString('base64');
    const fonts = fontDefinitions.map(definition => {
      const bytes = fs.readFileSync(path.join(fontDir, definition.file));
      if (!bytes.length) throw new Error('A font file is empty.');
      return { ...definition, sha256: sha256(bytes), data: bytes.toString('base64') };
    });
    if (new Set(fonts.map(font => font.sha256)).size !== 4)
      throw new Error('Four distinct Aptos face files are required.');
    browser = await chromium.launch({
      executablePath: process.env.CHROMIUM_EXECUTABLE || '/usr/bin/chromium', headless: true, timeout: 20000,
      args: ['--no-sandbox', '--disable-dev-shm-usage'],
      env: { ...process.env, XDG_CACHE_HOME: path.join(work, 'xdg-cache'), XDG_CONFIG_HOME: path.join(work, 'xdg-config') }
    });
    const page = await browser.newPage({ viewport: { width: 620, height: 930 } });
    const blockedRequests = [];
    await page.route('**/*', route => { blockedRequests.push(route.request().url()); return route.abort(); });
    // The image and fonts are read locally and supplied in memory. No network or file navigation.
    const memoryHtml = html.replaceAll('../../assets/branding/eagle-auto.png', logoData);
    await page.setContent(memoryHtml, { timeout: 20000 });
    await page.evaluate(async definitions => {
      await Promise.all(definitions.map(async definition => {
        const face = new FontFace('Aptos', `url(data:font/ttf;base64,${definition.data})`,
          { style: definition.style, weight: definition.weight });
        await face.load();
        document.fonts.add(face);
      }));
      await Promise.all(['12px Aptos', 'bold 12px Aptos', 'italic 12px Aptos', 'italic bold 12px Aptos']
        .map(font => document.fonts.load(font, 'Ác Bá – Trân Long Kỳ Cuộc – Trừng Ác – Thủy Lao')));
      await document.fonts.ready;
      document.body.dataset.actualAptos = 'loaded';
    }, fonts);
    const uiResult = await page.evaluate(() => window.eaglePreview.runSelfTests());
    if (uiResult.result !== 'passed' || uiResult.tests !== 45 ||
        !Array.isArray(uiResult.checks) || uiResult.checks.length !== uiResult.tests)
      throw new Error('The 45 inherited HTML UI checks did not complete.');
    const appearanceResult = await page.evaluate(async () => {
      const checks = [];
      function check(name, ok) { if (!ok) throw new Error('HTML appearance check failed: ' + name); checks.push(name); }
      const $ = id => document.getElementById(id);
      const fonts = ['12px Aptos', 'bold 12px Aptos', 'italic 12px Aptos', 'italic bold 12px Aptos'];
      fonts.forEach(font => check('loaded font face ' + font, document.fonts.check(font, 'Được sửa lại – Trừng Ác – Thủy Lao')));
      check('body uses Aptos first', getComputedStyle(document.body).fontFamily.split(',')[0].trim() === 'Aptos');
      check('all form controls inherit Aptos', [...document.querySelectorAll('button,input,select,dialog')]
        .every(control => getComputedStyle(control).fontFamily.split(',')[0].trim() === 'Aptos'));
      await Promise.all([...document.images].map(image => image.decode()));
      check('both new eagle images decode', document.images.length === 2 && [...document.images].every(image => image.naturalWidth > 0));
      check('title uses new eagle logo', document.querySelector('.brand-logo').alt === 'Logo đại bàng EAGLE Auto');
      check('application version is v0.10', document.title.includes('v0.10') && document.querySelector('.brand').textContent.includes('v0.10'));
      check('no chicken title or old logo', !/ChickenAuto|🔥|🐔|🐓/.test(document.querySelector('.title').textContent));
      $('menuAbout').click();
      check('About opens a modal dialog', $('aboutDialog').open && $('aboutDialog').matches(':modal'));
      check('About has the new eagle logo', $('aboutDialog').querySelector('img').naturalWidth > 0);
      check('About keeps requested attribution', $('aboutAttribution').textContent === 'Được sửa lại dựa trên Chicken Auto 107, vibe coding bằng Codex bởi tenkafuku.');
      check('About reports four loaded Aptos faces', $('aboutFontStatus').textContent.includes('Aptos (4 kiểu đã nạp'));
      check('About makes browser test scope explicit', $('aboutDialog').textContent.includes('WinForms trên Windows'));
      $('closeAbout').click();
      check('About closes with its button', !$('aboutDialog').open);
      $('menuAbout').click();
      document.dispatchEvent(new Event('appearance-about-ready'));
      return { result: 'passed', tests: checks.length, checks };
    });
    await page.keyboard.press('Escape');
    const aboutEscapeWorks = await page.locator('#aboutDialog').evaluate(dialog => !dialog.open);
    if (!aboutEscapeWorks) throw new Error('About Escape did not close the modal dialog.');
    appearanceResult.checks.push('About closes with Escape');
    appearanceResult.tests++;
    // Chromium reports the actual font used for Vietnamese glyphs, rather than only a CSS family string.
    const client = await page.context().newCDPSession(page);
    await client.send('DOM.enable');
    await client.send('CSS.enable');
    const glyphText = 'Ác Bá – Trân Long Kỳ Cuộc – Trừng Ác – Thủy Lao – Được sửa lại';
    await page.evaluate(definitions => {
      const host = document.createElement('div'); host.id = 'font-probe'; host.className = 'font-probe';
      for (const definition of definitions) {
        const probe = document.createElement('div');
        probe.id = 'face-' + definition.style + '-' + definition.weight;
        probe.style.fontStyle = definition.style; probe.style.fontWeight = definition.weight;
        probe.textContent = 'Ác Bá – Trân Long Kỳ Cuộc – Trừng Ác – Thủy Lao – Được sửa lại';
        host.appendChild(probe);
      }
      document.body.appendChild(host);
    }, fontDefinitions);
    await page.evaluate(() => document.fonts.ready);
    const documentNode = await client.send('DOM.getDocument');
    const renderedFonts = [];
    for (const definition of fontDefinitions) {
      const selector = `#face-${definition.style}-${definition.weight}`;
      const { nodeId } = await client.send('DOM.querySelector', { nodeId: documentNode.root.nodeId, selector });
      const { fonts: actualFonts } = await client.send('CSS.getPlatformFontsForNode', { nodeId });
      if (!actualFonts.length || actualFonts.some(font => !font.isCustomFont || !/aptos/i.test(font.familyName)) ||
          !actualFonts.some(font => font.glyphCount > 0))
        throw new Error('Vietnamese glyphs did not render entirely with the uploaded Aptos face: ' + selector);
      renderedFonts.push({ style: definition.style, weight: definition.weight, fonts: actualFonts });
      appearanceResult.checks.push(`actual uploaded Aptos renders Vietnamese glyphs ${definition.style} ${definition.weight}`);
      appearanceResult.tests++;
    }
    await page.evaluate(() => document.getElementById('font-probe').remove());
    await client.detach();
    if (blockedRequests.length !== 0) throw new Error('HTML attempted an unexpected external resource request.');
    appearanceResult.checks.push('zero network requests from owned HTML');
    appearanceResult.tests++;
    const screenshots = {};
    for (const mode of ['selected', 'empty', 'about']) {
      await page.evaluate(value => window.eaglePreview.prepareScreenshot(value), mode);
      await page.evaluate(() => document.fonts.ready);
      const screenshot = path.join(work, `v0.10-${mode}.png`);
      await page.locator('#window').screenshot({ path: screenshot });
      screenshots[mode] = { file: path.basename(screenshot), sha256: sha256(fs.readFileSync(screenshot)) };
    }
    const summary = {
      result: 'passed', tests: uiResult.tests + appearanceResult.tests,
      ui: uiResult, appearance: appearanceResult, renderedFonts, glyphText,
      browser: browser.version(), html_sha256: sha256(html), logo_sha256: sha256(logoBytes),
      fontFiles: fonts.map(({ file, style, weight, sha256 }) => ({ file, style, weight, sha256 })),
      networkRequests: 0, screenshots,
      scope: 'Owned HTML mockup with fake data and privately uploaded Aptos faces. Actual Chromium glyph rendering only; WinForms, Windows DPI, native DLL and game remain untested.',
      fontDistribution: 'Uploaded TTF bytes exist only in ignored local input files and browser memory. No font bytes are saved in HTML, screenshots metadata, package or Git.'
    };
    fs.writeFileSync(path.join(work, 'v0.10-summary.json'), JSON.stringify(summary, null, 2) + '\n');
    console.log(`PASS: ${uiResult.tests} inherited HTML UI checks + ${appearanceResult.tests} appearance/font checks; three v0.10 screenshots. WinForms remains untested.`);
  } finally { if (browser) await browser.close(); }
})().catch(error => { console.error(error.name + ': ' + error.message.slice(0, 500)); process.exitCode = 1; });
