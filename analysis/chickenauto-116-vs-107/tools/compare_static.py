"""Reproduce a static comparison. Never execute or load either application/DLL.

Raw extracted assets stay outside the checkout. Output JSON contains PE metadata,
Qt reflection, UI translations and explicitly selected configuration names only.
Offsets and payload hashes intentionally identify the reviewed releases exactly.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import struct
import subprocess
import sys
import zipfile
import zlib

import dnfile
import py7zr


STABLE_SHA = "34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea"
EX_SHA = "11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573"
QM_MAGIC = bytes.fromhex("3cb86418caef9c95cd211cbf60a1bddd")
CLASSES = [
    "MainWindow", "DebugMessageTextarea", "WorkerDetectGameInstance", "WorkerUpdateUi",
    "GameInstanceListWidget", "game_controllers::ShortcutKeysManager",
    "others_ui::AdsDialog", "others_ui::ChangelogDialog", "others_ui::SettingAutoDialog",
    "others_ui::IgnoreMonsterListDialog", "others_ui::SettingListPlayerGetLotusBuffHp",
    "others_ui::ThrowTrashItemDialog", "setting_tabs::BasicSettings",
    "setting_tabs::QuestSettings", "setting_tabs::SkillSettings", "setting_tabs::UtilitiesSettings",
]
CONFIG_NAMES = [
    "AutoSettings.json", "enableAuto", "enableAutoAttack", "enableAutoKs", "enableUseF1", "ignoreMonsterList",
    "enableAttackFollowLeader", "enableAttackRadius", "attackRadiusPoint.x()",
    "attackRadiusPoint.y()", "attackRadiusMap", "enableUseSkillPet", "autoRecoverHp",
    "autoRecoverHpPercent", "autoRecoverMp", "autoRecoverMpPercent", "autoRecoverPetHp",
    "autoRecoverPetHpPercent", "autoUsePetRecoverHpSkill", "autoUsePetRecoverHpSkillPercent",
    "autoUsePetRecoverMpSkill", "autoUsePetRecoverMpSkillPercent", "enableAutoLotusBuffHp",
    "autoLotusBuffHpPercent", "autoLotusBuffPlayerList", "enableAutoActionWhenDead",
    "autoLogoutGameAfterDeadSeconds", "enableAutoAlertHp", "autoAlertHpPercent",
    "enableAutoLevelup", "autoLevelupTo", "enableAutoUseX2dot5ExpItem", "enableAutoUseSkill",
    "autoSkillList", "inventoryBagPassword", "enableAutoAcceptSceneTransfer",
    "enableAutoAcceptTeamRequest", "enableAutoPickupLootPackage", "enableAutoThrowTrashItem",
    "autoThrowTrashItemList", "enableAutoSummonPet", "autoSummonPetId", "enableAutoResetOnlineTime",
    "AutoSettings::gameExePath", "AutoSettings::enableAlertSound", "AutoSettings::enableMinimizeToSystemTray",
    "AutoSettings::enableConfirmAlertBeforeClosing", "AutoSettings::enableShortcutKeys",
    "Bin/ChickenAuto.log", "Bin/EasyHook.dll", ":/Resources/EasyHook.dll", "LuaPlus.dll", "UI_CEGUI.dll",
    "WorkerDetectGameInstance::updateAuto", "Version", "NewAutoUrl", "NewAutoUrl_1",
    "NewAutoUrl_2", "NewAutoUrl_3", "NewAutoUrl_4", "Data/(ChickenAuto_commands)",
]


def sha(data):
    return hashlib.sha256(data).hexdigest()


def write_json(path, value):
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n")


def qrc_files(data):
    # Qt rcc v1 arrays, verified from the registration call at VA 0x4014f0.
    names, tree, payloads = 0x97b5c8, 0x97b7e0, 0x878610
    visited, result = set(), []

    def walk(index, parent):
        if index in visited or index >= 40:
            raise ValueError("Unexpected Qt resource tree")
        visited.add(index)
        offset = tree + 14 * index
        name_offset, flags = struct.unpack_from(">IH", data, offset)
        length = struct.unpack_from(">H", data, names + name_offset)[0]
        name = data[names + name_offset + 6:names + name_offset + 6 + length * 2].decode("utf-16be")
        if "/" in name or "\\" in name or name in (".", ".."):
            raise ValueError("Unexpected resource name")
        path = parent + "/" + name if index else ""
        if flags & 2:
            count, first = struct.unpack_from(">II", data, offset + 6)
            for child in range(first, first + count):
                walk(child, path)
        else:
            country, language, relative = struct.unpack_from(">HHI", data, offset + 6)
            start = payloads + relative
            size = struct.unpack_from(">I", data, start)[0]
            raw = data[start + 4:start + 4 + size]
            if len(raw) != size:
                raise ValueError("Truncated resource")
            decoded = raw
            if flags & 1:
                decoded = zlib.decompress(raw[4:])
                if len(decoded) != struct.unpack_from(">I", raw)[0]:
                    raise ValueError("Resource decompression size mismatch")
            result.append(({
                "path": ":" + path, "flags": flags, "stored_size": size,
                "decoded_size": len(decoded), "sha256": sha(decoded),
                "file_offset": hex(start + 4), "country": country, "language": language,
            }, decoded))

    walk(0, "")
    if len(result) != 13:
        raise ValueError("Unexpected resource count")
    return result


def qm_messages(data, base):
    if not data.startswith(QM_MAGIC):
        raise ValueError("Missing Qt QM magic")
    cursor, messages = 16, []
    while cursor < len(data):
        block = data[cursor]
        size = struct.unpack_from(">I", data, cursor + 1)[0]
        cursor += 5
        end = cursor + size
        if end > len(data):
            raise ValueError("Truncated QM block")
        if block != 0x69:
            cursor = end
            continue
        current = {}
        while cursor < end:
            offset, tag = cursor, data[cursor]
            cursor += 1
            if tag == 1:
                if current:
                    messages.append(current)
                current = {}
                continue
            if tag == 5:
                cursor += 4
                continue
            names = {2: "source16", 3: "translation", 4: "context16", 6: "source", 7: "context", 8: "comment"}
            if tag not in names:
                raise ValueError("Unsupported QM message tag")
            count = struct.unpack_from(">I", data, cursor)[0]
            cursor += 4
            if count == 0xffffffff:
                count = 0
            if cursor + count > end:
                raise ValueError("Truncated QM message")
            value = data[cursor:cursor + count].decode("utf-16be" if tag in (2, 3, 4) else "utf-8")
            current[names[tag]] = value
            current.setdefault("file_offset", hex(base + offset))
            cursor += count
        if cursor != end or current:
            raise ValueError("Invalid QM termination")
    return messages


def meta_class(data, pe, name):
    position = data.find(name.encode(), 0x97ba00, 0x97dd00)
    if position < 0:
        raise ValueError("Missing reviewed Qt class: " + name)
    header = None
    for offset in range(position - 6000, position - 15, 4):
        refcount, length, alloc, relative = struct.unpack_from("<iiiI", data, offset)
        if refcount == -1 and length == len(name) and alloc == 0 and offset + relative == position:
            header = offset
            break
    if header is None or (position - header) % 16:
        raise ValueError("Invalid Qt string table")
    count = (position - header) // 16
    strings, tail = [], position
    for index in range(count):
        offset = header + index * 16
        refcount, length, alloc, relative = struct.unpack_from("<iiiI", data, offset)
        if refcount != -1 or length < 0 or length > 500 or alloc != 0:
            raise ValueError("Invalid Qt string record")
        start = offset + relative
        strings.append(data[start:start + length].decode("utf-8"))
        tail = max(tail, start + length + 1)
    address = pe.OPTIONAL_HEADER.ImageBase + pe.get_rva_from_offset(header)
    candidates = []
    for match in re.finditer(re.escape(struct.pack("<I", address)), data):
        pointer = match.start()
        if not 0x97b7e0 <= pointer <= 0x97dd00:
            continue
        metadata_va, static_call = struct.unpack_from("<II", data, pointer + 4)
        try:
            metadata = pe.get_offset_from_rva(metadata_va - pe.OPTIONAL_HEADER.ImageBase)
        except Exception:
            continue
        if not tail <= metadata <= tail + 16:
            continue
        fields = struct.unpack_from("<14I", data, metadata)
        if fields[0] != 7 or fields[1] != 0 or fields[4] > 100:
            continue
        methods = []
        for index in range(fields[4]):
            method = struct.unpack_from("<5I", data, metadata + 4 * (fields[5] + index * 5))
            methods.append({"name": strings[method[0]], "argument_count": method[1], "flags": hex(method[4])})
        candidates.append({"class": name, "string_table_offset": hex(header), "metadata_offset": hex(metadata),
                           "revision": fields[0], "method_count": fields[4], "signal_count": fields[13],
                           "static_metacall_va": hex(static_call), "methods": methods})
    if len(candidates) != 1:
        raise ValueError("Missing or ambiguous Qt metaobject: " + name)
    return candidates[0]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, required=True)
    parser.add_argument("--work", type=Path, required=True)
    parser.add_argument("--ilspy", type=Path, required=True)
    args = parser.parse_args()
    repo, work = args.repo.resolve(), args.work.resolve()
    if work == repo or repo in work.parents:
        raise ValueError("Use an empty workspace outside the checkout")
    if work.exists() and any(work.iterdir()):
        raise ValueError("Refusing to overwrite previous work")
    original = {p.name: sha(p.read_bytes()) for p in repo.glob("ChickenAuto*")
                if p.is_file() and p.suffix in (".exe", ".zip", ".7z")}
    baseline = json.loads((repo / "analysis/chickenautoex-107/evidence/inventory.json").read_text())
    if original != {name: value["sha256"] for name, value in baseline["original_files"].items()}:
        raise ValueError("Original releases differ from the reviewed baseline")
    work.mkdir(parents=True, exist_ok=True, mode=0o700)
    evidence = work / "evidence"
    evidence.mkdir()
    with zipfile.ZipFile(repo / "ChickenAuto-new.zip") as archive:
        if archive.namelist() != ["ChickenAuto.exe"] or archive.testzip() is not None:
            raise ValueError("Unexpected stable ZIP")
        stable = archive.read("ChickenAuto.exe")
    if sha(stable) != STABLE_SHA:
        raise ValueError("Stable payload changed")
    with zipfile.ZipFile(repo / "ChickenAutoEx-new.zip") as archive:
        if archive.namelist() != ["ChickenAutoEx.exe"] or archive.testzip() is not None:
            raise ValueError("Unexpected Ex ZIP")
        if sha(archive.read("ChickenAutoEx.exe")) != EX_SHA or baseline["executable"]["sha256"] != EX_SHA:
            raise ValueError("Ex metadata/payload provenance changed")
    target = work / "ChickenAuto-116.exe"
    target.write_bytes(stable)
    pe = dnfile.dnPE(data=stable)
    if pe.net or pe.OPTIONAL_HEADER.DATA_DIRECTORY[14].Size != 0:
        raise ValueError("Expected a native payload")
    wrapper = dnfile.dnPE(str(repo / "ChickenAuto-new.exe"))
    overlay = wrapper.get_overlay_data_start_offset()
    archive_path = work / "wrapper.7z"
    archive_path.write_bytes((repo / "ChickenAuto-new.exe").read_bytes()[overlay:])
    for label, path in [("7z", repo / "ChickenAuto-new.7z"), ("wrapper", archive_path)]:
        with py7zr.SevenZipFile(path) as archive:
            if archive.getnames() != ["ChickenAuto.exe"]:
                raise ValueError("Unexpected archive members")
            archive.extractall(work / label)
        if (work / label / "ChickenAuto.exe").read_bytes() != stable:
            raise ValueError("Stable payloads differ")
    sys.path.insert(0, str(repo / "analysis/chickenautoex-107/tools"))
    from recover import inspect
    resources = qrc_files(stable)
    resource_data = {item["path"]: (item, raw) for item, raw in resources}
    hook_info, hook = resource_data[":/Resources/EasyHook.dll"]
    hook_path = work / "EasyHook-116.dll"
    hook_path.write_bytes(hook)
    hook_116 = inspect(hook_path)
    hook_107 = next(item for item in baseline["embedded_dependencies"] if item["file"] == "EasyHook.dll")
    qm_info, qm = resource_data[":/res/languages/AutoTLBB_vi.qm"]
    messages = qm_messages(qm, int(qm_info["file_offset"], 16))
    if stable[0x99d768:0x99d76e] != b"5.7.1\0":
        raise ValueError("Expected Qt version literal")
    if len(messages) != 256:
        raise ValueError("Unexpected translation count")
    # This deliberately exposes no arbitrary strings from executable/data sections.
    for message in messages:
        if re.search(r'(?:password|token|api[_-]?key)\s*[=:]\s*[^%\s]', message.get("source", ""), re.I):
            raise ValueError("Review possible sensitive UI literal before exporting")
    classes = [meta_class(stable, pe, name) for name in CLASSES]
    config = []
    for name in CONFIG_NAMES:
        # Require the whole zero-terminated UTF-8 token, not a substring.
        pattern = rb'(?<=\x00)' + re.escape(name.encode()) + rb'\x00'
        hits = [hex(m.start()) for m in re.finditer(pattern, stable) if 0x823f00 <= m.start() <= 0x82b000]
        config.append({"literal": name, "file_offsets": hits})
    if any(not item["file_offsets"] for item in config):
        raise ValueError("A reviewed configuration literal was not found")
    command = [str(args.ilspy.resolve()), "--disable-updatecheck", "-lv", "CSharp7_3", "-p",
               "-o", str(work / "ilspy-output"), str(target)]
    result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    expected_error = "PE file does not contain any managed metadata."
    if result.returncode == 0 or expected_error not in result.stdout.decode(errors="replace"):
        raise ValueError("Unexpected native ILSpy result; inspect tool output privately")
    (evidence / "ilspy-116.txt").write_text("ILSpyCmd 9.1.0.7988, CSharp7_3 project decompilation\n"
                                          + "Exit code: " + str(result.returncode) + "\n"
                                          + "MetadataFileNotSupportedException: " + expected_error + "\n")
    quest = next(item for item in classes if item["class"] == "setting_tabs::QuestSettings")
    address = int(quest["static_metacall_va"], 16)
    native = subprocess.run(["objdump", "-d", "--start-address=" + hex(address),
                             "--stop-address=" + hex(address + 3), str(target)],
                            stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=True)
    if b"ret" not in native.stdout:
        raise ValueError("Unexpected QuestSettings metacall body")
    # Normalize the raw workspace path; this is only a three-byte dispatch stub.
    (evidence / "quest-metacall-116.txt").write_text(native.stdout.decode().replace(str(target), "ChickenAuto-116.exe"))
    write_json(evidence / "inventory.json", {
        "original_files_sha256": original, "all_stable_payloads_identical": True,
        "stable_wrapper_overlay_offset": overlay, "stable_wrapper": inspect(repo / "ChickenAuto-new.exe"),
        "stable_116": inspect(target), "ex_107": baseline["executable"],
        "com_descriptor": {"rva": pe.OPTIONAL_HEADER.DATA_DIRECTORY[14].VirtualAddress,
                           "size": pe.OPTIONAL_HEADER.DATA_DIRECTORY[14].Size},
        "sections_116": [{"name": s.Name.decode().rstrip("\0"), "file_offset": hex(s.PointerToRawData),
                          "rva": hex(s.VirtualAddress), "raw_size": s.SizeOfRawData} for s in pe.sections],
        "qt_version_string": {"value": "5.7.1", "file_offset": "0x99d768",
                              "matched": stable[0x99d768:0x99d76e] == b"5.7.1\0"},
        "qt_resources_116": [info for info, _ in resources],
        "hook_116": hook_116, "hook_107": hook_107,
        "hook_bytes_identical": hook_116["sha256"] == hook_107["sha256"],
        "qt_translation_count": len(messages), "qt_translation_contexts": sorted({m.get("context", "") for m in messages}),
        "ilspy_116_exit_code": result.returncode, "ilspy_116_expected_error": expected_error,
        "target_binaries_executed": False, "windows_or_game_tests_run_by_analysis": False,
    })
    write_json(evidence / "qt-metaobjects-116.json", classes)
    write_json(evidence / "ui-translations-116.json", messages)
    write_json(evidence / "selected-literals-116.json", config)
    if original != {p.name: sha(p.read_bytes()) for p in repo.glob("ChickenAuto*")
                    if p.is_file() and p.suffix in (".exe", ".zip", ".7z")}:
        raise ValueError("Original preservation failed")
    print("PASS: identical stable archive payloads; native x86; 13 Qt resources; 256 translations; 16 Qt metaobjects.")
    print("PASS: ILSpy rejects native metadata; original releases preserved; neither executable/DLL was run.")
    print("Evidence ready for textual review:", evidence)


if __name__ == "__main__":
    main()
