"""Reversible UI-only build hooks; fonts supplied locally, never redistributed."""
import hashlib
import json
from pathlib import Path

HOOK_BEFORE = '\t\t\tInitializeComponent();'
HOOK_AFTER = HOOK_BEFORE + '\n\t\t\tTinhKiemAuto.Appearance.AppAppearance.Apply(this);'


def review_data(repo):
    return json.loads((repo / 'tools/appearance_review.json').read_text())


def apply_ui_hook(text, count, reverse=False):
    before, after = (HOOK_AFTER, HOOK_BEFORE) if reverse else (HOOK_BEFORE, HOOK_AFTER)
    assert text.count(before) == count, 'Unexpected UI initialization count'
    return text.replace(before, after, count)


def restore_direct_source(repo, path, text):
    from typography_review import restore_source as restore_typography
    text = restore_typography(repo, path, text)
    from local_diagnostics_review import restore_source
    text = restore_source(repo, path, text)
    for edit in reversed(review_data(repo)['direct_source_edits']):
        if edit['path'] == path:
            assert text.count(edit['after']) == edit['count'], path
            text = text.replace(edit['after'], edit['before'], edit['count'])
    return text


def write_ui_sources(repo, prepared):
    review = review_data(repo)
    root = prepared / 'appearance'
    root.mkdir()
    baseline = repo / 'analysis/chickenautoex-107/recovered'
    for relative, item in review['recovered_ui_sources'].items():
        original = (baseline / relative).read_text()
        assert hashlib.sha256(original.encode()).hexdigest() == item['original_sha256_lf'], relative
        changed = apply_ui_hook(original, item['hook_count'])
        assert apply_ui_hook(changed, item['hook_count'], reverse=True) == original
        if relative == 'TinhKiemAuto/CanhBao.cs':
            from local_diagnostics_review import warning_hook
            changed = warning_hook(changed)
        target = root / relative
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(changed)


def verify_ui_sources(repo):
    from typography_review import verify_sources
    verify_sources(repo)
    layout = json.loads((repo / "tools/layout_review.json").read_text())
    for relative, expected in layout["additional_source_hashes_lf"].items():
        assert hashlib.sha256((repo / relative).read_text().encode()).hexdigest() == expected, relative
    review = review_data(repo)
    root = repo / '.build/chickenautoex/appearance'
    assert {str(p.relative_to(root)) for p in root.rglob('*.cs')} == set(review['recovered_ui_sources'])
    for relative, item in review['recovered_ui_sources'].items():
        changed = (root / relative).read_text()
        if relative == 'TinhKiemAuto/CanhBao.cs':
            from local_diagnostics_review import warning_hook
            changed = warning_hook(changed, reverse=True)
        assert apply_ui_hook(changed, item['hook_count'], reverse=True) == (repo / 'analysis/chickenautoex-107/recovered' / relative).read_text(), relative
    for relative, expected in review['assets_sha256'].items():
        assert hashlib.sha256((repo / relative).read_bytes()).hexdigest() == expected, relative
    for relative, expected in review['additional_source_hashes_lf'].items():
        assert hashlib.sha256((repo / relative).read_text().encode()).hexdigest() == expected, relative
    return len(review['recovered_ui_sources'])
