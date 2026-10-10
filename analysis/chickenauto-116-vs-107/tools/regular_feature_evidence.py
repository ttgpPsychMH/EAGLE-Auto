"""Verify reviewed feature evidence without running game applications or hooks.

Export named C# method locations/hashes, and references to reviewed Qt evidence.
These checks establish source/UI evidence, never runtime feature parity.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess


BASE = "9c59d1774ac1aa18dd9e080684f732577fece999"
GAME = "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs"
FORM = "src/ChickenAutoEx/FrmMain.cs"
TLBB = "analysis/chickenautoex-107/recovered/TinhKiemAuto/TLBB.cs"

# IDs correspond to REGULAR-FEATURES.vi.md, not a claim that these features pass.
FEATURES = [
    ("F01", "Nhận diện/trạng thái", ["form.SetInfo", "tlbb.Read"], [], ["WorkerDetectGameInstance", "WorkerUpdateUi"]),
    ("F02", "Hồi HP", ["game.Buff", "form.checkregenhp_CheckedChanged"], ["autoRecoverHp", "autoRecoverHpPercent"], []),
    ("F03", "Hồi MP", ["game.Buff", "form.checkrengenmp_CheckedChanged"], ["autoRecoverMp", "autoRecoverMpPercent"], []),
    ("F04", "Hồi HP/Hoan Hỉ pet", ["form.Auto", "game.Auto", "game.BuffPet"], ["autoRecoverPetHp", "autoRecoverPetHpPercent"], []),
    ("F05", "Cộng Sinh/Huyết Tế", ["game.Buff"], ["autoUsePetRecoverHpSkill", "autoUsePetRecoverMpSkill"], []),
    ("F06", "Xuất/thu pet", ["game.XuatPet", "game.Auto"], ["enableAutoSummonPet", "autoSummonPetId"], []),
    ("F07", "Đánh cơ bản/F1", ["game.Attack"], ["enableAutoAttack", "enableUseF1"], []),
    ("F08", "Danh sách skill", ["game.SkillDo", "game.DoSkill", "game.SaveSkill"], ["enableAutoUseSkill", "autoSkillList"], []),
    ("F09", "Đánh quanh/gom/lọc quái", ["game.GetBestTarget", "game.Attack"], ["enableAttackRadius", "ignoreMonsterList", "enableAutoKs"], []),
    ("F10", "Theo key", ["game.FollowKey"], ["enableAttackFollowLeader"], []),
    ("F11", "Buff Nga My", ["game.Auto", "form.checkisNM_CheckedChanged"], ["enableAutoLotusBuffHp", "autoLotusBuffPlayerList"], []),
    ("F12", "Nhặt đồ", ["game.PickItem", "game.ForcePickItem"], ["enableAutoPickupLootPackage"], []),
    ("F13", "Vứt rác/bảo vệ đồ", ["game.DropItem", "game.IsDropEx", "game.IsDrop", "game.IsCanDelete"], ["enableAutoThrowTrashItem", "autoThrowTrashItemList"], []),
    ("F14", "Sau tử vong/cảnh báo", ["game.Auto"], ["enableAutoActionWhenDead", "autoLogoutGameAfterDeadSeconds", "enableAutoAlertHp"], []),
    ("F15", "Đồng ý tổ đội/chuyển cảnh", ["game.Accept", "game.AcceptAll", "game.Auto"], ["enableAutoAcceptTeamRequest", "enableAutoAcceptSceneTransfer"], []),
    ("F16", "Cấu hình", ["form.LoadSetting", "form.SaveSetting", "game.LoadSetting", "game.SaveSetting"], ["AutoSettings.json"], []),
    ("F17", "Phím tắt/tray/nhiều game", ["form.SetHotKey", "form.SetInfo", "form.ListViewNhanVat_ItemChecked"], ["AutoSettings::enableShortcutKeys", "AutoSettings::enableMinimizeToSystemTray"], ["game_controllers::ShortcutKeysManager"]),
    ("F18", "Tiện ích level/exp", ["game.UpLvl", "game.AutoX2"], ["enableAutoLevelup", "autoLevelupTo", "enableAutoUseX2dot5ExpItem"], []),
    ("F19", "Ngôn ngữ", ["form.InitializeComponent"], [], ["others_ui::SettingAutoDialog"]),
    ("F20", "Hiệu năng/khả năng chẩn đoán", ["form.Auto", "game.Auto"], [], ["WorkerDetectGameInstance", "WorkerUpdateUi"]),
]
DUNGEON_METHODS = ["TrungAc", "DatDoiAcBa", "DatDoiAcTac", "DatDoiKyCuoc",
                   "DatDoiLauLanTamBao", "DatDoiThuyLao", "TKC"]


def digest(data):
    return hashlib.sha256(data).hexdigest()


def method(text, name):
    pattern = (r'^\t\t(?:public|private|internal|protected)(?: static)? [^\n]+ '
               + re.escape(name) + r'\([^\n]*\)\n\t\t\{\n.*?^\t\t\}\n')
    matches = list(re.finditer(pattern, text, re.M | re.S))
    if not matches:
        raise ValueError("Missing C# method: " + name)
    return matches


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[3])
    parser.add_argument("--stable-evidence", type=Path, required=True,
                        help="Fresh compare_static.py evidence directory")
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    repo = args.repo.resolve()
    evidence = repo / "analysis/chickenauto-116-vs-107/evidence"
    checked = ["inventory.json", "qt-metaobjects-116.json", "ui-translations-116.json",
               "selected-literals-116.json", "ilspy-116.txt", "quest-metacall-116.txt"]
    for name in checked:
        if (evidence / name).read_bytes() != (args.stable_evidence / name).read_bytes():
            raise ValueError("Fresh native evidence differs: " + name)
    inventory = json.loads((evidence / "inventory.json").read_text())
    literals = {x["literal"]: x["file_offsets"] for x in
                json.loads((evidence / "selected-literals-116.json").read_text())}
    classes = {x["class"]: x for x in json.loads((evidence / "qt-metaobjects-116.json").read_text())}
    translations = json.loads((evidence / "ui-translations-116.json").read_text())
    sources = {"form": (FORM, (repo / FORM).read_text()),
               "game": (GAME, (repo / GAME).read_text()),
               "tlbb": (TLBB, (repo / TLBB).read_text())}
    anchors = {}
    ids = {x for _, _, names, _, _ in FEATURES for x in names}
    ids.update("game." + name for name in DUNGEON_METHODS)
    ids.update(["form.TryGetDungeonContext", "form.UpdateDungeonMenu"])
    for key in sorted(ids):
        group, name = key.split(".", 1)
        path, text = sources[group]
        anchors[key] = [{"path": path, "line": text.count("\n", 0, match.start()) + 1,
                         "method_text_sha256_lf": digest(match.group().encode())}
                        for match in method(text, name)]
    features = []
    for id_, label, names, config, qt in FEATURES:
        assert all(literals.get(name) for name in config), id_
        assert all(name in classes for name in qt), id_
        features.append({"id": id_, "name": label, "source_107": names,
                         "ui_evidence_116": {"config_literals": {name: literals[name] for name in config},
                                             "qt_classes": qt},
                         "runtime_comparison": "NOT_RUN"})
    form, game = sources["form"][1], sources["game"][1]
    form_auto = method(form, "Auto")[0].group()
    game_auto = method(game, "Auto")[0].group()
    pet = method(game, "BuffPet")[0].group()
    nm = method(form, "checkisNM_CheckedChanged")[0].group()
    assert "value.Auto();" in form_auto and "value.BuffPet();" in form_auto
    assert re.search(r'if \(!IsAuto\)\s*\{\s*return;', game_auto)
    assert "IsAuto" not in pet and "Global.Paused" not in pet
    assert "TLBB.PetHPPercent <= 50" in pet and "TLBB.PetHPPercent <= 85" in pet
    assert "BuffPetPercent" not in pet
    assert "CurGame.IsNM = checkisNM.Checked;" in nm and "(checkhuyette.Checked ?" in nm
    assert "CurGame.IsMP = checkrengenmp.Checked;" in method(form, "checkrengenmp_CheckedChanged")[0].group()
    assert game.count("BuffPetPercent") == 3  # declaration, load, save; not a consumer
    language = [m for m in translations if m.get("context") == "SettingAutoDialog" and m.get("source") == "Language"]
    assert len(language) == 1
    untouched = ["src", "tests", "tools", ".github", "global.json", "ChickenAutoEx.sln",
                 "analysis/chickenautoex-107/recovered", "ChickenAuto-new.exe", "ChickenAuto-new.zip",
                 "ChickenAuto-new.7z", "ChickenAutoEx-new.exe", "ChickenAutoEx-new.zip", "ChickenAutoEx-new.7z"]
    assert not subprocess.check_output(["git", "diff", BASE, "--", *untouched], cwd=repo)
    result = {
        "application_baseline_commit": BASE,
        "stable_payload_sha256": inventory["stable_116"]["sha256"],
        "stable_native_x86": inventory["stable_116"]["machine"] == "0x14c" and not inventory["stable_116"]["managed"],
        "source_file_sha256": {p: digest((repo / p).read_bytes()) for p, _ in sources.values()},
        "fresh_native_evidence_matches_historical": True,
        "native_evidence_sha256": {name: digest((evidence / name).read_bytes()) for name in checked},
        "anchors": anchors, "features": features,
        "static_findings": {
            "pet_called_outside_game_auto_gate": True,
            "buff_pet_has_no_auto_or_paused_gate": True,
            "pet_thresholds_hardcoded_not_buff_pet_percent": True,
            "nm_notice_uses_blood_sacrifice_checkbox": True,
            "mp_binding_fix_present": True,
            "native_language_ui": language,
        },
        "application_and_original_paths_unchanged": untouched,
        "execution": {"target_executables_or_dlls_run": False, "game_run": False,
                      "new_build_required": False, "windows_runtime_comparison_run": False},
        "limits": "Method hashes and Qt labels/config/slots establish evidence availability; native algorithms, runtime parity and bug impact remain unverified.",
    }
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n")
    print("PASS: 20 feature evidence groups, reviewed C# anchors, four static findings; fresh native evidence identical.")
    print("PASS: application, engine, licensing, stable release and existing tests/CI unchanged; no target executed.")


if __name__ == "__main__":
    main()
