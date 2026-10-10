"""Exact, reversible Ac Ba deltas after v0.5; generated source stays redacted."""
import hashlib
import json
from pathlib import Path


def apply_review(repo, text, reverse=False, form=False):
    review = json.loads((repo / 'tools/ac_ba_review.json').read_text())
    edits = review['form_edits' if form else 'edits']
    for edit in reversed(edits) if reverse else edits:
        before, after = (edit['reviewed'], edit['original']) if reverse else (edit['original'], edit['reviewed'])
        if reverse and not before:
            before, after = edit['anchor'], after + edit['anchor']
        assert text.count(before) == edit['count'], edit['name']
        text = text.replace(before, after, edit['count'])
    for path, expected in review['additional_source_hashes_lf'].items():
        assert hashlib.sha256((repo / path).read_text().encode()).hexdigest() == expected, path
    return text


def write_regression_source(repo):
    from ac_tac_review import apply_review as ac_tac
    from trung_ac_review import apply_review as trung
    from ky_cuoc_review import apply_review as ky
    from thuy_lao_review import apply_review as thuy
    original = (repo / 'analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs').read_text()
    legacy = ac_tac(repo, trung(repo, ky(repo, thuy(repo, original))))
    path = repo / '.build/ac-ba-repair'
    path.mkdir(parents=True, exist_ok=True)
    (path / 'LegacyGame.cs').write_text(legacy)
    reviewed = apply_review(repo, legacy)
    assert apply_review(repo, reviewed, reverse=True) == legacy
    (path / 'ReviewedGame.cs').write_text(reviewed)
    form = (repo / 'src/ChickenAutoEx/FrmMain.cs').read_text()
    legacy_form = apply_review(repo, form, reverse=True, form=True)
    assert apply_review(repo, legacy_form, form=True) == form
    (path / 'LegacyFrmMain.cs').write_text(legacy_form)


if __name__ == '__main__':
    write_regression_source(Path(__file__).resolve().parents[1])
