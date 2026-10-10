"""Require eleven controlled failures on original Trung Ac source; reject fixture errors."""
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as ET

repo = Path(__file__).resolve().parents[1]
results = repo / ".build/test-results"
results.mkdir(parents=True, exist_ok=True)
trx = results / "trung-ac-baseline.trx"
if trx.exists():
    trx.unlink()
run = subprocess.run(["dotnet", "test", "tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj",
                      "-c", "Release", "--no-build", "--no-restore", "--filter", "TrungAcBaseline=yes",
                      "--logger", "trx;LogFileName=trung-ac-baseline.trx", "--results-directory", str(results)],
                     cwd=repo, env=dict(os.environ, TRUNG_AC_BASELINE="1"),
                     stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
(results / "trung-ac-baseline.log").write_text(run.stdout, encoding="utf-8")
assert run.returncode == 1 and trx.is_file(), "Expected completed baseline failures"
ns = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
tests = ET.parse(trx).getroot().findall(".//t:UnitTestResult", ns)
expected = {"EmptyDialogDoesNotThrow": "Assert.Null() Failure",
            "MissingYDoesNotThrow": "Assert.Null() Failure",
            "DisabledTaskNeverAbandonsQuestFromUiAlarm": "Assert.Empty() Failure",
            "NewRunClearsOldCompletion": "Assert.False() Failure",
            "DelayedAcceptDialogKeepsWaitingState": "Assert.True() Failure",
            "CorpseAloneDoesNotTriggerQuestReturn": "Assert.False() Failure",
            "MultipleLiveTargetsReceiveOneStableSelection": "Assert.Single() Failure",
            "SpawnUsesTokenOnceThenWaits": "Assert.Single() Failure",
            "ActiveTaskBlocksCalendarInOrdinaryMap": "Assert.True() Failure",
            "ClearMissionCancelsTask": "Assert.False() Failure",
            "LoginDoesNotRestartTaskAfterManualOff": "Assert.False() Failure"}
assert len(tests) == len(expected), "Expected exactly eleven legacy controls"
for test in tests:
    name = test.attrib["testName"].split(".")[-1]
    assert test.attrib["outcome"] == "Failed" and name in expected, name
    message = test.findtext("t:Output/t:ErrorInfo/t:Message", namespaces=ns)
    assert message.startswith(expected[name]), "Fixture/runtime failure instead of expected regression: " + name
    if name in ("EmptyDialogDoesNotThrow", "MissingYDoesNotThrow"):
        assert "TinhKiemAuto.Game.TrungAc()" in message, "Expected actual legacy parser failure"
    if name in ("MultipleLiveTargetsReceiveOneStableSelection", "SpawnUsesTokenOnceThenWaits"):
        assert "2 matching" in message, "Expected repeated legacy commands"
summary = {"legacy_tests_executed": 11, "expected_behavioral_failures": 11, "harness_errors": 0,
           "scope": "Selected original C# methods with fake dependencies; original EXE/native DLL never loaded"}
(results / "trung-ac-baseline-summary.json").write_text(json.dumps(summary, indent=2) + "\n")
print("PASS: eleven original Trung Ac regressions reproduced; no harness errors.")
