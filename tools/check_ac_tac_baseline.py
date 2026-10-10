"""Require thirteen controlled failures on v0.4 Ac Tac source; reject fixture errors."""
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as ET

repo = Path(__file__).resolve().parents[1]
results = repo / ".build/test-results"
results.mkdir(parents=True, exist_ok=True)
trx = results / "ac-tac-baseline.trx"
if trx.exists():
    trx.unlink()
run = subprocess.run(["dotnet", "test", "tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj",
                      "-c", "Release", "--no-build", "--no-restore", "--filter", "AcTacBaseline=yes",
                      "--logger", "trx;LogFileName=ac-tac-baseline.trx", "--results-directory", str(results)],
                     cwd=repo, env=dict(os.environ, AC_TAC_BASELINE="1"),
                     stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
(results / "ac-tac-baseline.log").write_text(run.stdout, encoding="utf-8")
assert run.returncode == 1 and trx.is_file(), "Expected completed baseline failures"
ns = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
tests = ET.parse(trx).getroot().findall(".//t:UnitTestResult", ns)
expected = {"SelectedMapDoesNotChangeWithCurrentMap": "Assert.Equal() Failure",
            "CalendarIsBusyOutsideDungeon": "Assert.True() Failure",
            "RandomPoolIncludesAllSixMaps": "Assert.Equal() Failure",
            "RouteEndAndQuietTimeDoNotProveBossDeath": "Assert.False() Failure",
            "AutoOffMemberGetsNoCommands": "Assert.Empty() Failure",
            "PauseSendsNoCommands": "Assert.Empty() Failure",
            "AutoOffGuardCancelsMap": "Assert.Equal() Failure",
            "NegativeLegacyIndexCannotCrashEngine": "Assert.Null() Failure",
            "WrongDialogNeverClicksAll": "Assert.DoesNotContain() Failure",
            "MonsterNamedAsEntryNpcIsNotTalkedTo": "Assert.DoesNotContain() Failure",
            "Generic272DoesNothingWhenGlobalFlagOff": "Assert.Empty() Failure",
            "AcTacSelectionDoesNotDisableOtherTeams": "Assert.True() Failure",
            "AcTacSelectionStopsConflictingLeaderQuest": "Assert.False() Failure"}
assert len(tests) == len(expected), "Expected exactly thirteen legacy controls"
for test in tests:
    name = test.attrib["testName"].split(".")[-1]
    assert test.attrib["outcome"] == "Failed" and name in expected, name
    message = test.findtext("t:Output/t:ErrorInfo/t:Message", namespaces=ns)
    assert message.startswith(expected[name]), "Fixture/runtime failure instead of expected regression: " + name
    if name == "NegativeLegacyIndexCannotCrashEngine":
        assert "IndexOutOfRangeException" in message and "TinhKiemAuto.Game.DatDoiAcTac()" in message
summary = {"legacy_tests_executed": 13, "expected_behavioral_failures": 13, "harness_errors": 0,
           "scope": "Selected v0.4 C# methods and UI with fake dependencies; original EXE/native DLL never loaded"}
(results / "ac-tac-baseline-summary.json").write_text(json.dumps(summary, indent=2) + "\n")
print("PASS: thirteen v0.4 Ac Tac regressions reproduced; no harness errors.")
