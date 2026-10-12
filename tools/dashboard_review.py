"""Exact reversible UI delta; preserve the entire v0.7 engine and old handlers."""
import hashlib
import json


def apply_review(repo, text, reverse=False):
    review = json.loads((repo / "tools/dashboard_review.json").read_text())
    if not reverse:
        assert hashlib.sha256(text.encode()).hexdigest() == review["legacy_form_sha256_lf"]
    for edit in reversed(review["form_edits"]) if reverse else review["form_edits"]:
        before, after = (edit["reviewed"], edit["original"]) if reverse else (edit["original"], edit["reviewed"])
        assert text.count(before) == edit["count"], edit["name"]
        text = text.replace(before, after, edit["count"])
    if reverse:
        assert hashlib.sha256(text.encode()).hexdigest() == review["legacy_form_sha256_lf"]
    for path, expected in {**review["additional_source_hashes_lf"], **review["preserved_v07_source_hashes_lf"],
                           **review["preserved_v08_source_hashes_lf"]}.items():
        assert hashlib.sha256((repo / path).read_text().encode()).hexdigest() == expected, path
    return text
