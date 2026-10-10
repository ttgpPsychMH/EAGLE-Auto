"""Reverse reviewed v0.4 partial-method changes before checking v0.2/v0.3 hashes."""
import hashlib
import json


def baseline_partial(repo, path):
    text = (repo / path).read_text(encoding="utf-8")
    reviews = json.loads((repo / "tools/dungeon_reaudit_review.json").read_text())
    entry = reviews["partials"].get(path)
    if entry is None:
        return text
    assert hashlib.sha256(text.encode()).hexdigest() == entry["reviewed_sha256_lf"], path
    for edit in reversed(entry["edits"]):
        assert text.count(edit["reviewed"]) == edit.get("count", 1), edit["name"]
        text = text.replace(edit["reviewed"], edit["original"], edit.get("count", 1))
    assert hashlib.sha256(text.encode()).hexdigest() == entry["baseline_sha256_lf"], path
    return text
