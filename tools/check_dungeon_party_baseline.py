"""Reproduce moving-state cancellation in four hash-checked v0.14 dungeon modules."""
import hashlib,json,os,subprocess,xml.etree.ElementTree as ET
from pathlib import Path
repo=Path(__file__).resolve().parents[1]
review=json.loads((repo/'tools/dungeon_party_review.json').read_text())
for kind in ('KyCuoc','AcTac','AcBa','ThuyLao'):
 path=f'src/ChickenAutoEx/{kind}/Game.{kind}.cs'
 before=(repo/f'analysis/dungeon-party-v015/baselines/Game.{kind}.cs.txt').read_text()
 assert hashlib.sha256(before.encode()).hexdigest()==review['source_edits'][path]['original_sha256_lf'],kind
results=repo/'.build/test-results';results.mkdir(parents=True,exist_ok=True)
trx=results/'dungeon-party-baseline.trx';trx.unlink(missing_ok=True)
run=subprocess.run(['dotnet','test','tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj','-c','Release','--no-build','--no-restore','--filter','DungeonPartyBaseline=yes','--logger','trx;LogFileName=dungeon-party-baseline.trx','--results-directory',str(results)],cwd=repo,env=dict(os.environ,DUNGEON_PARTY_BASELINE='1'),stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True)
(results/'dungeon-party-baseline.log').write_text(run.stdout)
assert run.returncode==1 and trx.is_file(),'Expected four completed behavioral failures'
ns={'t':'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
tests=ET.parse(trx).getroot().findall('.//t:UnitTestResult',ns)
assert len(tests)==4
for test in tests:
 assert test.attrib['outcome']=='Failed',test.attrib['testName']
 assert test.findtext('t:Output/t:ErrorInfo/t:Message',namespaces=ns).startswith('Assert.True() Failure'),'Harness error'
summary={'baseline_commit':'f5dc6a9','executed':4,'expected_behavioral_failures':4,'harness_errors':0,'scope':'Actual managed dungeon methods; fake state=2, readers, clocks and commands. No EXE/native DLL execution.'}
(results/'dungeon-party-baseline-summary.json').write_text(json.dumps(summary,indent=2)+'\n')
print('PASS: four v0.14 moving-state cancellation regressions reproduced.')
