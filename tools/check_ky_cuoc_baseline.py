"""Require six controlled failures on original Ky Cuoc source; reject fixture errors."""
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as ET

repo = Path(__file__).resolve().parents[1]
results = repo / ".build/test-results"
results.mkdir(parents=True, exist_ok=True)
trx = results / "ky-cuoc-baseline.trx"
if trx.exists():
    trx.unlink()
run = subprocess.run(["dotnet", "test", "tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj",
                      "-c", "Release", "--no-build", "--no-restore", "--filter", "KyCuocBaseline=yes",
                      "--logger", "trx;LogFileName=ky-cuoc-baseline.trx", "--results-directory", str(results)],
                     cwd=repo, env=dict(os.environ, KY_CUOC_BASELINE="1"),
                     stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
(results / "ky-cuoc-baseline.log").write_text(run.stdout, encoding="utf-8")
assert run.returncode == 1 and trx.is_file(), "Expected completed baseline failures"
ns = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
tests = ET.parse(trx).getroot().findall(".//t:UnitTestResult", ns)
expected = {"NewRunResetsCompletion": "Assert.True() Failure",
            "DisabledMenuDoesNotMoveAfterBossDeath": "Assert.Empty() Failure",
            "AutoOffMemberReceivesNoCommands": "Assert.Empty() Failure",
            "ExitTravelDoesNotMeanCompleted": "Assert.False() Failure",
            "CombatResumesNearCurrentPositionInsteadOfOldWaypoint": "Assert.Contains() Failure",
            "PetAoeWithEmptyMonstersDoesNotThrow": "System.ArgumentOutOfRangeException"}
assert len(tests) == len(expected), "Expected exactly six legacy controls"
for test in tests:
    name = test.attrib["testName"].split(".")[-1]
    assert test.attrib["outcome"] == "Failed" and name in expected, name
    message = test.findtext("t:Output/t:ErrorInfo/t:Message", namespaces=ns)
    assert message.startswith(expected[name]), "Fixture/runtime failure instead of expected regression: " + name
    if name == "PetAoeWithEmptyMonstersDoesNotThrow":
        stack = test.findtext("t:Output/t:ErrorInfo/t:StackTrace", namespaces=ns)
        assert "TinhKiemAuto.Game.AOE()" in stack, "Expected actual legacy AOE indexing failure"
summary = {"legacy_tests_executed": 6, "expected_behavioral_failures": 6, "harness_errors": 0,
           "scope": "Selected original C# methods with fake dependencies; original EXE/native DLL never loaded"}
(results / "ky-cuoc-baseline-summary.json").write_text(json.dumps(summary, indent=2) + "\n")
print("PASS: six original Ky Cuoc/pet AOE regressions reproduced; no harness errors.")
