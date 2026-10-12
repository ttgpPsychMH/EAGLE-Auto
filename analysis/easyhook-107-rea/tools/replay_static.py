"""Replay selected REA CLI observations without loading/executing a target.
Run with the investigation venv Python (dnfile dependency), after tool activation.
"""
import hashlib
import json
from pathlib import Path
import struct
import subprocess
import tempfile
import zipfile
import dnfile

repo=Path(__file__).resolve().parents[3]
root=repo/'.build/easyhook-107-rea'
root.mkdir(parents=True,exist_ok=True)
work=Path(tempfile.mkdtemp(prefix='replay-',dir=root))
with zipfile.ZipFile(repo/'ChickenAutoEx-new.zip') as archive:
    payload=archive.read('ChickenAutoEx.exe')
assert hashlib.sha256(payload).hexdigest()=='11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573'
pe=dnfile.dnPE(data=payload)
row=next(r for r in pe.net.mdtables.ManifestResource if str(r.Name)=='TinhKiemAuto.EasyHook.dll')
rva=pe.net.struct.ResourcesRva+row.Offset
size=struct.unpack('<I',pe.get_data(rva,4))[0]
original=pe.get_data(rva+4,size)
assert hashlib.sha256(original).hexdigest()=='1017f117193687302d3817d66cb9e7914c026d78aced4a02517a5c940efe5d79'
packed=work/'EasyHook-107.dll';packed.write_bytes(original)
derived=work/'EasyHook-107-unpacked.dll'
upx='/workspace/rea-tools/upx-5.2.1-amd64_linux/upx'
with (work/'upx.log').open('w') as stream:
    for args in [['-t',str(packed)],['-d','-o',str(derived),str(packed)]]:
        subprocess.run([upx]+args,check=True,stdout=stream,stderr=subprocess.STDOUT)
assert hashlib.sha256(derived.read_bytes()).hexdigest()=='5d8542b7e4dda25d2d8a391d6e33d830f7e256cd18fbb5ef9cf199fdb17c48d0'
rea='/workspace/rea-tools/runtime/node_modules/.bin/rea'
queries=[('overview','analyze',None),('GetMSG','decompile','0x10004110'),('SetHook','decompile','0x10005630'),
         ('UnHook','decompile','0x10005890'),('recv-wrapper','decompile','0x100052c0'),('recv-wrapper-asm','instructions','0x100052c0')]
for name,command,address in queries:
    args=[rea,command,str(derived)]
    if address:args.append(address)
    args+=['--provider','ghidra','--format','json']
    with (work/(name+'.json')).open('w') as out,(work/(name+'.stderr')).open('w') as err:
        subprocess.run(args,check=True,stdout=out,stderr=err,timeout=420)
    result=json.loads((work/(name+'.json')).read_text())
    assert not result.get('error') and 'evidence_id' in result,name
    assert result['subject']['digest']['sha256']=='5d8542b7e4dda25d2d8a391d6e33d830f7e256cd18fbb5ef9cf199fdb17c48d0'
    print(name,result['evidence_id'],flush=True)
assert hashlib.sha256(packed.read_bytes()).hexdigest()=='1017f117193687302d3817d66cb9e7914c026d78aced4a02517a5c940efe5d79'
print('Static replay complete; raw observations kept private in',work)
