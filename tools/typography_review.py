"""Review fixed UI copy independently and reverse only the declared literal edits."""
import hashlib
import json


def review_data(repo):
    return json.loads((repo / 'tools/typography_review.json').read_text())


def restore_source(repo, path, text):
    for edit in reversed(review_data(repo)['direct_source_edits']):
        if edit['path'] == path:
            assert text.count(edit['after']) == edit['count'], path
            text = text.replace(edit['after'], edit['before'], edit['count'])
    return text


def verify_sources(repo):
    from dungeon_party_review import restore_source as restore_party
    for relative, expected in review_data(repo)['additional_source_hashes_lf'].items():
        assert hashlib.sha256(restore_party(repo, relative).encode()).hexdigest() == expected, relative
