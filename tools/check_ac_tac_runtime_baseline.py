"""Require eight behavioral failures on the hash-checked v0.13 Ac Tac coordinator."""
import hashlib
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as ET

repo = Path(__file__).resolve().parents[1]
baseline = repo / 'analysis/ac-tac/baselines/v0.13-Game.AcTac.cs.txt'
assert hashlib.sha256(baseline.read_bytes()).hexdigest() == '7f7f2c512c7b3353b641fd86e2a23285a1af987dd0e71ba1d0bd453b10380549'
results = repo / '.build/test-results'
results.mkdir(parents=True, exist_ok=True)
trx = results / 'ac-tac-runtime-baseline.trx'
trx.unlink(missing_ok=True)
run = subprocess.run(['dotnet', 'test', 'tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj',
                      '-c', 'Release', '--no-build', '--no-restore', '--filter', 'AcTacRuntimeBaseline=yes',
                      '--logger', 'trx;LogFileName=ac-tac-runtime-baseline.trx', '--results-directory', str(results)],
                     cwd=repo, env=dict(os.environ, AC_TAC_RUNTIME_BASELINE='1'),
                     stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
(results / 'ac-tac-runtime-baseline.log').write_text(run.stdout)
assert run.returncode == 1 and trx.is_file(), 'Expected completed baseline failures'
ns = {'t': 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
tests = ET.parse(trx).getroot().findall('.//t:UnitTestResult', ns)
expected = {
    'UnreadyMemberDoesNotStarveReadyMembersDuringEntry': 'Assert.Contains() Failure',
    'CombatInsideCannotPreventRemainingMemberEntering': 'Assert.Contains() Failure',
    'MemberSearchesNpcIndependentlyAfterLeaderEntered': 'Assert.Contains() Failure',
    'TeamFollowRequestIsThrottledAndStopsWhenFollowObserved': 'Assert.Single() Failure',
    'MemberRegroupUsesSameMapMoveAndDismountsWithoutGoto': 'Assert.Contains() Failure',
    'CoordinatorSuppressesOrdinaryFollowAndReleasesOnCancel': 'Assert.Empty() Failure',
    'TravelProgressPreventsFalseAbsoluteTripTimeout': 'Assert.Equal() Failure',
    'CombatWithoutHpProgressCannotHoldPatrolForever': 'Assert.Equal() Failure',
}
assert len(tests) == len(expected), 'Expected exactly eight v0.13 controls'
for test in tests:
    name = test.attrib['testName'].split('.')[-1]
    message = test.findtext('t:Output/t:ErrorInfo/t:Message', namespaces=ns)
    assert test.attrib['outcome'] == 'Failed' and name in expected, name
    assert message.startswith(expected[name]), 'Fixture failure instead of behavioral regression: ' + name
summary = {'baseline_commit': 'f948e01', 'legacy_tests_executed': 8, 'expected_behavioral_failures': 8,
           'harness_errors': 0, 'baseline_partial_sha256': hashlib.sha256(baseline.read_bytes()).hexdigest(),
           'scope': 'Actual C# with fake readers/commands/clock; original EXE and native DLL never loaded'}
(results / 'ac-tac-runtime-baseline-summary.json').write_text(json.dumps(summary, indent=2) + '\n')
print('PASS: eight v0.13 Ac Tac regressions reproduced; no harness errors.')
