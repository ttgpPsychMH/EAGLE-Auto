"""Apply/reverse exact reviewed Thuy Lao deltas; all other recovered Game code is protected."""
import hashlib
import json
from dungeon_reaudit_review import baseline_partial
from pathlib import Path


def apply_review(repo, text, reverse=False):
    review = json.loads((repo / "tools/thuy_lao_review.json").read_text())
    edits = list(reversed(review["edits"])) if reverse else review["edits"]
    for edit in edits:
        before, after = (edit["reviewed"], edit["original"]) if reverse else (edit["original"], edit["reviewed"])
        if reverse and not before:
            # Removed methods retain unique adjacent markers to make reversal exact.
            before, after = edit["anchor"], after + edit["anchor"]
        assert text.count(before) == edit["count"], edit["name"]
        text = text.replace(before, after, edit["count"])
    for path, expected in review["additional_source_hashes_lf"].items():
        assert hashlib.sha256(baseline_partial(repo, path).encode()).hexdigest() == expected, path
    return text


def write_regression_source(repo):
    baseline = repo / "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs"
    path = repo / ".build/thuy-lao/ReviewedGame.cs"
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(apply_review(repo, baseline.read_text()), encoding="utf-8")


if __name__ == "__main__":
    write_regression_source(Path(__file__).resolve().parents[1])
