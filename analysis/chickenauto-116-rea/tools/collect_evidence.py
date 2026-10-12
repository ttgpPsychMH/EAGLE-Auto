"""Project selected static REA observations; full native source/evidence stays ignored."""
import hashlib
import json
from pathlib import Path
import shutil


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    repo = Path(__file__).resolve().parents[3]
    work = repo / '.build/chickenauto-116-rea'
    output = repo / 'analysis/chickenauto-116-rea/evidence'
    output.mkdir(parents=True, exist_ok=True)
    manifest = {'projection_notice': 'Selected review projections are not canonical Evidence. Raw responses and complete native source remain ignored.',
                'targets': {'app': '34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea',
                            'hook': 'f22b7f349f20f94327c365d47a56484092c45dba3fa6b4fa76231f4f49336cef'}, 'observations': []}
    specs = [
        ('hook','GetMSG'),('hook','SetHook'),('hook','SetHook-asm'),('hook','UnHook'),('hook','UnHook-asm'),
        ('hook','recv-wrapper'),('hook','recv-wrapper-asm'),('hook','recv-install'),
        ('app-focused','post-hook-command'),('app-focused','post-hook-command-asm'),
        ('app-focused','initialize-game-hook'),('app-focused','initialize-game-hook-asm'),
        ('app-focused','main-native-event'),('app-focused','main-native-event-asm'),
        ('app-focused','native-message-handler'),('app-focused','native-message-handler-asm'),
        ('app-focused','empty-metacall'),('app-focused','basic-settings-metacall'),
    ]
    for directory, name in specs:
        path = work / directory / (name + '.json')
        response = json.loads(path.read_text())
        assert not response.get('isError'), path.name
        v = response['structuredContent']
        digest = v['subject']['digest']['sha256']
        assert digest == manifest['targets']['hook' if directory == 'hook' else 'app']
        record = {'target': 'hook' if directory == 'hook' else 'app', 'name': name,
                  'evidence_id': v['evidence_id'], 'raw_response_sha256': sha(path),
                  'operation': v['operation'], 'parameters': v['parameters'],
                  'subject': v['subject'], 'analysis_profile': v['analysis_profile'],
                  'confidence': v['confidence'], 'limitations': v['limitations']}
        text = v['normalized_result']
        assert isinstance(text, str), name
        # Normalize trailing whitespace in the projection only; preserve the raw response hash.
        text = '\n'.join(line.rstrip() for line in text.strip().splitlines()) + '\n'
        dst = output / (record['target'] + '-' + name + '.txt')
        header = 'Source Evidence: ' + v['evidence_id'] + '\nTarget SHA256: ' + digest + '\nProjection: normalized trailing whitespace; inferred pseudocode types are not original declarations.\n\n'
        dst.write_text(header + text)
        record['projection_file'] = dst.name
        record['projection_sha256'] = sha(dst)
        manifest['observations'].append(record)
    # Full constructors/controllers/workers can include unrelated constants. Publish a call map and reviewed excerpts separately.
    for directory, name in [('app-focused','auto-attack'),('app-focused','auto-loot'),('app-focused','worker-detect-run'),('app-focused','worker-ui-run'),('app-focused','configure-hook'),('app-focused','load-hook-library')]:
        path = work / directory / (name + '.json');v=json.loads(path.read_text())['structuredContent']
        assert v['subject']['digest']['sha256'] == manifest['targets']['app']
        manifest['observations'].append({'target':'app','name':name,'evidence_id':v['evidence_id'],'raw_response_sha256':sha(path),'operation':v['operation'],'parameters':v['parameters'],'analysis_profile':v['analysis_profile'],'confidence':v['confidence'],'limitations':v['limitations'],'full_source_published':False})
    for filename in ['input-identity.json','verified-dll-comparison.json','selected-vtables.json','version-metadata.json','preservation.json','command-immediate-scan.json']:
        shutil.copyfile(work / filename, output / filename)
    (output / 'inventory.json').write_text(json.dumps(manifest, indent=2) + '\n')
    print('Selected static evidence projected; constructors, controllers, complete inventories and provider snapshots remain private.')


if __name__ == '__main__':
    main()
