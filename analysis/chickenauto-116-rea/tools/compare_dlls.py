"""Compare every byte of the 116 hook with a statically UPX-decompressed 107 copy."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import subprocess
import zipfile
import dnfile
import pefile


def sha(data):
    return hashlib.sha256(data).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--upx', default='/workspace/rea-tools/upx-5.2.1-amd64_linux/upx')
    args = parser.parse_args()
    repo = Path(__file__).resolve().parents[3]
    work = repo / '.build/chickenauto-116-rea'
    with zipfile.ZipFile(repo / 'ChickenAutoEx-new.zip') as archive:
        original = archive.read('ChickenAutoEx.exe')
    assert sha(original) == '11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573'
    pe = dnfile.dnPE(data=original)
    row = next(r for r in pe.net.mdtables.ManifestResource if str(r.Name) == 'TinhKiemAuto.EasyHook.dll')
    rva = pe.net.struct.ResourcesRva + row.Offset
    size = struct.unpack('<I', pe.get_data(rva, 4))[0]
    packed = pe.get_data(rva + 4, size)
    assert sha(packed) == '1017f117193687302d3817d66cb9e7914c026d78aced4a02517a5c940efe5d79'
    packed_path = work / 'EasyHook-107-packed.dll'
    if packed_path.exists(): assert packed_path.read_bytes() == packed
    else: packed_path.write_bytes(packed)
    derived = work / 'EasyHook-107-derived.dll'
    with (work / 'upx-comparison.log').open('w') as log:
        subprocess.run([args.upx, '-t', str(packed_path)], check=True, stdout=log, stderr=subprocess.STDOUT)
        if not derived.exists():
            subprocess.run([args.upx, '-d', '-o', str(derived), str(packed_path)], check=True, stdout=log, stderr=subprocess.STDOUT)
    a, b = derived.read_bytes(), (work / 'EasyHook-116.dll').read_bytes()
    assert sha(a) == '5d8542b7e4dda25d2d8a391d6e33d830f7e256cd18fbb5ef9cf199fdb17c48d0'
    assert sha(b) == 'f22b7f349f20f94327c365d47a56484092c45dba3fa6b4fa76231f4f49336cef'
    pa, pb = pefile.PE(data=a), pefile.PE(data=b)
    assert len(a) == len(b) == 583168
    differences = [{'file_offset': hex(i), '107_byte': x, '116_byte': y} for i, (x, y) in enumerate(zip(a, b)) if x != y]
    assert differences == [{'file_offset': '0x165', '107_byte': 4, '116_byte': 16}]
    normalized = bytearray(a)
    header_offset = pa.OPTIONAL_HEADER.get_field_absolute_offset('SizeOfHeaders')
    struct.pack_into('<I', normalized, header_offset, pb.OPTIONAL_HEADER.SizeOfHeaders)
    assert bytes(normalized) == b
    assert packed_path.read_bytes() == packed
    sections = []
    for sa, sb in zip(pa.sections, pb.sections):
        assert sa.Name == sb.Name and sa.get_data() == sb.get_data()
        assert sa.VirtualAddress == sb.VirtualAddress and sa.Misc_VirtualSize == sb.Misc_VirtualSize
        sections.append({'name': sa.Name.rstrip(b'\0').decode(), 'rva': hex(sa.VirtualAddress),
                         'raw_size': sa.SizeOfRawData, 'sha256': sha(sa.get_data()), 'identical': True})
    assert len(sections) == len(pa.sections) == len(pb.sections) == 7
    result = {'107_packed_sha256': sha(packed), '107_derived_sha256': sha(a), '116_sha256': sha(b),
              'equal_size': len(a), 'differences': differences, 'sections': sections,
              'size_of_headers': {'field_file_offset': hex(header_offset), '107': pa.OPTIONAL_HEADER.SizeOfHeaders,
                                  '116': pb.OPTIONAL_HEADER.SizeOfHeaders},
              'complete_files_equal_after_in_memory_header_normalization': True,
              'targets_executed': False, 'runtime_loader_equivalence_proven': False}
    (work / 'verified-dll-comparison.json').write_text(json.dumps(result, indent=2) + '\n')
    print('PASS: all 583168 bytes compared; only one SizeOfHeaders byte differs; seven sections identical.')
    print('Header normalization was in RAM only; no original or 116 DLL modified, no target executed.')


if __name__ == '__main__':
    main()
