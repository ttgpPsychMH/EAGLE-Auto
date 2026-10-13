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
    for relative, expected in review_data(repo)['additional_source_hashes_lf'].items():
        assert hashlib.sha256((repo / relative).read_text().encode()).hexdigest() == expected, relative
