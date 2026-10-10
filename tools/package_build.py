"""Package verified build outputs; do not package sources or private keys."""
import argparse
import hashlib
import json
from pathlib import Path
import zipfile


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    args = parser.parse_args()
    repo = args.repo.resolve()
    verified = json.loads((repo / ".build/chickenautoex/verified.json").read_text())
    output = repo / "src/ChickenAutoEx/bin/Release/net48"
    for name, expected in verified["artifacts"].items():
        if hashlib.sha256((output / name).read_bytes()).hexdigest() != expected:
            raise ValueError("Run verify_build.py again after changing output")
    artifacts = repo / ".build/artifacts"
    artifacts.mkdir(exist_ok=True)
    path = artifacts / "EAGLE-Auto-v0.1-net48.zip"
    with zipfile.ZipFile(path, "w", zipfile.ZIP_DEFLATED) as archive:
        for name in verified["artifacts"]:
            archive.write(output / name, name)
        archive.write(repo / "LICENSE", "LICENSE")
        archive.write(repo / "docs/ChickenAutoEx-WINDOWS11.vi.md", "WINDOWS11.vi.md")
        archive.write(repo / "docs/ChickenAutoEx-UI-SETTINGS.vi.md", "ChickenAutoEx-UI-SETTINGS.vi.md")
        archive.write(repo / "docs/ChickenAutoEx-DUNGEON-MENU.vi.md", "ChickenAutoEx-DUNGEON-MENU.vi.md")
        archive.write(repo / "docs/EAGLE-Auto-v0.1.vi.md", "EAGLE-Auto-v0.1.vi.md")
        # Preserve available third-party package notices; do not claim licenses were relicensed.
        archive.write(repo / "docs/ChickenAutoEx-DEPENDENCIES.md", "DEPENDENCIES.md")
    checksum = hashlib.sha256(path.read_bytes()).hexdigest()
    (artifacts / (path.name + ".sha256")).write_text(checksum + "  " + path.name + "\n")
    print("Packaged reviewed build files:", path)
    print("SHA256:", checksum)
    print("Windows runtime testing remains outstanding; this package is not a validated release.")


if __name__ == "__main__":
    main()
