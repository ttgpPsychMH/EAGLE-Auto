"""Exact reversible hook diagnostic deltas; never modify the recovered snapshot."""
import hashlib
import json


def apply_review(repo, text, reverse=False, form=False):
    review = json.loads((repo / "tools/hook_diagnostics_review.json").read_text())
    edits = review["form_edits" if form else "edits"]
    for edit in reversed(edits) if reverse else edits:
        before, after = (edit["reviewed"], edit["original"]) if reverse else (edit["original"], edit["reviewed"])
        assert text.count(before) == edit["count"], edit["name"]
        text = text.replace(before, after, edit["count"])
    for path, expected in review["additional_source_hashes_lf"].items():
        assert hashlib.sha256((repo / path).read_text().encode()).hexdigest() == expected, path
    return text


def write_regression_source(repo):
    legacy = (repo / ".build/ac-ba-repair/ReviewedGame.cs").read_text()
    reviewed = apply_review(repo, legacy)
    assert apply_review(repo, reviewed, reverse=True) == legacy
    path = repo / ".build/hook-diagnostics"
    path.mkdir(parents=True, exist_ok=True)
    (path / "ReviewedGame.cs").write_text(reviewed)
