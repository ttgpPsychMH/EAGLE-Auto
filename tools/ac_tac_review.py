"""Apply/reverse exact Ác Tặc deltas after v0.4 reviews."""
import hashlib
import json
from pathlib import Path


def apply_review(repo, text, reverse=False, form=False):
    review = json.loads((repo / "tools/ac_tac_review.json").read_text())
    edits = review["form_edits" if form else "edits"]
    for edit in reversed(edits) if reverse else edits:
        before, after = (edit["reviewed"], edit["original"]) if reverse else (edit["original"], edit["reviewed"])
        if reverse and not before:
            before, after = edit["anchor"], after + edit["anchor"]
        assert text.count(before) == edit["count"], edit["name"]
        text = text.replace(before, after, edit["count"])
    for path, expected in review["additional_source_hashes_lf"].items():
        assert hashlib.sha256((repo / path).read_text(encoding="utf-8").encode()).hexdigest() == expected, path
    return text


def write_regression_source(repo):
    from thuy_lao_review import apply_review as thuy
    from ky_cuoc_review import apply_review as ky
    from trung_ac_review import apply_review as trung
    original = (repo / "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs").read_text()
    legacy = trung(repo, ky(repo, thuy(repo, original)))
    path = repo / ".build/ac-tac"
    path.mkdir(parents=True, exist_ok=True)
    (path / "LegacyGame.cs").write_text(legacy, encoding="utf-8")
    (path / "ReviewedGame.cs").write_text(apply_review(repo, legacy), encoding="utf-8")
    from ac_ba_review import apply_review as ac_ba
    (path / "LegacyFrmMain.cs").write_text(apply_review(repo,
        ac_ba(repo, (repo / "src/ChickenAutoEx/FrmMain.cs").read_text(), reverse=True, form=True), reverse=True, form=True), encoding="utf-8")


if __name__ == "__main__":
    write_regression_source(Path(__file__).resolve().parents[1])
