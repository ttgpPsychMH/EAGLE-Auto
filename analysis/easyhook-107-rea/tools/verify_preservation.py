"""Confirm the investigation did not change app/auth/source snapshots or releases."""
import hashlib
import json
from pathlib import Path
import subprocess

repo=Path(__file__).resolve().parents[3]
base='1ff6bd54d28037a8101eee43fa14b5cfe2ec90fe'
protected=['src','tests','tools','.github','.config','EagleAuto.Version.props','ChickenAutoEx.sln','global.json',
           'analysis/chickenautoex-107','analysis/chickenauto-116-vs-107','analysis/ac-ba']
changed=subprocess.run(['git','diff','--name-only',base,'--']+protected,cwd=repo,check=True,stdout=subprocess.PIPE,text=True).stdout
assert not changed,'Protected source/config changed'
inventory=json.loads((repo/'analysis/chickenautoex-107/evidence/inventory.json').read_text())
files={}
for name,item in inventory['original_files'].items():
    with (repo/name).open('rb') as stream:actual=hashlib.file_digest(stream,'sha256').hexdigest()
    assert actual==item['sha256'],name
    files[name]=actual
close=json.loads((repo/'.build/easyhook-107-rea/mcp-close.json').read_text());assert not close.get('isError')
summary={'base_commit':base,'protected_source_paths':protected,'protected_sources_unchanged':True,
         'original_release_hashes':files,'original_release_files_unchanged':True,'licensing_vip_source_unchanged':True,
         'application_version_unchanged':'0.6','mcp_close_reported_error':False,
         'target_dll_or_application_executed':False,'windows_game_tested':False,
         'execution_scope':'REA/Ghidra/UPX tool processes and pure synthetic byte-layout Python; target read as data only'}
(repo/'analysis/easyhook-107-rea/evidence/preservation.json').write_text(json.dumps(summary,indent=2)+'\n')
print('PASS: original releases, application/tests/build config and recovered/auth snapshots unchanged.')
