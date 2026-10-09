"""Read release files as data; never load or execute the target assembly."""
import argparse
import collections
import hashlib
import json
from pathlib import Path
import struct
import subprocess
import zipfile

import dnfile
import py7zr


def sha(data):
    return hashlib.sha256(data).hexdigest()


def inspect(path):
    pe = dnfile.dnPE(str(path))
    result = {
        "file": path.name, "size": path.stat().st_size,
        "sha256": sha(path.read_bytes()), "machine": hex(pe.FILE_HEADER.Machine),
        "subsystem": pe.OPTIONAL_HEADER.Subsystem, "managed": bool(pe.net),
        "authenticode_directory_size": pe.OPTIONAL_HEADER.DATA_DIRECTORY[4].Size,
        "imports": [x.dll.decode() for x in getattr(pe, "DIRECTORY_ENTRY_IMPORT", [])],
    }
    if not pe.net:
        result["exports"] = [x.name.decode() for x in
                             getattr(getattr(pe, "DIRECTORY_ENTRY_EXPORT", None), "symbols", [])
                             if x.name]
        return result
    tables = pe.net.mdtables
    result.update({
        "clr_metadata_version": pe.net.metadata.struct.Version.decode().rstrip("\0"),
        "clr_flags": hex(pe.net.struct.Flags),
        "entrypoint_token": hex(pe.net.struct.EntryPointTokenOrRva),
        "type_definitions": len(tables.TypeDef.rows),
        "method_definitions": len(tables.MethodDef.rows),
        "module_mvid": str(tables.Module.rows[0].Mvid),
        "namespaces": dict(collections.Counter(str(x.TypeNamespace) for x in tables.TypeDef)),
    })
    def identity(row):
        return {"name": str(row.Name), "version": ".".join(str(getattr(row, field)) for field in
                ("MajorVersion", "MinorVersion", "BuildNumber", "RevisionNumber"))}
    result["assemblies"] = [identity(x) for x in tables.Assembly]
    result["references"] = [dict(identity(x), public_key_token=x.PublicKey.value.hex())
                            for x in tables.AssemblyRef]
    result["resources"] = []
    for row in tables.ManifestResource or []:
        # Each embedded resource begins with its little-endian byte count.
        rva = pe.net.struct.ResourcesRva + row.Offset
        size = struct.unpack("<I", pe.get_data(rva, 4))[0]
        data = pe.get_data(rva + 4, size)
        if len(data) != size:
            raise ValueError("Truncated resource")
        result["resources"].append({"name": str(row.Name), "size": size, "sha256": sha(data)})
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--ilspy", type=Path, required=True)
    parser.add_argument("--references", type=Path, required=True)
    args = parser.parse_args()
    repo, out = args.repo.resolve(), args.output.resolve()
    if out == repo or repo in out.parents:
        raise ValueError("Use an output directory outside the checkout")
    if out.exists() and any(out.iterdir()):
        raise ValueError("Output must be empty; do not overwrite previous analysis")
    originals = {p.name: {"size": p.stat().st_size, "sha256": sha(p.read_bytes())}
                 for p in sorted(repo.glob("ChickenAuto*")) if p.is_file()}
    out.mkdir(parents=True, exist_ok=True)
    extracted = out / "original-extracted"
    extracted.mkdir()
    with zipfile.ZipFile(repo / "ChickenAutoEx-new.zip") as archive:
        if archive.namelist() != ["ChickenAutoEx.exe"] or archive.testzip() is not None:
            raise ValueError("Unexpected ZIP layout or failed integrity check")
        payload = archive.read("ChickenAutoEx.exe")
    expected = "11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573"
    if sha(payload) != expected:
        raise ValueError("Different release; review its provenance before proceeding")
    target = extracted / "ChickenAutoEx-zip.exe"
    target.write_bytes(payload)
    wrapper = repo / "ChickenAutoEx-new.exe"
    offset = dnfile.dnPE(str(wrapper)).get_overlay_data_start_offset()
    overlay = wrapper.read_bytes()[offset:]
    if not overlay.startswith(b"7z\xbc\xaf\x27\x1c"):
        raise ValueError("Unrecognized standalone wrapper")
    archive_path = extracted / "wrapper.7z"
    archive_path.write_bytes(overlay)
    for label, path in [("7z", repo / "ChickenAutoEx-new.7z"), ("wrapper", archive_path)]:
        with py7zr.SevenZipFile(path) as archive:
            if archive.getnames() != ["ChickenAutoEx.exe"]:
                raise ValueError("Unexpected archive entries; refusing extraction")
            archive.extractall(extracted / label)
        if (extracted / label / "ChickenAutoEx.exe").read_bytes() != payload:
            raise ValueError("Release payloads differ")
    pe = dnfile.dnPE(str(target))
    for resource in pe.net.resources:
        name = str(resource.name)
        if name in ("TinhKiemAuto.EasyHook.dll", "TinhKiemAuto.Newtonsoft.Json.dll"):
            (extracted / name.removeprefix("TinhKiemAuto.")).write_bytes(resource.data)
    inventory = {
        "original_files": originals, "wrapper_overlay_offset": offset,
        "all_ex_payloads_identical": True,
        "executable": inspect(target), "standalone_wrapper": inspect(wrapper),
        "embedded_dependencies": [inspect(extracted / name) for name in
                                  ("EasyHook.dll", "Newtonsoft.Json.dll")],
    }
    (out / "inventory.json").write_text(json.dumps(inventory, indent=2) + "\n")
    command = [str(args.ilspy.resolve()), "--disable-updatecheck", "-lv", "CSharp7_3",
               "-r", str(args.references.resolve()), "-p", "-o", str(out / "decompiled"), str(target)]
    with (out / "decompiler.log").open("w") as log:
        subprocess.run(command, stdout=log, stderr=subprocess.STDOUT, check=True)
    after = {p.name: {"size": p.stat().st_size, "sha256": sha(p.read_bytes())}
             for p in sorted(repo.glob("ChickenAuto*")) if p.is_file()}
    if originals != after:
        raise ValueError("Original-file preservation check failed")
    print("Static recovery complete; original release files unchanged. No target was executed.")


if __name__ == "__main__":
    main()
