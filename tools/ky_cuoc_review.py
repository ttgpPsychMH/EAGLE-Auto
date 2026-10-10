"""Apply/reverse only reviewed Ky Cuoc and shared pet AOE deltas, after Thuy Lao."""
import hashlib
import json
from pathlib import Path


def apply_review(repo, text, reverse=False):
    review = json.loads((repo / "tools/ky_cuoc_review.json").read_text())
    edits = list(reversed(review["edits"])) if reverse else review["edits"]
    for edit in edits:
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
    text = thuy_lao(repo, (repo / "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs").read_text())
    path = repo / ".build/ky-cuoc/ReviewedGame.cs"
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(apply_review(repo, text), encoding="utf-8")


if __name__ == "__main__":
    write_regression_source(Path(__file__).resolve().parents[1])
