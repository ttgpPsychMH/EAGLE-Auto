"""Project selected REA static evidence; never publish raw callback/auth/Lua source."""
import hashlib
import json
from pathlib import Path
import subprocess

repo = Path(__file__).resolve().parents[3]
private = repo / '.build/easyhook-107-rea'
output = repo / 'analysis/easyhook-107-rea/evidence'
output.mkdir(parents=True, exist_ok=True)

def digest(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

def evidence(name):
    path = private / (name + '.json')
    outer = json.loads(path.read_text())
    value = outer.get('structuredContent', outer)
    assert not outer.get('isError') and 'evidence_id' in value, name
    return path, value

identity = json.loads((private / 'identity.json').read_text())
assert digest(private / 'EasyHook-107.dll') == identity['dll_sha256']
assert digest(private / 'EasyHook-107-unpacked.dll') == '5d8542b7e4dda25d2d8a391d6e33d830f7e256cd18fbb5ef9cf199fdb17c48d0'
index = {'packed_input':identity, 'derived_artifact':{'transformation':'UPX 5.2.1 -d to a separate output; no target execution',
         'size':(private/'EasyHook-107-unpacked.dll').stat().st_size, 'sha256':digest(private/'EasyHook-107-unpacked.dll')},
         'evidence_kind':'Selected projections from original canonical REA evidence; excerpts are NOT canonical Evidence objects',
         'records':{}, 'target_executed':False}
selected = ['GetMSG-unpacked','SetHook-unpacked','UnHook-unpacked','GetMSG-assembly','SetHook-assembly','UnHook-assembly',
            'recv-wrapper','recv-wrapper-assembly','recv-install','recv-install-assembly','registered-message-init-bytes',
            'recv-iat-xrefs','sendmessage-iat-xrefs','message-id-xrefs','recv-install-callees','recv-wrapper-callees']
for name in selected:
    source, value = evidence(name)
    assert value['subject']['digest']['sha256'] == index['derived_artifact']['sha256'], name
    filename = name + ('.txt' if isinstance(value['normalized_result'],str) else '.json')
    header = {'projection_formatting':'Trailing line whitespace and final blank lines removed; leading indentation preserved', 'source_file':source.name, 'private_raw_response_sha256':digest(source), 'rea_evidence_id':value['evidence_id'],
              'artifact_sha256':value['subject']['digest']['sha256'], 'provider':value['provider'], 'operation':value['operation'],
              'parameters':value['parameters'], 'limitations':value['limitations'], 'projection_file':filename}
    index['records'][name] = header
    normalized = value['normalized_result']
    if isinstance(normalized,str):
        (output/filename).write_text('Selected static observation; not original source or runtime proof.\n'
                                  +'REA evidence: '+value['evidence_id']+'\nArtifact SHA256: '+index['derived_artifact']['sha256']+'\n\n'+normalized+'\n')
    else:
        (output/filename).write_text(json.dumps({'projection_notice':index['evidence_kind'],'source_evidence':header,'observation':normalized},indent=2)+'\n')
# Only the notification setup branches and callback entry/exit; unrelated Lua/auth commands stay private.
for name in ['message-callback','message-callback-assembly']:
    source, value = evidence(name)
    text = value['normalized_result']
    if name == 'message-callback':
        excerpt = text[:text.index('    if (*(int *)(param_5 + 0xc) == -4)')]
        excerpt += '\n  /* ... unrelated dispatch branches omitted; this is an excerpt ... */\n'
        excerpt += text[text.rfind('    __security_check_cookie'):]
    else:
        lines=text.splitlines();excerpt='\n'.join(lines[:46])+'\n... unrelated dispatch omitted ...\n'+'\n'.join(lines[-23:])+'\n'
    filename=name+'-excerpt.txt'
    (output/filename).write_text('EXCERPT ONLY: canonical full response stays private; do not treat this as complete control flow.\n'
                               +'REA evidence: '+value['evidence_id']+'\nCanonical response SHA256: '+digest(source)+'\n\n'+excerpt)
    index['records'][name]={'rea_evidence_id':value['evidence_id'],'private_raw_response_sha256':digest(source),
        'operation':value['operation'],'projection_file':filename,'extent':'Selected setup and return snippets; unrelated Lua/auth dispatch omitted',
        'projection_formatting':'Trailing line whitespace and final blank lines removed; leading indentation preserved',
        'limitations':value['limitations']}
for name in ['overview','unpacked-overview']:
    source, value=evidence(name)
    index[name]={'source_response_sha256':digest(source),'rea_evidence_id':value['evidence_id'],'subject':value['subject'],
                 'observation':value['normalized_result'],'analysis_profile':value['analysis_profile'],'limitations':value['limitations']}
# Cross-check Ghidra's observed init bytes against independently extracted file bytes.
from_source = json.loads((output/'registered-message-init-bytes.json').read_text())['observation']['bytes_hex']
assert from_source == '6870020710ff1588010710a370250a10c3'
assembly=subprocess.run(['objdump','-d','--start-address=0x10001020','--stop-address=0x10001031',str(private/'EasyHook-107-unpacked.dll')],check=True,stdout=subprocess.PIPE,text=True).stdout
(output/'registered-message-init-objdump.txt').write_text('Independent objdump static decoding; Ghidra did not define a function at this address.\n'+assembly)
assert '0x10070188' in assembly and '0x100a2570' in assembly
(output/'inventory.json').write_text(json.dumps(index,ensure_ascii=False,indent=2)+'\n')
# Provider acceptance, not full machine/client readiness.
doc=json.loads(Path('/workspace/rea-tools/doctor.json').read_text())
lock=json.loads(Path('/workspace/rea-tools/runtime/package-lock.json').read_text())['packages']['node_modules/rea-agents']
toolchain={'rea':{k:lock[k] for k in ['version','resolved','integrity']},'ghidra':'12.1.4','upx':'5.2.1',
 'node':'24.19.0','jdk':'Temurin 21.0.12.1+1','host':'Debian 13 / Linux x64',
 'scoped_ghidra_healthy':doc['healthy'],'full_environment_healthy':doc['environment_healthy'],
 'scope_checks':doc['scope_checks'],'full_host_advertised_support':False,
 'mcp_mode':'Owned local stdio client; no persistent Codex registration was written',
 'archives_sha256':{'ghidra_12.1.4_PUBLIC_20260921.zip':'ddac49f903da9d5bac833e5cc79395098b9c33cfd3279be5f31bd00387d2d4db',
 'OpenJDK21U-jdk_x64_linux_hotspot_21.0.12.1_1.tar.gz':'ce79869e1307ed8ee1e2baa86a412b1eb5b75d10a01006d788a6f968bcfaee94',
 'upx-5.2.1-amd64_linux.tar.xz':'402162aad30af47e60dbd767fb2e64ca394ace9727ba1f40283641f1d1b91657'}}
assert toolchain['scoped_ghidra_healthy']
(output/'toolchain.json').write_text(json.dumps(toolchain,indent=2)+'\n')
for path in output.glob('*.txt'):
    path.write_text('\n'.join(line.rstrip() for line in path.read_text().splitlines()).rstrip()+'\n')
print('Projected selected REA evidence; no raw callbacks, binaries, credentials or source reconstruction bundle published.')
