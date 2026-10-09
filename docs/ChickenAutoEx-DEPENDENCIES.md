# Dependency provenance for the recovered build

This is a review build, not a published Windows-tested release.

| Component | Source/version | License/provenance |
|---|---|---|
| Recovered ChickenAutoEx | Pinned payload SHA256 `11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573` | Original repository MIT notice is retained in LICENSE. |
| Newtonsoft.Json | NuGet 13.0.4, net45 DLL, assembly 13.0.0.0 | MIT, James Newton-King; package metadata and license apply. Same DLL is copied and embedded. |
| Zen.Barcode.Rendering.Framework | NuGet 3.1.10729.1, assembly Zen.Barcode.Core 3.1.0.0 | NuGet metadata credits Zen Design Corp 2008–2011 and references the original CodePlex license. Original upstream license text was not recovered here; verify redistribution terms before publishing. |
| Native Bin/EasyHook.dll | Original embedded x86 PE; SHA256 `1017f117193687302d3817d66cb9e7914c026d78aced4a02517a5c940efe5d79` | Preserved unchanged. It is not replaced with the similarly named managed EasyHook package. Native source/provenance beyond the release is unavailable. |
| ProtoBuf / ComponentAce zlib / other merged code | Already merged into original assembly; recovered source retained | Original per-component notices/version provenance are incomplete. No relicensing claim is made. |
| Framework references | Microsoft.NETFramework.ReferenceAssemblies.net48 1.0.3 | Build only; Microsoft package terms. Windows provides the actual .NET Framework runtime. |
| Recovery framework references | net35 package 1.0.3 | Decompiler only, not the application target. |
| ILSpyCmd | 9.1.0.7988 | Open-source decompiler; restored via pinned local tool manifest. Not bundled with application. |

No package signature, artifact checksum or TLS validation was disabled during preparation. Restore uses committed NuGet lockfiles. Generated source/binaries retain legacy key material from the original release; keep them out of Git and do not use embedded third-party API credentials. The updater never sends credentials and never downloads executables.
