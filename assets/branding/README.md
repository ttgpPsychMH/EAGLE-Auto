# EAGLE Auto branding

`eagle-auto.png` is the transparent production asset derived from the user's latest front-facing eagle reference using image editing. `eagle-auto.ico` is the same asset encoded at 16, 24, 32, 48, 64, 128 and 256 pixels for Windows. The earlier side-profile VectorStock reference is not used.

The PNG and ICO are embedded as `EagleAuto.Branding.Logo` and `EagleAuto.Branding.Icon`; the ICO also supplies the Win32 executable icon. Legacy release resources/snapshots remain unchanged for verification, while displayed app icons use these assets.

Aptos font binaries are deliberately absent. Users supply their local fonts; see `docs/fonts/APTOS-LOCAL.vi.md`.
