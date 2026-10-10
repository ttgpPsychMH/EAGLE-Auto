"""Require eight behavioral failures on v0.5; reject compilation/fixture failures."""
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as ET

repo = Path(__file__).resolve().parents[1]
results = repo / '.build/test-results'
results.mkdir(parents=True, exist_ok=True)
trx = results / 'ac-ba-baseline.trx'
if trx.exists():
    trx.unlink()
run = subprocess.run(['dotnet', 'test', 'tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj',
                      '-c', 'Release', '--no-build', '--no-restore', '--filter', 'AcBaBaseline=yes',
                      '--logger', 'trx;LogFileName=ac-ba-baseline.trx', '--results-directory', str(results)],
                     cwd=repo, env=dict(os.environ, AC_BA_BASELINE='1'),
                     stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
(results / 'ac-ba-baseline.log').write_text(run.stdout)
assert run.returncode == 1 and trx.is_file(), 'Expected completed baseline failures'
ns = {'t': 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
tests = ET.parse(trx).getroot().findall('.//t:UnitTestResult', ns)
expected = {'UnknownEventDoesNotWalkToOwnSchool':'Assert.Empty() Failure',
            'OutdoorCalendarIsBusy':'Assert.True() Failure', 'AutoOffClearsAcBa':'Assert.False() Failure',
            'ActiveAcBaRequestsSystemHook':'Assert.True() Failure', 'PausedEngineSendsNoCommands':'Assert.Empty() Failure',
            'WrongDungeonIsNeverPatrolled':'Assert.Empty() Failure', 'UnobservedDeadBossIsNotCompletion':'Assert.False() Failure',
            'WrongDialogNeverClicksAll':'Assert.DoesNotContain() Failure'}
assert len(tests) == len(expected), 'Expected exactly eight controls'
for test in tests:
    name = test.attrib['testName'].split('.')[-1]
    assert test.attrib['outcome'] == 'Failed' and name in expected, name
    message = test.findtext('t:Output/t:ErrorInfo/t:Message', namespaces=ns)
    assert message.startswith(expected[name]), 'Fixture/runtime error: ' + name
summary = {'legacy_tests_executed':8, 'expected_behavioral_failures':8, 'harness_errors':0,
           'scope':'Selected redacted v0.5 C# against fake dependencies; EXE/native DLL never loaded'}
(results / 'ac-ba-baseline-summary.json').write_text(json.dumps(summary, indent=2)+'\n')
print('PASS: eight v0.5 Ac Ba regressions reproduced; no harness errors.')
