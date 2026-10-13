"""Exact reversible v0.15 managed deltas; historical snapshots and native/auth code remain protected."""
import hashlib
import json


def restore_source(repo, path, text=None):
    review = json.loads((repo / 'tools/dungeon_party_review.json').read_text())
    if text is None:
        text = (repo / path).read_text()
    entry = review['source_edits'].get(path)
    if entry is None:
        return text
    assert hashlib.sha256(text.encode()).hexdigest() == entry['reviewed_sha256_lf'], path
    for edit in reversed(entry['edits']):
        assert text.count(edit['reviewed']) == 1, path
        text = text.replace(edit['reviewed'], edit['original'], 1)
    assert hashlib.sha256(text.encode()).hexdigest() == entry['original_sha256_lf'], path
    return text


def apply_game(repo, text, reverse=False):
    review = json.loads((repo / 'tools/dungeon_party_review.json').read_text())
    for edit in reversed(review['game_edits']) if reverse else review['game_edits']:
        before, after = (edit['reviewed'], edit['original']) if reverse else (edit['original'], edit['reviewed'])
        assert text.count(before) == 1, edit['name']
        text = text.replace(before, after, 1)
    for path, expected in review['additional_source_hashes_lf'].items():
        assert hashlib.sha256((repo / path).read_text().encode()).hexdigest() == expected, path
    return text
