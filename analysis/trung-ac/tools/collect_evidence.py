"""Collect Trừng Ác static evidence. Never load/run target assemblies or native DLLs.

Run from the repository root; --il accepts ignored output from the documented
trusted ILSpyCmd invocation. Export only the named method and selected snippets.
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
from thuy_lao_review import apply_review as thuy_lao
from ky_cuoc_review import apply_review as ky_cuoc

BASELINE = "91a93f2"
PAYLOAD = "11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573"
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--il", type=Path)
args = parser.parse_args()
base = repo / "analysis/chickenautoex-107/recovered"
game_path = base / "TinhKiemAuto/Game.cs"
game = game_path.read_text(encoding="utf-8")
current = ky_cuoc(repo, thuy_lao(repo, game))
assert thuy_lao(repo, ky_cuoc(repo, current, reverse=True), reverse=True) == game
form = (repo / "src/ChickenAutoEx/FrmMain.cs").read_text(encoding="utf-8")


def method(source, name, parameters=r"[^\n]*"):
    matches = list(re.finditer(
        r"^\t\t(?:private|public)(?: static)? [^\n]+ " + re.escape(name)
        + r"\(" + parameters + r"\)\n\t\t\{\n.*?^\t\t\}\n", source, re.M | re.S))
    assert len(matches) == 1, name
    return matches[0]


engine = method(current, "TrungAc", "").group()
alarm = method(current, "TheoDoiCanhBao", "").group()
timer = alarm[alarm.index("\t\t\tif (comeTime != null"):alarm.index("\t\t\tif (TLBB.Disconnected)")]
menu = method(form, "ItemTrungAc_Click").group()
clear = method(current, "ClearNhiemVu", "").group()
calendar = method(current, "SetCalendar", "").group()
reset = method(current, "ClearMission", "").group()
set_info = method(form, "SetInfo", "").group()
checks = {
    "engine_unchanged_since_original": engine == method(game, "TrungAc", "").group(),
    "alarm_method_unchanged_since_original": alarm == method(game, "TheoDoiCanhBao", "").group(),
    "parser_substring_precedes_map_validation": engine.index("TrungAcInfo.Substring(num, length)") < engine.index("MissionMap != -1"),
    "parser_accepts_any_nonzero_split_length_then_reads_index_1": "text.Split(',').Length != 0" in engine and "text.Split(',')[1]" in engine,
    "timer_has_no_module_auto_online_pause_guard": not any(s in timer for s in ["IsTrungAc", "IsAuto", "TLBB.Online", "Global.Paused"]),
    "ui_set_info_calls_alarm_without_auto_guard": "game.TheoDoiCanhBao();" in set_info and "if (!game.IsAuto)" not in set_info,
    "menu_only_toggles_leader_task_without_session_reset": "leader.IsTrungAc = !leader.IsTrungAc;" in menu and not any(s in menu for s in ["State", "IsXongTrungAc", "comeTime", "ResetDungeonActions"]),
    "clear_nhiem_vu_sets_flag_false_before_testing_it": clear.index("IsTrungAc =") < clear.index("if (IsTrungAc)"),
    "clear_mission_does_not_cancel_trung_ac": "IsTrungAc" not in reset,
    "current_is_busy_omits_trung_ac": "private bool IsBusy => IsMapPhuBan() || IsThuyLao || IsKyCuoc;" in current,
    "calendar_clears_missions_before_selecting_another": "ClearMission();" in calendar,
    "engine_falls_through_to_state_null": engine.rstrip().endswith("State = STATE.Null;\n\t\t}"),
    "engine_has_no_party_dispatch": not any(s in engine for s in ["Party", "TrieuTap", "AskTeamFollow"]),
    "no_managed_assignment_to_talk_to_xa_phu": re.search(r"State\s*=\s*STATE\.TalkToXaPhu", current) is None,
    "token_use_in_come_has_no_state_change": "PlayerPackageUseItem(item8.Index);\n\t\t\t\t\t\treturn;" in engine,
    "normal_combat_loops_without_break_after_live_target": re.search(r"SendKey\(Global.BaseSkill\);\s*doneTime = null;\s*\}\s*else", engine) is not None,
    "normal_combat_marks_done_on_dead_candidate_in_city": "if (TLBB.MapId <= 2 || doneTime.Elapsed.TotalSeconds > 10.0)" in engine,
    "completion_is_read_from_two_dialog_snapshots": "QuestFrame.All(this).Contains(\"#{CXDY_090423_01}\") && QuestFrame.All(this).Contains(\"#{CXDY_090423_02}\")" in engine,
    "engine_does_not_call_local_quest_completion_write": "SetComplete" not in engine and "SetTrangThai" not in engine,
}
assert all(checks.values()), [name for name, passed in checks.items() if not passed]

result = {"baseline_application_commit": BASELINE, "display_version": "v0.3",
          "date": "2026-10-10", "timezone": "Asia/Bangkok",
          "scope": "Static source/IL review; no new engine simulation, target execution or game test",
          "application_executed": False, "native_dll_executed": False,
          "automation_changed": False, "product_version_changed": False,
          "static_pattern_checks": checks, "files": {}, "state_references": {}, "core_methods": {}}
paths = [repo / path for path in ["src/ChickenAutoEx/FrmMain.cs", "src/ChickenAutoEx/Global.cs",
    "src/ChickenAutoEx/KyCuoc/Game.KyCuoc.cs", "src/ChickenAutoEx/KyCuoc/Game.PetAoe.cs",
    "EagleAuto.Version.props", "tools/thuy_lao_review.json", "tools/ky_cuoc_review.json", "tools/dungeon_menu_review.json"]]
paths += [base / path for path in ["TinhKiemAuto/Game.cs", "TinhKiemAuto/STATE.cs",
    "TinhKiemAuto/Task.cs", "TinhKiemAuto/TaskInfo.cs", "TinhKiemAuto/QuestFrame.cs",
    "TinhKiemAuto/TINHKIEM.cs", "TinhKiemAuto/GameObjects.cs", "TinhKiemAuto/GameObject.cs",
    "TinhKiemAuto/PacketItem.cs", "TinhKiemAuto/Account.cs", "TinhKiemAuto/TLBB.cs",
    "TinhKiemAuto/MAP.cs", "TinhKiemAuto/Address.cs", "TinhKiemAuto/Memory.cs",
    "TinhKiemAuto/LUA.cs", "FindPath.cs", "TinhKiemAuto.Models/Unity.cs",
    "TinhKiemAuto/Scripts.cs", "TinhKiemAuto.Scripts.xml", "TinhKiemAuto.PathList.txt"]]
symbols = ["IsTrungAc", "IsXongTrungAc", "IsBTDByLogin", "TrungAcInfo", "MissionMap", "MissionX", "MissionY",
           "comeTime", "doneTime", "HongTrungAcTime", "IsHong", "TalkToXaPhu", "CXDT_090304_01", "CXDY_090423_01", "CXDY_090423_02"]
for path in paths:
    relative = str(path.relative_to(repo))
    source = path.read_text(encoding="utf-8")
    result["files"][relative] = {"sha256_bytes": hashlib.sha256(path.read_bytes()).hexdigest()}
    for symbol in symbols:
        lines = [i for i, line in enumerate(source.splitlines(), 1) if re.search(r"\b" + re.escape(symbol) + r"\b", line)]
        if lines: result["state_references"].setdefault(symbol, {})[relative] = lines
for name, parameters in [("TrungAc", ""), ("TheoDoiCanhBao", ""), ("ClearNhiemVu", ""),
    ("ClearMission", ""), ("Auto", ""), ("SetCalendar", ""), ("TrongPhamVi", "int x, int y, int map"),
    ("DaDenNoi", "int x, int y, int map"), ("TimDuong", r"float InputX, float InputY, int MapID"),
    ("ForcePickItem", ""), ("SkillDo", ""), ("Attack", ""), ("LuaToString", ""), ("LuaString", "")]:
    old = method(game, name, parameters)
    reviewed = method(current, name, parameters)
    result["core_methods"][name] = {"snapshot_source_line": game[:old.start()].count("\n") + 1,
        "original_sha256_lf": hashlib.sha256(old.group().encode()).hexdigest(),
        "current_v03_sha256_lf": hashlib.sha256(reviewed.group().encode()).hexdigest(),
        "unchanged_by_existing_reviews": old.group() == reviewed.group()}
with zipfile.ZipFile(repo / "ChickenAutoEx-new.zip") as archive:
    assert hashlib.sha256(archive.read("ChickenAutoEx.exe")).hexdigest() == PAYLOAD
result["original_executable_sha256"] = PAYLOAD
result["original_binary_files"] = {p.name: hashlib.sha256(p.read_bytes()).hexdigest()
                                   for p in sorted(repo.glob("ChickenAuto*")) if p.is_file() and p.suffix in {".exe", ".zip", ".7z"}}
output = repo / "analysis/trung-ac/evidence"
output.mkdir(parents=True, exist_ok=True)
if args.il:
    assert hashlib.sha256((repo / ".build/ChickenAutoEx-original.exe").read_bytes()).hexdigest() == PAYLOAD
    il = args.il.read_text(encoding="utf-8-sig")

    def il_method(name):
        end_marker = "\t} // end of method Game::" + name + "\n"
        end = il.index(end_marker)
        start = il.rfind("\t.method", 0, end)
        body = il[start:end + len(end_marker)]
        assert " " + name + " () cil managed" in body
        return body

    engine_il = il_method("TrungAc")
    alarm_il = il_method("TheoDoiCanhBao")
    timer_il = alarm_il[alarm_il.index("\t\tIL_0318:"):alarm_il.index("\t\tIL_03c1:")]
    assert "System.String::Substring(int32, int32)" in engine_il
    assert "\t\tIL_0523: ldlen\n\t\tIL_0524: brfalse.s" in engine_il
    assert "Mission_Abnegate_Popup" in timer_il and "ldc.r8 150" in timer_il and "ldc.r8 300" in timer_il
    assert "Game::get_IsAuto" not in timer_il and "Game::IsTrungAc" not in timer_il
    exports = {"TrungAc.il.txt": engine_il, "Alarm-trung-ac-timer.il.txt": timer_il,
               "ClearNhiemVu.il.txt": il_method("ClearNhiemVu")}
    for filename, body in exports.items():
        normalized = "\n".join(line.rstrip() for line in body.splitlines()).rstrip() + "\n"
        (output / filename).write_text(normalized, encoding="utf-8")
    result["original_il_cross_check"] = {"tool": "ILSpyCmd 9.1.0.7988, static target read",
        "input_executable_sha256": PAYLOAD, "input_il_sha256": hashlib.sha256(args.il.read_bytes()).hexdigest(),
        "exported_evidence": list(exports),
        "provenance_limit": "Collector verifies payload and expected IL patterns; arbitrary supplied IL is not independently attested."}
else:
    previous = output / "source-inventory.json"
    if previous.exists():
        old_result = json.loads(previous.read_text(encoding="utf-8"))
        if old_result.get("original_executable_sha256") == PAYLOAD:
            result["original_il_cross_check"] = old_result.get("original_il_cross_check", {})
(output / "source-inventory.json").write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(f"PASS: {len(checks)} static patterns, exact reviewed-source reversal, original binary hash; selected evidence exported.")
