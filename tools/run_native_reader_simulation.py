"""Build/run only our portable C++ model with synthetic data; never load a game DLL."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--compiler', default='g++')
    args = parser.parse_args()
    repo = Path(__file__).resolve().parents[1]
    source = repo / 'tools/native_reader_simulation'
    work = repo / '.build/native-reader-simulation'
    work.mkdir(parents=True, exist_ok=True)
    output = work / 'reader-simulation'
    for path in (work / 'fixtures.json', work / 'summary.json'):
        path.unlink(missing_ok=True)
    command = [args.compiler, '-std=c++17', '-Wall', '-Wextra', '-Werror', '-pthread', '-O2',
               str(source / 'main.cpp'), '-o', str(output)]
    subprocess.run(command, check=True, timeout=60)
    run = subprocess.run([str(output)], check=True, stdout=subprocess.PIPE, timeout=20)
    result = json.loads(run.stdout)
    if result['passed'] < 20 or result['failed'] or len(result['fixtures']) != 11:
        raise ValueError('Incomplete native model run')
    (work / 'fixtures.json').write_bytes(run.stdout)
    summary = {k: v for k, v in result.items() if k != 'fixtures'}
    summary.update(source_sha256={p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in source.glob('*') if p.is_file()},
                   fixture_count=len(result['fixtures']), compiler=subprocess.check_output([args.compiler, '--version'], text=True).splitlines()[0],
                   build_command=command, model_executable_sha256=hashlib.sha256(output.read_bytes()).hexdigest(),
                   fixture_sha256=hashlib.sha256(run.stdout).hexdigest())
    (work / 'summary.json').write_text(json.dumps(summary, indent=2) + '\n')
    print(f"PASS: {result['passed']} C++ behavioral scenarios, {len(result['fixtures'])} synthetic cross-language fixtures; no Windows APIs or original DLL executed.")


if __name__ == '__main__':
    main()
