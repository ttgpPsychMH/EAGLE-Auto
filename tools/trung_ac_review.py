"""Apply/reverse exact Trừng Ác deltas after Thủy Lao and Kỳ Cuộc reviews."""
import hashlib
import json
from pathlib import Path


def apply_review(repo, text, reverse=False, form=False):
    review = json.loads((repo / "tools/trung_ac_review.json").read_text())
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
    from thuy_lao_review import apply_review as thuy_lao
    from ky_cuoc_review import apply_review as ky_cuoc
    original = (repo / "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs").read_text()
    path = repo / ".build/trung-ac/ReviewedGame.cs"
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(apply_review(repo, ky_cuoc(repo, thuy_lao(repo, original))), encoding="utf-8")


if __name__ == "__main__":
    write_regression_source(Path(__file__).resolve().parents[1])
