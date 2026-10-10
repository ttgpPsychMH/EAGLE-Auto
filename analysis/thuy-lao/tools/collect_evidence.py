"""Inventory Thuy Lao source references and hashes; never load/run the game application."""
import hashlib
import json
from pathlib import Path
import re
import zipfile

repo = Path(__file__).resolve().parents[3]
baseline = repo / "analysis/chickenautoex-107/recovered"
files = [repo / "src/ChickenAutoEx/FrmMain.cs"] + [baseline / path for path in [
    "TinhKiemAuto/Game.cs", "TinhKiemAuto/Task.cs", "TinhKiemAuto/TaskInfo.cs",
    "TinhKiemAuto/TLBB.cs", "TinhKiemAuto/Address.cs", "TinhKiemAuto/Memory.cs",
    "TinhKiemAuto/LUA.cs", "TinhKiemAuto/GameObjects.cs", "TinhKiemAuto/MAP.cs",
    "TinhKiemAuto/THAIHO.cs", "TinhKiemAuto/TOCHAU.cs", "TinhKiemAuto/Scripts.cs",
    "TinhKiemAuto.Scripts.xml", "TinhKiemAuto.PathList.txt", "TinhKiemAuto.Screen.txt"]]
symbols = ["IsThuyLao", "TrangThaiThuyLao", "DaNhanThuyLao", "IsXongThuyLao"]
result = {"baseline_commit": "c1b126b0b7ed3ddf5cc1e3930c177559c779d666",
          "scope": "Static source/data inventory, not engine tests or client compatibility validation",
          "application_executed": False, "files": {}, "state_references": {}}
for path in files:
    relative = str(path.relative_to(repo))
    source = path.read_text(encoding="utf-8")
    result["files"][relative] = {"sha256_bytes": hashlib.sha256(path.read_bytes()).hexdigest()}
    for symbol in symbols:
        lines = [i for i, line in enumerate(source.splitlines(), 1) if re.search(r"\b" + symbol + r"\b", line)]
        if lines:
            result["state_references"].setdefault(symbol, {})[relative] = lines
game = (baseline / "TinhKiemAuto/Game.cs").read_text(encoding="utf-8")
result["core_methods"] = {}
for name in ["DiThuyLao", "NhanThuyLao", "DatDoiThuyLao", "TrieuTap", "ClearMission", "SetCalendar"]:
    matches = list(re.finditer(r"^\t\t(?:private|public) [^\n]+ " + name + r"\([^\n]*\)\n\t\t\{\n.*?^\t\t\}\n", game, re.M | re.S))
    assert len(matches) == 1, name
    match = matches[0]
    result["core_methods"][name] = {"source_line": game[:match.start()].count("\n") + 1,
                                  "sha256_lf": hashlib.sha256(match.group().encode()).hexdigest()}
assert 'TimDuong(THAIHO.HoDienKhanh.X, THAIHO.HoDienKhanh.Y, MAP.ToChau);' in game
result["original_il_cross_check"] = {
    "origin": "Recorded manual IL inspection during this review; this collector does not run ILSpy",
    "tool": "ILSpyCmd", "input_archive": "ChickenAutoEx-new.zip", "entry": "ChickenAutoEx.exe",
    "methods_inspected": ["DiThuyLao", "NhanThuyLao", "DatDoiThuyLao"],
    "DiThuyLao_route": {"map_field_instruction": "IL_004d: ldsfld int32 TinhKiemAuto.MAP::ToChau",
                       "call_instruction": "IL_0052: call instance void TinhKiemAuto.Game::TimDuong(float32, float32, int32)"}}
with zipfile.ZipFile(repo / "ChickenAutoEx-new.zip") as archive:
    result["original_il_cross_check"]["input_executable_sha256"] = hashlib.sha256(archive.read("ChickenAutoEx.exe")).hexdigest()
output = repo / "analysis/thuy-lao/evidence/source-inventory.json"
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print("Collected source hashes, all four state references, core method hashes and recorded IL cross-check.")
