"""Reversible observation hooks only; do not change game actions or entitlement checks."""
import hashlib
import json

WARNING_BEFORE = '\t\t\t_instance = this;'
WARNING_AFTER = '\t\t\tTinhKiemAuto.Diagnostics.LocalDiagnostics.Warning(tieude, noidung, (int)type);\n' + WARNING_BEFORE


def warning_hook(text, reverse=False):
    before, after = (WARNING_AFTER, WARNING_BEFORE) if reverse else (WARNING_BEFORE, WARNING_AFTER)
    assert text.count(before) == 1, 'Warning constructor observation hook changed'
    return text.replace(before, after, 1)


def restore_source(repo, path, text):
    review = json.loads((repo / 'tools/local_diagnostics_review.json').read_text())
    for edit in reversed(review['direct_source_edits']):
        if path == edit['path']:
            assert text.count(edit['after']) == edit['count'], path
            text = text.replace(edit['after'], edit['before'], edit['count'])
    return text


def verify_sources(repo):
    review = json.loads((repo / 'tools/local_diagnostics_review.json').read_text())
    actual = {str(path.relative_to(repo)) for path in (repo / 'src/ChickenAutoEx/Diagnostics').glob('*.cs')}
    actual.add('src/ChickenAutoEx/FrmMain.LocalDiagnostics.cs')
    assert actual == set(review['additional_source_hashes_lf']), 'Unreviewed diagnostic source'
    for path, expected in review['additional_source_hashes_lf'].items():
        assert hashlib.sha256((repo / path).read_text().encode()).hexdigest() == expected, path
    return True
