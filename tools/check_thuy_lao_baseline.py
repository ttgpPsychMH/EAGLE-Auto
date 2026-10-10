"""Require four behavioral regressions on immutable legacy methods, not harness errors."""
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as ET

repo = Path(__file__).resolve().parents[1]
results = repo / ".build/test-results"
results.mkdir(parents=True, exist_ok=True)
trx = results / "thuy-lao-baseline.trx"
if trx.exists():
    trx.unlink()
environment = dict(os.environ, THUY_LAO_BASELINE="1")
run = subprocess.run(["dotnet", "test", "tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj",
                      "-c", "Release", "--no-build", "--no-restore", "--filter", "ThuyLaoBaseline=yes",
                      "--logger", "trx;LogFileName=thuy-lao-baseline.trx", "--results-directory", str(results)],
                     cwd=repo, env=environment, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
(results / "thuy-lao-baseline.log").write_text(run.stdout, encoding="utf-8")
assert run.returncode == 1 and trx.is_file(), "Baseline runner did not complete with expected behavioral failures"
ns = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
root = ET.parse(trx).getroot()
tests = root.findall(".//t:UnitTestResult", ns)
expected = {"EntryFromAnotherMapTargetsTheNpcMap": "Assert.Contains() Failure",
            "AutoOffMemberReceivesNoCommands": "Assert.Empty() Failure",
            "MemberAlreadyInsideWaitsForLeaderOutside": "Assert.Empty() Failure",
            "RouteEndDoesNotMeanQuestCompleted": "Assert.False() Failure"}
assert len(tests) == len(expected), "Expected exactly four baseline tests"
for test in tests:
    name = test.attrib["testName"].split(".")[-1]
    assert test.attrib["outcome"] == "Failed" and name in expected, name
    message = test.findtext("t:Output/t:ErrorInfo/t:Message", namespaces=ns)
    assert message.startswith(expected[name]), "Harness/runtime failure instead of the expected assertion: " + name
summary = {"legacy_tests_executed": 4, "expected_behavioral_failures": 4, "harness_errors": 0,
           "scope": "Same simulation assertions against original recovered methods, never the automation executable"}
(results / "thuy-lao-baseline-summary.json").write_text(json.dumps(summary, indent=2) + "\n")
print("PASS: all four original Thuy Lao behavioral regressions reproduced; no harness errors.")
