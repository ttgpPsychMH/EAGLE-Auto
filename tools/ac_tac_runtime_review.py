"""Exact v0.14 Ac Tac ownership hooks; reversible independently of historical reviews."""
import hashlib
import json


def apply_review(repo, text, reverse=False):
    review = json.loads((repo / 'tools/ac_tac_runtime_review.json').read_text())
    for edit in reversed(review['edits']) if reverse else review['edits']:
        variants = edit.get('variants', [edit])
        matches = [v for v in variants if text.count(v['reviewed' if reverse else 'original']) == 1]
        assert len(matches) == 1, edit['name']
        selected = matches[0]
        before, after = (selected['reviewed'], selected['original']) if reverse else (selected['original'], selected['reviewed'])
        text = text.replace(before, after, 1)
    for path, expected in review['additional_source_hashes_lf'].items():
        from dungeon_party_review import restore_source
        assert hashlib.sha256(restore_source(repo, path).encode()).hexdigest() == expected, path
    return text
