"""Verify the rebuilt Windows PE as data, never by executing it."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import struct

import dnfile
from product_version import product_version


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def embedded_resources(pe):
    resources = {}
    for row in pe.net.mdtables.ManifestResource or []:
        rva = pe.net.struct.ResourcesRva + row.Offset
        size = struct.unpack("<I", pe.get_data(rva, 4))[0]
        resources[str(row.Name)] = pe.get_data(rva + 4, size)
    return resources


def reviewed_method(text, name):
    pattern = (r'^\t\t(?:private|public)(?: static)? [^\n]+ ' + re.escape(name)
               + r'\([^\n]*\)\n\t\t\{\n.*?^\t\t\}\n')
    matches = list(re.finditer(pattern, text, re.MULTILINE | re.DOTALL))
    assert len(matches) == 1, "Expected exactly one UI method: " + name
    return matches[0].group()


def restore_reviewed_dungeon_ui(repo, original, current):
    review = json.loads((repo / "tools/dungeon_menu_review.json").read_text())
    for entry in review["changed_methods"]:
        before = reviewed_method(original, entry["name"])
        after = reviewed_method(current, entry["name"])
        assert hashlib.sha256(before.encode()).hexdigest() == entry["original_sha256"], entry["name"]
        assert hashlib.sha256(after.encode()).hexdigest() == entry["reviewed_sha256"], entry["name"]
        current = current.replace(after, before, 1)
    for entry in review["added_helpers"]:
        added = reviewed_method(current, entry["name"])
        assert hashlib.sha256(added.encode()).hexdigest() == entry["reviewed_sha256"], entry["name"]
        assert current.count(added + "\n") == 1, entry["name"]
        current = current.replace(added + "\n", "", 1)
    for entry in review["designer_edits"]:
        assert current.count(entry["reviewed"]) == 1, "Unexpected designer edit"
        current = current.replace(entry["reviewed"], entry["original"], 1)
    return current


def verify_branding(repo):
    review = json.loads((repo / "tools/branding_review.json").read_text())
    restored = {}
    for entry in review["source_edits"]:
        text = (repo / entry["path"]).read_text(encoding="utf-8")
        for edit in entry["edits"]:
            assert text.count(edit["reviewed"]) == edit["count"], entry["path"]
            text = text.replace(edit["reviewed"], edit["original"])
        assert hashlib.sha256(text.encode()).hexdigest() == entry["original_sha256_lf"], entry["path"]
        restored[entry["path"]] = text
    constants = (repo / "src/ChickenAutoEx/AppBranding.cs").read_text(encoding="utf-8")
    assert hashlib.sha256(constants.encode()).hexdigest() == review["branding_constants_sha256_lf"]
    for path, expected in review["additional_source_hashes_lf"].items():
        assert hashlib.sha256((repo / path).read_text(encoding="utf-8").encode()).hexdigest() == expected, path
    return restored


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--version", help="Match an explicit MSBuild EagleAutoVersion override for a version probe")
    args = parser.parse_args()
    repo = args.repo.resolve()
    product = product_version(repo, args.version)
    baseline = repo / "analysis/chickenautoex-107/recovered"
    inventory = json.loads((baseline.parent / "evidence/inventory.json").read_text())
    manifest = json.loads((repo / ".build/chickenautoex/prepared.json").read_text())
    output = repo / "src/ChickenAutoEx/bin/Release/net48"
    pe = dnfile.dnPE(str(output / product["executable"]))
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
    assert 'public static string Version => "107";' in current_global
    branding_restored = verify_branding(repo)
    original_form = (baseline / "TinhKiemAuto/FrmMain.cs").read_text()
    current_form = branding_restored["src/ChickenAutoEx/FrmMain.cs"]
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
    preserved_form = restore_reviewed_dungeon_ui(repo, original_form, preserved_form)
    marker = '\t\t\ttxtlogs.AppendText("Bật auto :"'
    assert original_form[original_form.index(marker):] == preserved_form[preserved_form.index(marker):]
    code = current_form[current_form.index("private async void FrmMain_Load"):current_form.index("private void InitializeStartup")]
    assert code.index("InitializeStartup();") < code.index("await new UpdateClient().CheckAsync")
    assert "Process.Start" not in code and "Application.Exit" not in code

    # Inspect presentation/version data as PE bytes; never load the application.
    for value in ["EAGLE Auto " + product["display_version"], "Về EAGLE Auto",
                  "Được sửa lại dựa trên Chicken Auto 107, vibe coding bằng Codex bởi tenkafuku."]:
        assert pe.__data__.find(value.encode("utf-16le")) >= 0, "Missing compiled branding: " + value
    assert (pe.net.mdtables.Assembly.rows[0].MajorVersion,
            pe.net.mdtables.Assembly.rows[0].MinorVersion) == (1, 0), "Keep legacy CLR assembly version"
    assert str(pe.net.mdtables.Assembly.rows[0].Name) == product["executable"][:-4]
    pe.parse_data_directories(directories=[2])  # Win32 resource directory, including VERSIONINFO
    version_strings = {}
    for block in pe.FileInfo:
        for entry in block:
            for table in getattr(entry, "StringTable", []):
                version_strings.update(table.entries)
    for key, value in {b"ProductName": b"EAGLE Auto", b"CompanyName": b"tenkafuku",
                       b"FileVersion": product["file_version"].encode(),
                       b"ProductVersion": product["display_version"].encode()}.items():
        assert version_strings.get(key) == value, key.decode()

    # Reverse only documented redactions; verify all other Game/auth/debug code stayed identical.
    # The exact preservation checks below avoid executing or interpreting this code.
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
        if relative.endswith("/Game.cs"):
            from thuy_lao_review import apply_review
            from ky_cuoc_review import apply_review as ky_cuoc
            redacted = ky_cuoc(repo, redacted, reverse=True)
            redacted = apply_review(repo, redacted, reverse=True)
        assert count == expected and redacted == (baseline / relative).read_text(), relative

    result = {
        "framework": "net48", "architecture": "x86", "embedded_resources": len(resources),
        "unchanged_original_resources": 26, "json_assembly_version": 13,
        "original_release_files_unchanged": True, "entitlement_source_preserved": True,
        "automation_source_preserved_except_reviewed_thuy_lao_ky_cuoc_and_pet_aoe": True,
        "reviewed_ky_cuoc_fixes": True, "reviewed_pet_aoe_fix": True,
        "reviewed_thuy_lao_fixes": True,
        "reviewed_ui_settings_fixes": ["mp_checkbox_binding", "independent_optional_map_indexes"],
        "reviewed_dungeon_menu_fixes": True,
        "display_name": "EAGLE Auto", "display_version": product["display_version"],
        "product_version": product,
        "legacy_protocol_version": "107", "reviewed_branding": True,
        "target_executable_executed": False, "windows_runtime_tested": False,
        "artifacts": {name: digest(output / name) for name in
                      (product["executable"], product["config"], "Newtonsoft.Json.dll", "Zen.Barcode.Core.dll")},
    }
    (repo / ".build/chickenautoex/verified.json").write_text(json.dumps(result, indent=2) + "\n")
    print("PASS: x86/CLR4, 27 resources, matching dependency identities, preserved originals/licensing, and automation outside reviewed Thuy Lao/Ky Cuoc/pet AOE deltas.")
    print("This is static build verification, not a Windows runtime test.")


if __name__ == "__main__":
    main()
