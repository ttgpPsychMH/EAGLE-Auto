"""Restore build-only assets from the pinned release without executing it.

Generated source contains original embedded key material. Keep the output in
the ignored .build directory; do not print literals or commit generated files.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil
import stat
import struct
import subprocess
import tempfile
import zipfile

import dnfile


PAYLOAD_SHA256 = "11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573"
LITERAL = r'"(?:\\.|[^"\\])*"'
SITES = {
    "TinhKiemAuto/Debug.cs": ("TinhKiemAuto.Debug", r'dictionary\.Add\("key", ' + LITERAL + r'\);', 1),
    "TinhKiemAuto/Game.cs": ("TinhKiemAuto.Game", r'TINHKIEM\.Hasher\.Decrypt\(' + LITERAL + r', ' + LITERAL + r'\)', 1),
    "TinhKiemAuto.Models/LoadFile.cs": ("TinhKiemAuto.Models.LoadFile", r'string password = ' + LITERAL + r';', 2),
    "TinhKiemAuto.Models/TienIch.cs": ("TinhKiemAuto.Models.TienIch", r'return GetMD5\(' + LITERAL + r' \+ time\);', 1),
}


def digest(data):
    return hashlib.sha256(data).hexdigest()


def package_file(assets, package, version, relative):
    for directory in assets["packageFolders"]:
        path = Path(directory) / package.lower() / version / relative
        if path.is_file():
            return path
    raise ValueError("Restore required package: " + package + " " + version)


def pe_resource(pe, kind, identifier=None):
    kinds = [e for e in pe.DIRECTORY_ENTRY_RESOURCE.entries if e.id == kind]
    if len(kinds) != 1:
        raise ValueError("Unexpected PE resource type")
    entries = kinds[0].directory.entries
    item = entries[0] if identifier is None else next(e for e in entries if e.id == identifier)
    data = item.directory.entries[0].data.struct
    return pe.get_data(data.OffsetToData, data.Size)


def extract_icon(pe):
    group = pe_resource(pe, 14)
    reserved, icon_type, count = struct.unpack_from("<HHH", group)
    if reserved != 0 or icon_type != 1 or count < 1:
        raise ValueError("Invalid icon directory")
    entries, bodies, offset = [], [], 6 + count * 16
    for index in range(count):
        width, height, colors, zero, planes, bits, size, identifier = struct.unpack_from("<BBBBHHIH", group, 6 + index * 14)
        body = pe_resource(pe, 3, identifier)
        if len(body) != size:
            raise ValueError("Invalid icon image size")
        entries.append(struct.pack("<BBBBHHII", width, height, colors, zero, planes, bits, size, offset))
        bodies.append(body)
        offset += size
    return struct.pack("<HHH", 0, 1, count) + b"".join(entries + bodies)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--dotnet", default="dotnet")
    args = parser.parse_args()
    repo = args.repo.resolve()
    baseline = repo / "analysis/chickenautoex-107/recovered"
    assets = json.loads((repo / "src/ChickenAutoEx/obj/project.assets.json").read_text())
    framework = package_file(assets, "Microsoft.NETFramework.ReferenceAssemblies.net35", "1.0.3",
                             "build/.NETFramework/v3.5/mscorlib.dll").parent
    new_json = package_file(assets, "Newtonsoft.Json", "13.0.4", "lib/net45/Newtonsoft.Json.dll")
    barcode = package_file(assets, "Zen.Barcode.Rendering.Framework", "3.1.10729.1", "lib/Zen.Barcode.Core.dll")
    original_hashes = {p.name: digest(p.read_bytes()) for p in repo.glob("ChickenAuto*") if p.is_file()}
    with zipfile.ZipFile(repo / "ChickenAutoEx-new.zip") as archive:
        if archive.namelist() != ["ChickenAutoEx.exe"] or archive.testzip() is not None:
            raise ValueError("Unexpected release archive")
        payload = archive.read("ChickenAutoEx.exe")
    if digest(payload) != PAYLOAD_SHA256:
        raise ValueError("The selected release differs from the reviewed binary")

    root = repo / ".build"
    root.mkdir(exist_ok=True, mode=0o700)
    # TemporaryDirectory avoids accepting a partially prepared tree after failure.
    with tempfile.TemporaryDirectory(prefix="prepare-", dir=root) as directory:
        work = Path(directory)
        target = work / "ChickenAutoEx.exe"
        target.write_bytes(payload)
        references = work / "references"
        references.mkdir()
        for path in framework.glob("*.dll"):
            shutil.copy2(path, references / path.name)
        shutil.copy2(barcode, references / barcode.name)
        pe = dnfile.dnPE(str(target))
        resource_path = work / "resources"
        resource_path.mkdir()
        resource_hashes = {}
        for row in pe.net.mdtables.ManifestResource:
            name = str(row.Name)
            if Path(name).name != name or "/" in name or "\\" in name:
                raise ValueError("Unsafe resource name")
            rva = pe.net.struct.ResourcesRva + row.Offset
            size = struct.unpack("<I", pe.get_data(rva, 4))[0]
            data = pe.get_data(rva + 4, size)
            if len(data) != size:
                raise ValueError("Truncated resource")
            if name == "TinhKiemAuto.Newtonsoft.Json.dll":
                # The extracted assembly is used only to interpret legacy IL.
                (references / "Newtonsoft.Json.dll").write_bytes(data)
                data = new_json.read_bytes()
            (resource_path / name).write_bytes(data)
            resource_hashes[name] = digest(data)
        (work / "app.ico").write_bytes(extract_icon(pe))
        hydrated = work / "hydrated"
        hydrated.mkdir()
        source_hashes = {}
        for relative, (type_name, pattern, expected) in SITES.items():
            result = subprocess.run([args.dotnet, "tool", "run", "ilspycmd", "--", "--disable-updatecheck",
                                     "-lv", "CSharp7_3", "-r", str(references), "-t", type_name, str(target)],
                                    cwd=repo, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=True)
            raw = result.stdout.decode("utf-8-sig")
            originals = re.findall(pattern, raw)
            if len(originals) != expected:
                raise ValueError("Unexpected literal layout for " + relative)
            review = (baseline / relative).read_text()
            # Keep every non-sensitive token of the reviewed file identical.
            review_pattern = pattern.replace(LITERAL, r'"\[REDACTED\]"(?: /\* analysis redaction \*/)?')
            # Game's marker occurs just before the closing parenthesis.
            if relative.endswith("Game.cs"):
                review_pattern = r'TINHKIEM\.Hasher\.Decrypt\("\[REDACTED\]", "\[REDACTED\]" /\* analysis redaction \*/\)'
            matches = list(re.finditer(review_pattern, review))
            if len(matches) != expected:
                raise ValueError("Review snapshot changed: " + relative)
            for match, original in reversed(list(zip(matches, originals))):
                review = review[:match.start()] + original + review[match.end():]
            if relative.endswith("/Game.cs"):
                from thuy_lao_review import apply_review
                review = apply_review(repo, review)
                from ky_cuoc_review import apply_review as ky_cuoc
                review = ky_cuoc(repo, review)
                from trung_ac_review import apply_review as trung_ac
                review = trung_ac(repo, review)
                from ac_tac_review import apply_review as ac_tac
                review = ac_tac(repo, review)
                from ac_ba_review import apply_review as ac_ba
                review = ac_ba(repo, review)
            path = hydrated / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text(review)
            path.chmod(stat.S_IRUSR | stat.S_IWUSR)
            source_hashes[relative] = digest(path.read_bytes())
        final = root / "chickenautoex"
        previous_manifest = final / "prepared.json"
        if final.exists():
            if not previous_manifest.is_file() or json.loads(previous_manifest.read_text()).get("generator") != "tools/prepare_build.py":
                raise ValueError("Refusing to overwrite an unknown build directory")
            shutil.rmtree(final)
        final.mkdir(mode=0o700)
        shutil.move(str(hydrated), final / "hydrated")
        shutil.move(str(resource_path), final / "resources")
        shutil.move(str(work / "app.ico"), final / "app.ico")
        manifest = {"generator": "tools/prepare_build.py", "original_payload_sha256": PAYLOAD_SHA256,
                    "target_framework": "net48", "json_package": "13.0.4",
                    "resource_hashes": resource_hashes, "hydrated_source_hashes": source_hashes}
        (final / "prepared.json").write_text(json.dumps(manifest, indent=2) + "\n")
    if original_hashes != {p.name: digest(p.read_bytes()) for p in repo.glob("ChickenAuto*") if p.is_file()}:
        raise ValueError("Original file preservation failed")
    from thuy_lao_review import write_regression_source
    write_regression_source(repo)
    from ky_cuoc_review import write_regression_source as write_ky_cuoc
    write_ky_cuoc(repo)
    from trung_ac_review import write_regression_source as write_trung_ac
    write_trung_ac(repo)
    from ac_tac_review import write_regression_source as write_ac_tac
    write_ac_tac(repo)
    from ac_ba_review import write_regression_source as write_ac_ba
    write_ac_ba(repo)
    from dungeon_reaudit_review import baseline_partial
    for module in ("ThuyLao", "KyCuoc"):
        (repo / (".build/dungeon-reaudit/" + module + "-v03.cs")).parent.mkdir(parents=True, exist_ok=True)
        (repo / (".build/dungeon-reaudit/" + module + "-v03.cs")).write_text(
            baseline_partial(repo, "src/ChickenAutoEx/" + module + "/Game." + module + ".cs"), encoding="utf-8")
    print("Prepared 27 embedded resources, icon and four hydrated source files; originals unchanged.")
    print("Generated key material is confined to ignored .build; no release executable was run.")


if __name__ == "__main__":
    main()
