"""Offline reconstruction of observed notification byte layout; never execute DLL code."""
import hashlib
import json
from pathlib import Path
import struct

root = Path(__file__).resolve().parents[1]
assembly = (root/'evidence/recv-wrapper-assembly.txt').read_text()
for anchor in ['CMP EDX, 0xf','CMP EDX, 0x3e8','LEA EBX, [EDX + 0x4]',
               'MOV dword ptr [EBP + -0x8], EDX','MOV dword ptr [EBP + -0x4], 0x10089cc0','RET 0x10']:
    assert anchor in assembly, anchor

# Synthetic data only. Model = prefix + copied chunk, cbData = n, exactly as observed assembly.
def forward(pid, received, destination=True):
    n=len(received)
    if n<=15 or n>=1000 or not destination:return None
    scratch=struct.pack('<I',pid)+received
    return scratch[:n]

results=[]
def check(name, condition):
    assert condition, name
    results.append({'name':name,'passed':True})

pid=0x12345678
for n in [0,1,4,15,1000,1001,4096]:
    check('no notification for recv length '+str(n),forward(pid,b'a'*n) is None)
for n in [16,20,100,999]:
    data=bytes(i%256 for i in range(n));message=forward(pid,data)
    check('cbData equals n='+str(n),len(message)==n)
    check('PID prefix little endian n='+str(n),message[:4]==struct.pack('<I',pid))
    check('last four raw bytes absent n='+str(n),message[4:]==data[:-4])
check('missing destination suppresses message',forward(pid,b'a'*20,False) is None)
example=b'abcdefghijklmnopqrst';wire=forward(pid,example)
check('twenty-byte chunk becomes sixteen-byte payload',wire[4:]==b'abcdefghijklmnop')
# These show possible stream consequences, not real private-client packet semantics.
check('split short chunks are both lost',forward(pid,b'a'*10) is None and forward(pid,b'b'*10) is None)
check('coalesced large chunk is lost',forward(pid,b'a'*600+b'b'*600) is None)
check('UTF8 suffix can end in incomplete codepoint', forward(pid,b'a'*16+'Sơnxx'.encode('utf-8'))[4:].endswith(b'\xc6'))
summary={'checks_passed':len(results),'failed':0,'scope':'Pure synthetic byte-layout model bound to observed assembly anchors; NOT DLL/Windows/game execution',
         'assembly_projection_sha256':hashlib.sha256((root/'evidence/recv-wrapper-assembly.txt').read_bytes()).hexdigest(),
         'example':{'received_ascii':example.decode(),'forwarded_ascii':wire[4:].decode(),'omitted_ascii':example[-4:].decode()},
         'results':results,'target_executed':False}
(root/'evidence/byte-layout-checks.json').write_text(json.dumps(summary,indent=2)+'\n')
print(str(len(results))+' offline byte-layout checks passed; native/runtime behavior remains untested.')
