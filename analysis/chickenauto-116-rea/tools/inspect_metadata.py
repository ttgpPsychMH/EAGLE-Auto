"""Read version metadata and selected MSVC RTTI/vtables as data, without loading a PE."""
import hashlib
import json
from pathlib import Path
import re
import struct
import pefile


def main():
    repo = Path(__file__).resolve().parents[3]
    work = repo / '.build/chickenauto-116-rea'
    data = (work / 'ChickenAuto-116.exe').read_bytes()
    assert hashlib.sha256(data).hexdigest() == '34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea'
    pe = pefile.PE(data=data)
    base = pe.OPTIONAL_HEADER.ImageBase
    text = next(s for s in pe.sections if s.Name.rstrip(b'\0') == b'.text')
    rdata = next(s for s in pe.sections if s.Name.rstrip(b'\0') == b'.rdata')
    start = rdata.PointerToRawData
    end = start + rdata.SizeOfRawData
    selected = {'AutoAttack@game_auto_controllers@game_controllers', 'AutoControllerAbstract@game_auto_controllers@game_controllers',
                'AutoPickLootPackage@game_auto_controllers@game_controllers', 'MainWindow', 'WorkerDetectGameInstance',
                'WorkerUpdateUi', 'QuestSettings@setting_tabs'}
    vtables = []
    def va(offset): return base + pe.get_rva_from_offset(offset)
    for match in re.finditer(rb'\.\?AV([A-Za-z0-9_?@$]{1,220})@@\0', data):
        name = match.group(1).decode()
        if name not in selected: continue
        descriptor = va(match.start() - 8)
        tables = []
        for reference in re.finditer(re.escape(struct.pack('<I', descriptor)), data[start:end]):
            offset = start + reference.start() - 12
            if offset < start: continue
            signature, object_offset, ctor_offset, typ, hierarchy = struct.unpack_from('<5I', data, offset)
            if signature != 0 or ctor_offset != 0 or hierarchy < base: continue
            try: pe.get_offset_from_rva(hierarchy - base)
            except pefile.PEFormatError: continue
            for entry in re.finditer(re.escape(struct.pack('<I', va(offset))), data[start:end]):
                table = start + entry.start() + 4
                entries = []
                for index in range(128):
                    value = struct.unpack_from('<I', data, table + index * 4)[0]
                    if not base + text.VirtualAddress <= value < base + text.VirtualAddress + text.Misc_VirtualSize: break
                    entries.append(hex(value))
                if entries:
                    tables.append({'col_va':hex(va(offset)), 'object_offset':object_offset, 'vtable_va':hex(va(table)),
                                   'entries':entries, 'examined_limit':128, 'stopped_on_non_text_pointer':len(entries)<128})
        vtables.append({'class':name,'type_descriptor_va':hex(descriptor),'vtables':tables})
    assert len(vtables) == 7 and all(c['vtables'] for c in vtables)
    (work / 'selected-vtables.json').write_text(json.dumps(vtables,indent=2)+'\n')
    versions = {}
    for name in ['ChickenAuto-116.exe','EasyHook-116.dll']:
        p = pefile.PE(str(work / name));p.parse_data_directories(directories=[2]);strings={}
        for block in getattr(p,'FileInfo',[]):
            for item in block:
                for table in getattr(item,'StringTable',[]):
                    strings.update({k.decode():v.decode() for k,v in table.entries.items()
                                    if k in [b'FileVersion',b'ProductVersion',b'ProductName',b'FileDescription',b'OriginalFilename']})
        versions[name]={'file_version':strings,'linker':[p.OPTIONAL_HEADER.MajorLinkerVersion,p.OPTIONAL_HEADER.MinorLinkerVersion],
                        'timestamp':p.FILE_HEADER.TimeDateStamp,'debug_entries':[{'type':x.struct.Type,'size':x.struct.SizeOfData}
                         for x in getattr(p,'DIRECTORY_ENTRY_DEBUG',[])]}
    (work / 'version-metadata.json').write_text(json.dumps(versions,indent=2)+'\n')
    print('PASS: seven selected RTTI/vtable identities and PE version metadata recovered statically.')


if __name__ == '__main__':
    main()
