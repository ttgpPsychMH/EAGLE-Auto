"""Require nine controlled failures on v0.3 dungeon re-audit source; reject fixture errors."""
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as ET

repo = Path(__file__).resolve().parents[1]
results = repo / ".build/test-results"
results.mkdir(parents=True, exist_ok=True)
trx = results / "dungeon-reaudit-baseline.trx"
if trx.exists():
    trx.unlink()
run = subprocess.run(["dotnet", "test", "tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj",
                      "-c", "Release", "--no-build", "--no-restore", "--filter", "DungeonReauditBaseline=yes",
                      "--logger", "trx;LogFileName=dungeon-reaudit-baseline.trx", "--results-directory", str(results)],
                     cwd=repo, env=dict(os.environ, DUNGEON_REAUDIT_BASELINE="1"),
                     stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
(results / "dungeon-reaudit-baseline.log").write_text(run.stdout, encoding="utf-8")
assert run.returncode == 1 and trx.is_file(), "Expected completed baseline failures"
ns = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
tests = ET.parse(trx).getroot().findall(".//t:UnitTestResult", ns)
expected = {"GlobalPausePreventsThuyLaoCommands": "Assert.Empty() Failure",
            "UninitializedThuyLaoPreventsCommands": "Assert.Empty() Failure",
            "DisableDuringNpcGotoPreventsThuyLaoSelection": "Assert.DoesNotContain() Failure",
            "ParentDisabledDuringMemberGotoPreventsSelection": "Assert.DoesNotContain() Failure",
            "ThuyLaoDoesNotCommandMemberDoingTrungAc": "Assert.Empty() Failure",
            "GlobalPausePreventsKyCuocCommands": "Assert.Empty() Failure",
            "KyCuocDoesNotCommandMemberDoingTrungAc": "Assert.Empty() Failure",
            "DeadNineCancelsKyCuocBeforeCommands": "Assert.False() Failure",
            "ChangedMemberIdentityCancelsKyCuoc": "Assert.False() Failure"}
assert len(tests) == len(expected), "Expected exactly nine legacy controls"
for test in tests:
    name = test.attrib["testName"].split(".")[-1]
    assert test.attrib["outcome"] == "Failed" and name in expected, name
    message = test.findtext("t:Output/t:ErrorInfo/t:Message", namespaces=ns)
    assert message.startswith(expected[name]), "Fixture/runtime failure instead of expected regression: " + name
summary = {"legacy_tests_executed": 9, "expected_behavioral_failures": 9, "harness_errors": 0,
           "scope": "Restored, hash-checked v0.3 partial methods with fake dependencies; original EXE/native DLL never loaded"}
(results / "dungeon-reaudit-baseline-summary.json").write_text(json.dumps(summary, indent=2) + "\n")
print("PASS: nine v0.3 dungeon re-audit regressions reproduced; no harness errors.")
