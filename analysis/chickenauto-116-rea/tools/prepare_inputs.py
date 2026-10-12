"""Extract and identify the pinned 116 payload and Qt DLL without target execution."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import sys
import zipfile
import pefile


def sha(data):
    return hashlib.sha256(data).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    repo = Path(__file__).resolve().parents[3]
    work = args.output or repo / '.build/chickenauto-116-rea'
    work.mkdir(parents=True, exist_ok=True)
    inv = json.loads((repo / 'analysis/chickenauto-116-vs-107/evidence/inventory.json').read_text())
    for name, expected in inv['original_files_sha256'].items():
        assert sha((repo / name).read_bytes()) == expected, name
    with zipfile.ZipFile(repo / 'ChickenAuto-new.zip') as archive:
        assert archive.namelist() == ['ChickenAuto.exe'] and archive.testzip() is None
        executable = archive.read('ChickenAuto.exe')
    assert sha(executable) == '34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea'
    # Reuse the format-aware, pinned Qt rcc parser from the previous investigation.
    sys.path.insert(0, str(repo / 'analysis/chickenauto-116-vs-107/tools'))
    from compare_static import qrc_files
    resources = qrc_files(executable)
    info, dll = next(item for item in resources if item[0]['path'] == ':/Resources/EasyHook.dll')
    assert sha(dll) == 'f22b7f349f20f94327c365d47a56484092c45dba3fa6b4fa76231f4f49336cef'
    identity = {'original_files_sha256': inv['original_files_sha256'], 'qt_hook_resource': info,
                'executable_sha256': sha(executable), 'dll_sha256': sha(dll), 'target_binaries_executed': False}
    for name, data in [('ChickenAuto-116.exe', executable), ('EasyHook-116.dll', dll)]:
        path = work / name
        if path.exists():
            assert sha(path.read_bytes()) == sha(data), 'Refusing to replace unrelated input: ' + name
        else:
            path.write_bytes(data)
    (work / 'input-identity.json').write_text(json.dumps(identity, indent=2) + '\n')
    print('PASS: pinned release/Qt resource identities; 116 payload and DLL extracted only as data.')


if __name__ == '__main__':
    main()
