"""Collect static Ky Cuoc evidence; never load the automation assembly or its DLLs.

Run from the repo root: python analysis/tran-long-ky-cuoc/tools/collect_evidence.py
Optional --il points to ignored output freshly produced by trusted ILSpyCmd.
Only selected methods/snippets are exported; full IL can contain original secrets.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import sys
import zipfile

repo = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(repo / "tools"))
from thuy_lao_review import apply_review

BASELINE = "6d83466"
PAYLOAD = "11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573"
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--il", type=Path)
args = parser.parse_args()
baseline = repo / "analysis/chickenautoex-107/recovered"
game = (baseline / "TinhKiemAuto/Game.cs").read_text(encoding="utf-8")
reviewed = apply_review(repo, game)
assert apply_review(repo, reviewed, reverse=True) == game


def method(source, name, parameters=r"[^\n]*"):
    matches = list(re.finditer(
        r"^\t\t(?:private|public)(?: static)? [^\n]+ " + re.escape(name)
        + r"\(" + parameters + r"\)\n\t\t\{\n.*?^\t\t\}\n", source, re.M | re.S))
    assert len(matches) == 1, name
    return matches[0]


files = [repo / "src/ChickenAutoEx/FrmMain.cs", repo / "src/ChickenAutoEx/ChickenAutoEx.csproj",
         repo / "tools/thuy_lao_review.json", repo / "tools/dungeon_menu_review.json"]
files += [baseline / path for path in [
    "TinhKiemAuto/Game.cs", "TinhKiemAuto/TRANLONGKYCUOC.cs", "TinhKiemAuto/LACDUONG.cs",
    "TinhKiemAuto/MAP.cs", "TinhKiemAuto/TLBB.cs", "TinhKiemAuto/GameObjects.cs",
    "TinhKiemAuto/GameObject.cs", "TinhKiemAuto/QuestFrame.cs", "TinhKiemAuto/Task.cs",
    "TinhKiemAuto/TaskInfo.cs", "TinhKiemAuto/Address.cs", "TinhKiemAuto/Memory.cs",
    "FindPath.cs", "TinhKiemAuto.Models/Unity.cs", "TinhKiemAuto/LUA.cs", "TinhKiemAuto/Scripts.cs",
    "TinhKiemAuto/Script.cs", "TinhKiemAuto.Scripts.xml", "TinhKiemAuto.PathList.txt",
    "TinhKiemAuto.Screen.txt", "TinhKiemAuto.ImageResource.resx"]]
result = {"baseline_commit": BASELINE,
          "scope": "Static managed source/data/IL review, no engine simulation or Windows/game test",
          "application_executed": False, "native_dll_executed": False,
          "product_version_changed": False, "automation_changed": False,
          "files": {}, "state_references": {}, "core_methods": {}}
symbols = ["IsKyCuoc", "IsXongKyCuoc", "IsBossDie", "BossDieTime", "IsP", "ClearTime", "MoveIndex"]
for path in files:
    relative = str(path.relative_to(repo))
    source = path.read_text(encoding="utf-8")
    result["files"][relative] = {"sha256_bytes": hashlib.sha256(path.read_bytes()).hexdigest()}
    for symbol in symbols:
        lines = [i for i, line in enumerate(source.splitlines(), 1)
                 if re.search(r"\b" + symbol + r"\b", line)]
        if lines:
            result["state_references"].setdefault(symbol, {})[relative] = lines
for name, parameters in [("DatDoiKyCuoc", ""), ("DatDoiTKC", ""), ("P", ""), ("Auto", ""),
                         ("TrieuTap", ""), ("ClearMission", ""), ("SetCalendar", ""),
                         ("AOE", ""), ("Attack", ""), ("FollowKey", ""),
                         ("MoveNext", ""), ("MoveNext", r"int\[,] point")]:
    original = method(game, name, parameters)
    current = method(reviewed, name, parameters)
    result["core_methods"][name + "(" + parameters.replace("\\", "") + ")"] = {
        "snapshot_source_line": game[:original.start()].count("\n") + 1,
        "original_sha256_lf": hashlib.sha256(original.group().encode()).hexdigest(),
        "reviewed_sha256_lf": hashlib.sha256(current.group().encode()).hexdigest(),
        "unchanged_by_thuy_lao_review": original.group() == current.group()}

# These verify the reviewed code pattern, not actual game behavior.
assert re.findall(r"\bIsXongKyCuoc\s*=\s*(true|false)\s*;", reviewed) == ["true"]
kycuoc = method(reviewed, "DatDoiKyCuoc", "").group()
assert "Global.IsVIP" not in kycuoc
assert "TrieuTap();" in kycuoc and "if (TrieuTap" not in kycuoc
assert "QuestFrameOptionClicked(401001, -1);\n\t\t\t\t\t\t\tCloseQuest();" in kycuoc
completion = reviewed[reviewed.index("if (TLBB.MapId == MAP.TranLongKyCuoc && IsBossDie)"):]
completion = completion[:completion.index("if (IsSaveGold)")]
assert "IsKyCuoc" not in completion
assert "GoTo(TRANLONGKYCUOC.TeThanh);" in completion and "IsXongKyCuoc = true;" in completion
assert "20 - BossDieTime.Elapsed.Seconds" in completion and "TotalSeconds > 30.0" in completion
aoe = method(reviewed, "AOE", "").group()
assert "NearMonter20m.Count >= 0" in aoe and "Objects.Monter[0]" in aoe
p_method = method(reviewed, "P", "").group()
assert "QuestFrameOptionClicked(44000, 0);" in p_method
result["static_pattern_checks"] = {
    "completion_assignments": ["true"], "completion_resets_found": 0,
    "ky_cuoc_method_has_direct_vip_guard": False,
    "boss_completion_branch_checks_ky_cuoc_flag": False,
    "boss_completion_ignores_goto_result": True,
    "entry_ignores_trieu_tap_result": True,
    "countdown_wait_seconds": 30, "countdown_subtraction_constant": 20,
    "aoe_empty_list_guard_is_tautology": True}
with zipfile.ZipFile(repo / "ChickenAutoEx-new.zip") as archive:
    digest = hashlib.sha256(archive.read("ChickenAutoEx.exe")).hexdigest()
assert digest == PAYLOAD
result["original_executable_sha256"] = digest
result["original_binary_files"] = {path.name: hashlib.sha256(path.read_bytes()).hexdigest()
                                   for path in sorted(repo.glob("ChickenAuto*")) if path.is_file()}
output = repo / "analysis/tran-long-ky-cuoc/evidence"
output.mkdir(parents=True, exist_ok=True)
if args.il:
    # This is the file used by the documented ILSpy invocation, not merely the archive entry.
    assert hashlib.sha256((repo / ".build/ChickenAutoEx-original.exe").read_bytes()).hexdigest() == PAYLOAD
    il = args.il.read_text(encoding="utf-8-sig")
    def il_method(name):
        end_marker = "\t} // end of method Game::" + name + "\n"
        end = il.index(end_marker)
        start = il.rfind("\t.method", 0, end)
        body = il[start:end + len(end_marker)]
        assert " " + name + " () cil managed" in body, name
        return body
    leader = il_method("DatDoiKyCuoc")
    aoe_il = il_method("AOE")
    auto = il_method("Auto")
    assert "IL_0062: call instance bool TinhKiemAuto.Game::TrieuTap()\n\t\tIL_0067: pop" in leader
    assert il.count("call instance void TinhKiemAuto.Game::set_IsXongKyCuoc(bool)") == 1
    first = auto.index("\t\tIL_076b:")
    last = auto.index("\t\tIL_0801:", first)
    ending = auto[first:last]
    assert "IL_07b2: pop" in ending and "IL_07bc: call instance void TinhKiemAuto.Game::set_IsXongKyCuoc(bool)" in ending
    assert "IL_07d2: ldc.i4.s 20" in ending and "IL_079c: ldc.r8 30" in ending
    for filename, body in [("DatDoiKyCuoc.il.txt", leader), ("Auto-kycuoc-completion.il.txt", ending),
                           ("AOE-empty-list.il.txt", aoe_il)]:
        # Normalize ILSpy's trailing whitespace only; preserve every instruction.
        normalized = "\n".join(line.rstrip() for line in body.splitlines()).rstrip() + "\n"
        (output / filename).write_text(normalized, encoding="utf-8")
    result["original_il_cross_check"] = {
        "tool": "ILSpyCmd (static disassembly; target not executed)",
        "input_executable_sha256": PAYLOAD,
        "input_il_sha256": hashlib.sha256(args.il.read_bytes()).hexdigest(),
        "completion_setter_call_count_in_original_il": 1,
        "exported_evidence": ["DatDoiKyCuoc.il.txt", "Auto-kycuoc-completion.il.txt", "AOE-empty-list.il.txt"]}
else:
    # Keep independently collected IL provenance when refreshing source-only inventory.
    previous = output / "source-inventory.json"
    if previous.exists():
        data = json.loads(previous.read_text(encoding="utf-8"))
        if data.get("original_executable_sha256") == PAYLOAD:
            result["original_il_cross_check"] = data.get("original_il_cross_check", {})
(output / "source-inventory.json").write_text(
    json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print("PASS: static source patterns, protected-source reversal and original binary hash; evidence saved.")
