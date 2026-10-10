"""Read legacy source and cached IL as data; export only Ac Tac review evidence."""
from pathlib import Path
import hashlib,json,re,subprocess
repo=Path(__file__).resolve().parents[3]
base=repo/'analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs'
s=base.read_text()
methods={}
for name in ['DatDoiAcTac','AcTac','TalkNPCPhuBan','RandomAcTac','TrieuTap','SetCalendar','ClearMission']:
 m=re.search(r'^\t\t(?:private|public) [^\n]+ '+name+r'\([^\n]*\)\n\t\t\{\n.*?^\t\t\}\n',s,re.M|re.S);assert m,name
 methods[name]={'line':s[:m.start()].count('\n')+1,'sha256_lf':hashlib.sha256(m.group().encode()).hexdigest()}
checks={'map_getter_uses_current_map':'return TLBB.MapId;' in s[s.index('public int MapAcTac'):s.index('public int MapTKC')],
 'random_upper_bound_excludes_4':'new Random().Next(0, 4)' in s and 'if (num == 4)' in s,
 'completion_from_quiet_time':'ClearTime.Elapsed.TotalSeconds > 10.0' in s and 'IsBossDie = true;' in s,
 'shared_dialog_click_all':'QuestFrame.ClickAll();' in s,
 'route_index_only_upper_bound':'MapATIndex <= acTacPoint.GetLength(0) - 1' in s}
assert all(checks.values())
r={'baseline_commit':'9ae98bd','application_version':'0.4','scope':'Pre-fix static review; no target/native execution',
 'method_source':'Immutable original Game.cs snapshot; menu lifecycle comparison uses the v0.4 baseline FrmMain.cs',
 'methods':methods,'static_checks':checks,'files':{}}
for p in [base,repo/'src/ChickenAutoEx/FrmMain.cs',repo/'src/ChickenAutoEx/Global.cs',base.parent/'POINT.cs',base.parent/'MAP.cs',base.parent/'GameObjects.cs',base.parent/'QuestFrame.cs']:
 relative=str(p.relative_to(repo))
 data=subprocess.check_output(['git','show','9ae98bd:'+relative],cwd=repo) if p.name=='FrmMain.cs' else p.read_bytes()
 r['files'][relative]=hashlib.sha256(data).hexdigest()
out=repo/'analysis/ac-tac/evidence';out.mkdir(parents=True,exist_ok=True)
il=repo/'.build/trung-ac-original.il'
if il.exists():
 assert hashlib.sha256(il.read_bytes()).hexdigest()=='0a4fa65c52e48053aec0590ff1229c3fb794f848d373f51b411f1c2a9b569c35', 'Unreviewed IL cache'
 text=il.read_text()
 for name in ['DatDoiAcTac','TalkNPCPhuBan','RandomAcTac']:
  end=text.index('} // end of method Game::'+name)+len('} // end of method Game::'+name)
  start=text.rfind('\t.method ',0,end);assert start>=0
  (out/(name+'.il.txt')).write_text('\n'.join(line.rstrip() for line in text[start:end].splitlines())+'\n')
 r['il_input_sha256']=hashlib.sha256(il.read_bytes()).hexdigest()
(out/'source-inventory.json').write_text(json.dumps(r,ensure_ascii=False,indent=2)+'\n')
print('PASS: Ac Tac static patterns and selected source/IL evidence collected; no target executed')
