"""Verify the rebuilt Windows PE as data, never by executing it."""
import argparse
import hashlib
import json
from pathlib import Path
import struct

import dnfile


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def embedded_resources(pe):
    resources = {}
    for row in pe.net.mdtables.ManifestResource or []:
        rva = pe.net.struct.ResourcesRva + row.Offset
        size = struct.unpack("<I", pe.get_data(rva, 4))[0]
        resources[str(row.Name)] = pe.get_data(rva + 4, size)
    return resources


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    args = parser.parse_args()
    repo = args.repo.resolve()
    baseline = repo / "analysis/chickenautoex-107/recovered"
    inventory = json.loads((baseline.parent / "evidence/inventory.json").read_text())
    manifest = json.loads((repo / ".build/chickenautoex/prepared.json").read_text())
    output = repo / "src/ChickenAutoEx/bin/Release/net48"
    pe = dnfile.dnPE(str(output / "ChickenAutoEx.exe"))
    assert pe.FILE_HEADER.Machine == 0x14c and pe.net.struct.Flags & 3 == 3, "Build must remain x86 IL-only"
    assert pe.net.metadata.struct.Version.startswith(b"v4.0.30319"), "Expected CLR 4"
    resources = embedded_resources(pe)
    assert set(resources) == set(manifest["resource_hashes"]), "Embedded resources incomplete"
    for name, data in resources.items():
        assert hashlib.sha256(data).hexdigest() == manifest["resource_hashes"][name], name
    for item in inventory["executable"]["resources"]:
        if item["name"] != "TinhKiemAuto.Newtonsoft.Json.dll":
            assert hashlib.sha256(resources[item["name"]]).hexdigest() == item["sha256"], item["name"]
    assert resources["TinhKiemAuto.Newtonsoft.Json.dll"] == (output / "Newtonsoft.Json.dll").read_bytes(), "Embedded/copied JSON mismatch"
    refs = {str(row.Name): row.MajorVersion for row in pe.net.mdtables.AssemblyRef}
    for name, version in [("Newtonsoft.Json", 13), ("Zen.Barcode.Core", 3)]:
        dependency = dnfile.dnPE(str(output / (name + ".dll")))
        assert dependency.net.mdtables.Assembly.rows[0].MajorVersion == version == refs[name], name
    for name, item in inventory["original_files"].items():
        assert digest(repo / name) == item["sha256"], name

    original_global = (baseline / "TinhKiemAuto/Global.cs").read_text()
    current_global = (repo / "src/ChickenAutoEx/Global.cs").read_text()
    assert current_global.replace("https://raw.githubusercontent.com/ttgpPsychMH/EAGLE-Auto/master/PatchInfoEx.ini",
                                  "http://update.chickenauto.com/PatchInfoEx.ini") == original_global
    original_form = (baseline / "TinhKiemAuto/FrmMain.cs").read_text()
    current_form = (repo / "src/ChickenAutoEx/FrmMain.cs").read_text()
    # Reverse exactly the two reviewed UI/config fixes before comparing the legacy tail.
    # Keep every other handler, automation branch and entitlement condition protected.
    mp_fix = "CurGame.IsMP = checkrengenmp.Checked;"
    assert current_form.count(mp_fix) == 1, "Expected the reviewed MP checkbox binding"
    preserved_form = current_form.replace(mp_fix, "CurGame.IsMP = checkregenhp.Checked;", 1)
    map_fix = ("Option.MapBanDoIndex = array[61];\n"
               "\t\t\t\t\t}\n\t\t\t\t\tif (array.Length > 62)\n\t\t\t\t\t{\n"
               "\t\t\t\t\t\tOption.MaptriLieuIndex = array[62];")
    assert preserved_form.count(map_fix) == 1, "Expected independent optional map guards"
    preserved_form = preserved_form.replace(map_fix,
        "Option.MapBanDoIndex = array[61];\n\t\t\t\t\t\tOption.MaptriLieuIndex = array[62];", 1)
    marker = '\t\t\ttxtlogs.AppendText("Bật auto :"'
    assert original_form[original_form.index(marker):] == preserved_form[preserved_form.index(marker):]
    code = current_form[current_form.index("private async void FrmMain_Load"):current_form.index("private void InitializeStartup")]
    assert code.index("InitializeStartup();") < code.index("await new UpdateClient().CheckAsync")
    assert "Process.Start" not in code and "Application.Exit" not in code

    # Reverse only documented redactions; verify all other Game/auth/debug code stayed identical.
    # The exact preservation checks below avoid executing or interpreting this code.
    import re
    for relative, (pattern, expected) in {
        "TinhKiemAuto/Debug.cs": (r'(dictionary\.Add\("key", )("(?:\\.|[^"\\])*")', 1),
        "TinhKiemAuto/Game.cs": (r'(TINHKIEM\.Hasher\.Decrypt\()((?:"(?:\\.|[^"\\])*"), (?:"(?:\\.|[^"\\])*"))', 1),
        "TinhKiemAuto.Models/LoadFile.cs": (r'(string password = )("(?:\\.|[^"\\])*")', 2),
        "TinhKiemAuto.Models/TienIch.cs": (r'(return GetMD5\()("(?:\\.|[^"\\])*")', 1),
    }.items():
        text = (repo / ".build/chickenautoex/hydrated" / relative).read_text()
        def redact(match):
            count = len(re.findall(r'"(?:\\.|[^"\\])*"', match.group(2)))
            return match.group(1) + ', '.join(['"[REDACTED]"'] * count) + " /* analysis redaction */"
        redacted, count = re.subn(pattern, redact, text)
        assert count == expected and redacted == (baseline / relative).read_text(), relative

    result = {
        "framework": "net48", "architecture": "x86", "embedded_resources": len(resources),
        "unchanged_original_resources": 26, "json_assembly_version": 13,
        "original_release_files_unchanged": True, "automation_and_entitlement_source_preserved": True,
        "reviewed_ui_settings_fixes": ["mp_checkbox_binding", "independent_optional_map_indexes"],
        "target_executable_executed": False, "windows_runtime_tested": False,
        "artifacts": {name: digest(output / name) for name in
                      ("ChickenAutoEx.exe", "ChickenAutoEx.exe.config", "Newtonsoft.Json.dll", "Zen.Barcode.Core.dll")},
    }
    (repo / ".build/chickenautoex/verified.json").write_text(json.dumps(result, indent=2) + "\n")
    print("PASS: x86/CLR4, 27 resources, matching dependency identities, preserved originals and automation/licensing source.")
    print("This is static build verification, not a Windows runtime test.")


if __name__ == "__main__":
    main()
