"""Package only the source-owned read-only monitor and analysis evidence."""
import hashlib
from pathlib import Path
import zipfile

ROOT = Path(__file__).resolve().parents[3]
output = ROOT / ".build/multi-client-107/EAGLE-MultiClient-Diagnostics.zip"
output.parent.mkdir(parents=True, exist_ok=True)
files = {
    "EAGLE-MultiClient-Monitor.exe": ROOT / "analysis/multi-client-107/windows/bin/Release/net48/EAGLE-MultiClient-Monitor.exe",
    "EAGLE-MultiClient-Monitor.exe.config": ROOT / "analysis/multi-client-107/windows/bin/Release/net48/EAGLE-MultiClient-Monitor.exe.config",
    "WINDOWS-TEST.vi.md": ROOT / "analysis/multi-client-107/WINDOWS-TEST.vi.md",
    "REPORT.vi.md": ROOT / "analysis/multi-client-107/REPORT.vi.md",
    "results-template.csv": ROOT / "analysis/multi-client-107/results-template.csv",
    "simulation-summary.json": ROOT / ".build/multi-client-107/summary.json",
    "preservation.json": ROOT / ".build/multi-client-107/preservation.json",
}
with zipfile.ZipFile(output, "w", compression=zipfile.ZIP_DEFLATED) as archive:
    for name, source in files.items():
        assert source.is_file(), source
        archive.write(source, name)
with zipfile.ZipFile(output) as archive:
    assert archive.testzip() is None
    assert set(archive.namelist()) == set(files)
digest = hashlib.sha256(output.read_bytes()).hexdigest()
output.with_suffix(".zip.sha256").write_text(digest + "  " + output.name + "\n")
print("Packaged diagnostics only:", output.name, digest)
