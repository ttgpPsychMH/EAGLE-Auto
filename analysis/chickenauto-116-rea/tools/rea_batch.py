"""Owned read-only REA MCP batch; export full evidence rather than truncating responses."""
import argparse
import json
from pathlib import Path
import subprocess


ALLOWED = {'open_binary','close_binary','procedure_pseudo_code','procedure_assembly','procedure_callees',
           'procedure_callers','procedure_info','procedure_references','xrefs','search_procedures',
           'search_strings','list_names','list_procedures','resolve_containing_procedure','read_bytes',
           'get_evidence_bundle','export_evidence_bundle'}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--requests', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--rea-command', default='rea')
    args = parser.parse_args()
    root = args.output.resolve()
    # A fresh directory keeps evidence from failed/incomplete attempts distinct.
    root.mkdir(parents=True, exist_ok=False)
    requests = json.loads(args.requests.read_text())
    assert all(c['tool'] in ALLOWED for c in requests)
    with (root / 'mcp.stderr').open('w') as err:
        child = subprocess.Popen([args.rea_command, '--mcp'], stdin=subprocess.PIPE, stdout=subprocess.PIPE,
                                 stderr=err, text=True)
        counter = 0

        def rpc(method, parameters=None):
            nonlocal counter
            counter += 1
            child.stdin.write(json.dumps({'jsonrpc':'2.0','id':counter,'method':method,'params':parameters or {}}) + '\n')
            child.stdin.flush()
            while True:
                line = child.stdout.readline()
                if not line: raise RuntimeError('Owned REA process closed before response')
                obj = json.loads(line)
                if obj.get('id') == counter:
                    if 'error' in obj: raise RuntimeError(obj['error'])
                    return obj['result']

        failures = []
        try:
            rpc('initialize', {'protocolVersion':'2025-06-18','capabilities':{},'clientInfo':{'name':'eagle-static-analysis','version':'1.0'}})
            child.stdin.write(json.dumps({'jsonrpc':'2.0','method':'notifications/initialized'}) + '\n')
            child.stdin.flush()
            for command in requests:
                name, options = command['tool'], command.get('args', {}).copy()
                # Export destinations belong to this fresh batch; do not overwrite user evidence.
                if name == 'export_evidence_bundle': options['path'] = str(root / 'full-evidence.json')
                filename = command['out']
                assert Path(filename).name == filename and filename.endswith('.json')
                response = rpc('tools/call', {'name':name,'arguments':options})
                (root / filename).write_text(json.dumps(response, indent=2) + '\n')
                print(name, filename, 'error' if response.get('isError') else 'ok', flush=True)
                if response.get('isError'):
                    detail = json.loads(next(c['text'] for c in response['content'] if c.get('type') == 'text'))
                    code = detail.get('error', {}).get('code')
                    failures.append({'tool':name,'code':code})
                    # The session remains valid after this transport budget error; retain/export complete evidence.
                    if code != 'resource_constraint': break
            result = rpc('tools/call', {'name':'export_evidence_bundle','arguments':{'path':str(root / 'retained-evidence.json')}})
            (root / 'retained-export.json').write_text(json.dumps(result, indent=2) + '\n')
        finally:
            try:
                close = rpc('tools/call', {'name':'close_binary','arguments':{'snapshot_path':str(root / 'snapshot.json')}})
                (root / 'close.json').write_text(json.dumps(close, indent=2) + '\n')
            finally:
                child.stdin.close()
                try: child.wait(timeout=45)
                except subprocess.TimeoutExpired:
                    child.terminate(); child.wait(timeout=15)
        if any(f['code'] != 'resource_constraint' for f in failures):
            raise RuntimeError('Static batch contains incomplete operations; inspect saved errors')


if __name__ == '__main__':
    main()
