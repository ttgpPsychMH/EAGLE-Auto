"""Inventory Ac Ba source, routes and selected original IL as data, without target execution."""
from pathlib import Path
import configparser
import hashlib
import json
import re
import subprocess
import sys

repo = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(repo / "tools"))
from thuy_lao_review import apply_review as thuy
from ky_cuoc_review import apply_review as ky
from trung_ac_review import apply_review as trung
from ac_tac_review import apply_review as ac_tac

base = repo / "analysis/chickenautoex-107/recovered/TinhKiemAuto"
original = (base / "Game.cs").read_text()
reviewed = ac_tac(repo, trung(repo, ky(repo, thuy(repo, original))))
form = (repo / "src/ChickenAutoEx/FrmMain.cs").read_text()


def method(text, name):
    match = re.search(r'^\t\t(?:private|public|protected)(?: override)? [^\n]+ ' + name +
                      r'\([^\n]*\)\n\t\t\{\n.*?^\t\t\}\n', text, re.M | re.S)
    assert match, name
    return {"line": text[:match.start()].count('\n') + 1,
            "sha256_lf": hashlib.sha256(match.group().encode()).hexdigest()}


methods = {"Game." + n: method(reviewed, n) for n in
           ["DatDoiAcBa", "TalkNPCPhuBan", "TrieuTap", "Auto", "ClearMission", "SetCalendar", "TheoDoiCanhBao", "MoveNext", "Next", "AcTac", "FixKetMap", "AppointLeader"]}
for name in ["MoveNext", "Next"]:
    match = re.search(r'^\t\tpublic void ' + name + r'\(int\[,] point\)\n\t\t\{\n.*?^\t\t\}\n', reviewed, re.M | re.S)
    assert match
    methods["Game." + name + "(int[,])"] = {"line": reviewed[:match.start()].count('\n') + 1,
                                           "sha256_lf": hashlib.sha256(match.group().encode()).hexdigest()}
methods.update({"FrmMain." + n: method(form, n) for n in ["WndProc", "ItemAcBa_Click", "ResetDungeonActions", "UpdateDungeonMenu"]})
assert method(original, "DatDoiAcBa")["sha256_lf"] == methods["Game.DatDoiAcBa"]["sha256_lf"]
assert method(original, "TalkNPCPhuBan")["sha256_lf"] == methods["Game.TalkNPCPhuBan"]["sha256_lf"]

resource = repo / ".build/chickenautoex/resources/TinhKiemAuto.MapPath.dat"
resource_hash = hashlib.sha256(resource.read_bytes()).hexdigest()
assert resource_hash == "361c681f110be51cfb9c71f84526cf3c0153584e84a43e7461faf9fd6bf26250"
ini = configparser.ConfigParser(interpolation=None, strict=False)
ini.read_string(resource.read_text(encoding="utf-8-sig"))
menpai = (base / "MENPAI.cs").read_text()
maps = (base / "MAP.cs").read_text()
routes = []
for name, school_class in [("ThieuLam", "THIEULAM"), ("MinhGiao", "MINHGIAO"), ("CaiBang", "CAIBANG"),
                           ("VoDang", "VODANG"), ("NgaMy", "NGAMY"), ("TinhTuc", "TINHTUC"),
                           ("ThienLong", "THIENLONG"), ("ThienSon", "THIENSON"), ("TieuDao", "TIEUDAO"),
                           ("MoDung", "MODUNG"), ("DuongMon", "DUONGMON")]:
    school = int(re.search(r'int ' + name + r' = (\d+)', menpai).group(1))
    outside = int(re.search(r'int Id = (\d+)', (base / (school_class + ".cs")).read_text()).group(1))
    inside = int(re.search(r'int ' + name + r'AcBa = (\d+)', maps).group(1))
    points = [list(map(int, p.split(','))) for p in ini.get("MAP", str(inside)).split('-') if re.fullmatch(r'\d+,\d+', p)]
    assert len(points) > 1
    routes.append({"name": name, "school_id": school, "outdoor_map": outside, "dungeon_map": inside,
                   "embedded_point_count": len(points), "first_embedded_point": points[0]})

inputs = ["src/ChickenAutoEx/FrmMain.cs", "src/ChickenAutoEx/Global.cs", "EagleAuto.Version.props"]
inputs += [str(p.relative_to(repo)) for p in [base / (n + ".cs") for n in
           ["Game", "TLBB", "MAP", "MENPAI", "POINT", "Setting", "TINHKIEM", "ConverterEx", "GameObjects", "GameObject", "QuestFrame"]]]
inputs += ["tools/" + n + "_review.json" for n in ["thuy_lao", "ky_cuoc", "trung_ac", "ac_tac", "dungeon_reaudit"]]
inputs += [str(p.relative_to(repo)) for p in (repo / "src/ChickenAutoEx").glob("*/*.cs") if p.parent.name in ["ThuyLao", "KyCuoc", "TrungAc", "AcTac"]]
for p in inputs:
    assert (repo / p).read_bytes() == subprocess.check_output(["git", "show", "910020a:" + p], cwd=repo), "Changed analysis baseline: " + p
result = {"baseline_commit": "910020a", "application_version": "0.5", "scope": "Analysis only; application source/binaries unchanged",
          "game_source": "Original redacted Game.cs plus exact v0.2-v0.5 review layers; no hydrated secrets",
          "methods": methods, "input_sha256": {p: hashlib.sha256((repo / p).read_bytes()).hexdigest() for p in sorted(inputs)},
          "embedded_route_sha256": resource_hash, "schools": routes,
          "user_private_client_observation": "System event announcement includes NPC at school and Ac Ba appeared at that school; exact text/encoding not yet available"}
out = repo / "analysis/ac-ba/evidence"
out.mkdir(parents=True, exist_ok=True)
il_path = repo / ".build/trung-ac-original.il"
if il_path.exists():
    result["original_il_sha256"] = hashlib.sha256(il_path.read_bytes()).hexdigest()
    assert result["original_il_sha256"] == "0a4fa65c52e48053aec0590ff1229c3fb794f848d373f51b411f1c2a9b569c35"
    text = il_path.read_text()
    for cls, name in [("Game", "DatDoiAcBa"), ("TLBB", "get_MapAcBa")]:
        end = text.index("} // end of method " + cls + "::" + name) + len("} // end of method " + cls + "::" + name)
        start = text.rfind('\t.method ', 0, end)
        (out / (name + ".il.txt")).write_text('\n'.join(line.rstrip() for line in text[start:end].splitlines()) + '\n')
    end = text.index("} // end of method FrmMain::WndProc")
    start = text.rfind('\t.method ', 0, end)
    body = text[start:end]
    # Publish only the event recognition block; exclude unrelated hotkeys/chat contents.
    a = body.index('ldstr "duongmon"')
    a = body.rfind('\n', 0, a) + 1
    b = body.index('\t\tIL_0498:')
    (out / "WndProc.ac-ba.il.txt").write_text('// Selected original FrmMain::WndProc IL excerpt; recognition branches only.\n' +
        '\n'.join(line.rstrip() for line in body[a:b].splitlines()).rstrip() + '\n')
(out / "source-inventory.json").write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n')
print("PASS: selected Ac Ba source/IL inventory and all 11 embedded routes verified as data.")
