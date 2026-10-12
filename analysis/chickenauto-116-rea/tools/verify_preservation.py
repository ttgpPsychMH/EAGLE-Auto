"""Verify report-only scope and all original releases without executing targets."""
import hashlib
import json
from pathlib import Path
import subprocess


def main():
    repo = Path(__file__).resolve().parents[3]
    base = '19c6720f82a60e59e595229c99111434ed5fd8df'
    inv = json.loads((repo / 'analysis/chickenauto-116-vs-107/evidence/inventory.json').read_text())
    changed = subprocess.check_output(['git', 'diff', '--name-only', base], cwd=repo, text=True).splitlines()
    untracked = subprocess.check_output(['git', 'ls-files', '--others', '--exclude-standard'], cwd=repo, text=True).splitlines()
    assert all(path.startswith('analysis/chickenauto-116-rea/') for path in changed + untracked), changed + untracked
    for name, expected in inv['original_files_sha256'].items():
        assert hashlib.sha256((repo / name).read_bytes()).hexdigest() == expected, name
    work = repo / '.build/chickenauto-116-rea'
    for name, expected in {'ChickenAuto-116.exe': '34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea',
                           'EasyHook-116.dll': 'f22b7f349f20f94327c365d47a56484092c45dba3fa6b4fa76231f4f49336cef'}.items():
        assert hashlib.sha256((work / name).read_bytes()).hexdigest() == expected
    result = {'base_commit': base, 'report_only_scope_verified': True, 'original_release_files_unchanged': 6,
              'recovered_snapshot_unchanged': True, 'application_source_tests_version_ci_unchanged': True,
              'licensing_vip_unchanged': True, 'stable_116_original_and_extracted_copies_unchanged': True,
              'target_executables_or_dlls_run': False, 'windows_or_game_runtime_tested': False}
    (work / 'preservation.json').write_text(json.dumps(result, indent=2) + '\n')
    print('PASS: report-only changes, six original releases and both 116 analysis inputs preserved; v0.7 app untouched.')


if __name__ == '__main__':
    main()
