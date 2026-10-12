"""Create review-only REA requests for the pinned inputs prepared in ignored .build."""
import argparse
import json
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--target', choices=['app','hook'], required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--snapshot', type=Path)
    args = parser.parse_args()
    repo = Path(__file__).resolve().parents[3]
    target = repo / '.build/chickenauto-116-rea' / ('ChickenAuto-116.exe' if args.target == 'app' else 'EasyHook-116.dll')
    opening = {'path':str(target),'provider_id':'ghidra'}
    if args.snapshot: opening['snapshot_path'] = str(args.snapshot.resolve())
    requests = [{'tool':'open_binary','args':opening,'out':'open.json'}]
    def query(tool, address, name):
        requests.append({'tool':tool,'args':{'address' if tool == 'xrefs' else 'procedure':address},'out':name+'.json'})
    if args.target == 'app':
        for address,name in [('0xc265f4','SetHook-export-name-xrefs'),('0xc265b0','GetMSG-export-name-xrefs'),('0xc246a8','PostMessageA-xrefs'),('0x426bb0','command-dispatch-xrefs'),('0x10d0d5c','UnHook-pointer-xrefs')]:
            query('xrefs',address,name)
        for address,name in [('0x4113ca','load-hook-library'),('0x426bb0','post-hook-command'),('0x421e11','configure-hook'),('0x4053f0','auto-attack'),('0x406000','auto-loot'),('0x415930','worker-detect-run'),('0x417820','worker-ui-run'),('0x43e1e0','basic-settings-metacall'),('0x408270','empty-metacall'),('0x426b12','initialize-game-hook'),('0x411aa0','main-native-event'),('0x41f800','native-message-handler')]:
            query('procedure_pseudo_code',address,name)
        for address,name in [('0x426b12','initialize-game-hook-asm'),('0x426bb0','post-hook-command-asm'),('0x411aa0','main-native-event-asm'),('0x41f800','native-message-handler-asm')]:
            query('procedure_assembly',address,name)
        query('procedure_callees','0x41f800','native-message-handler-callees')
    else:
        for address,name in [('GetMSG','GetMSG'),('SetHook','SetHook'),('UnHook','UnHook'),('0x100052c0','recv-wrapper'),('0x10002810','recv-install')]:
            query('procedure_pseudo_code',address,name)
        for address,name in [('SetHook','SetHook-asm'),('UnHook','UnHook-asm'),('0x100052c0','recv-wrapper-asm')]:
            query('procedure_assembly',address,name)
    requests.append({'tool':'list_procedures','args':{},'out':'procedures-transport.json'})
    requests.append({'tool':'get_evidence_bundle','args':{'detail':'summary'},'out':'evidence-summary.json'})
    args.output.parent.mkdir(parents=True,exist_ok=True)
    args.output.write_text(json.dumps(requests,indent=2)+'\n')
    print('Created static requests:', args.target, len(requests))


if __name__ == '__main__':
    main()
