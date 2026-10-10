"""Package verified build outputs; do not package sources or private keys."""
import argparse
import hashlib
import json
from pathlib import Path
import zipfile
from product_version import product_version


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--version", help="Match an explicit MSBuild EagleAutoVersion override for a version probe")
    args = parser.parse_args()
    repo = args.repo.resolve()
    verified = json.loads((repo / ".build/chickenautoex/verified.json").read_text())
    product = product_version(repo, args.version)
    if verified["product_version"] != product:
        raise ValueError("Version changed since verification; build and verify again")
    output = repo / "src/ChickenAutoEx/bin/Release/net48"
    for name, expected in verified["artifacts"].items():
        if hashlib.sha256((output / name).read_bytes()).hexdigest() != expected:
            raise ValueError("Run verify_build.py again after changing output")
    artifacts = repo / ".build/artifacts"
    artifacts.mkdir(exist_ok=True)
    path = artifacts / product["zip_name"]
    with zipfile.ZipFile(path, "w", zipfile.ZIP_DEFLATED) as archive:
        for name in verified["artifacts"]:
            archive.write(output / name, name)
        archive.write(repo / "LICENSE", "LICENSE")
        archive.write(repo / "docs/ChickenAutoEx-WINDOWS11.vi.md", "WINDOWS11.vi.md")
        archive.write(repo / "docs/ChickenAutoEx-UI-SETTINGS.vi.md", "ChickenAutoEx-UI-SETTINGS.vi.md")
        archive.write(repo / "docs/ChickenAutoEx-DUNGEON-MENU.vi.md", "ChickenAutoEx-DUNGEON-MENU.vi.md")
        archive.write(repo / "docs/EAGLE-Auto.vi.md", "EAGLE-Auto.vi.md")
        archive.write(repo / "analysis/thuy-lao/REPORT.vi.md", "THUY-LAO-REVIEW.vi.md")
        archive.write(repo / "docs/EAGLE-Auto-THUY-LAO-SIMULATION.vi.md", "THUY-LAO-SIMULATION.vi.md")
        archive.write(repo / "docs/EAGLE-Auto-KY-CUOC-SIMULATION.vi.md", "KY-CUOC-SIMULATION.vi.md")
        archive.write(repo / "docs/EAGLE-Auto-TRUNG-AC-SIMULATION.vi.md", "TRUNG-AC-SIMULATION.vi.md")
        archive.write(repo / "docs/EAGLE-Auto-DUNGEON-REAUDIT.vi.md", "DUNGEON-REAUDIT.vi.md")
        archive.write(repo / "analysis/ac-tac/REPORT.vi.md", "AC-TAC-REVIEW.vi.md")
        archive.write(repo / "docs/EAGLE-Auto-AC-TAC-SIMULATION.vi.md", "AC-TAC-SIMULATION.vi.md")
        archive.write(repo / "analysis/ac-ba/REPORT.vi.md", "AC-BA-REVIEW.vi.md")
        archive.write(repo / "docs/EAGLE-Auto-AC-BA-SIMULATION.vi.md", "AC-BA-SIMULATION.vi.md")
        archive.writestr("BUILD-INFO.json", json.dumps(verified, indent=2) + "\n")
        # Preserve available third-party package notices; do not claim licenses were relicensed.
        archive.write(repo / "docs/ChickenAutoEx-DEPENDENCIES.md", "DEPENDENCIES.md")
    checksum = hashlib.sha256(path.read_bytes()).hexdigest()
    (artifacts / (path.name + ".sha256")).write_text(checksum + "  " + path.name + "\n")
    print("Packaged reviewed build files:", path)
    print("SHA256:", checksum)
    print("Windows runtime testing remains outstanding; this package is not a validated release.")


if __name__ == "__main__":
    main()
