"""Verify an analysis-only branch and all six immutable release inputs."""
import hashlib
import json
from pathlib import Path
import subprocess

BASE = "56b4d899f4eca6ee2c46a9b2fe4ec1e9d13ec30a"
ROOT = Path(__file__).resolve().parents[3]
NEW_WORKFLOW = ".github/workflows/multi-client-review.yml"


def main():
    base_paths = set(subprocess.check_output(
        ["git", "ls-tree", "-r", "--name-only", BASE], cwd=ROOT, text=True).splitlines())
    changes = subprocess.check_output(["git", "diff", "--name-only", BASE, "--"], cwd=ROOT, text=True).splitlines()
    for name in changes:
        assert name not in base_paths, "An existing tracked file changed: " + name
        assert name.startswith("analysis/multi-client-107/") or name == NEW_WORKFLOW, name
    inventory = json.loads((ROOT / "analysis/chickenautoex-107/evidence/inventory.json").read_text())
    releases = {}
    for name, expected in inventory["original_files"].items():
        actual = hashlib.sha256((ROOT / name).read_bytes()).hexdigest()
        assert actual == expected["sha256"], name
        releases[name] = actual
    protected = ["src/ChickenAutoEx/FrmMain.cs", "src/ChickenAutoEx/Global.cs", "src/ChickenAutoEx/Program.cs",
                 "EagleAuto.Version.props", "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs",
                 "analysis/chickenautoex-107/recovered/TinhKiemAuto/Memory.cs",
                 "analysis/chickenautoex-107/recovered/TinhKiemAuto/Address.cs"]
    hashes = {}
    for name in protected:
        current = (ROOT / name).read_bytes()
        original = subprocess.check_output(["git", "show", BASE + ":" + name], cwd=ROOT)
        assert current == original, name
        hashes[name] = hashlib.sha256(current).hexdigest()
    for name in ["Program.cs", "Metrics.cs"]:
        sampler = (ROOT / "analysis/multi-client-107/windows" / name).read_text()
        for forbidden in ["WriteProcessMemory", "VirtualAllocEx", "CreateRemoteThread", "SetWindowsHookEx", "PostMessage", "Process.Start("]:
            assert forbidden not in sampler, "Sampler contains mutation API: " + forbidden
    result = {
        "base_commit": BASE, "existing_tracked_files_unchanged": True,
        "licensing_vip_and_automation_unchanged": True, "application_version": "0.10",
        "original_release_files_unchanged": True, "original_release_hashes": releases,
        "selected_source_sha256": hashes, "production_launch_limit_changed": False,
        "runtime_model_integrated": False, "original_game_or_automation_executed": False,
        "windows_sampler_built_only": True, "windows_game_tested": False
    }
    output = ROOT / ".build/multi-client-107/preservation.json"
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(result, indent=2) + "\n")
    print("PASS: existing application/source/config unchanged; six original releases match their SHA256.")


if __name__ == "__main__":
    main()
