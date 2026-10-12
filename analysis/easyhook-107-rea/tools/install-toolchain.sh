#!/usr/bin/env bash
set -euo pipefail
rea_tools_root=/workspace/rea-tools
mkdir -p "$rea_tools_root/downloads" "$rea_tools_root/tmp"
node --version
npm --version
if [ -f "$rea_tools_root/runtime/package-lock.json" ]; then
  npm ci --prefix "$rea_tools_root/runtime" --cache "$rea_tools_root/npm-cache" --ignore-scripts --no-audit --no-fund
else
  npm install --prefix "$rea_tools_root/runtime" --cache "$rea_tools_root/npm-cache" --ignore-scripts --no-audit --no-fund rea-agents@6.4.0
fi
python - <<'PY'
from pathlib import Path
import hashlib,subprocess,tarfile,zipfile
root=Path('/workspace/rea-tools')
assets=[
 ('ghidra_12.1.4_PUBLIC_20260921.zip','https://github.com/NationalSecurityAgency/ghidra/releases/download/Ghidra_12.1.4_build/ghidra_12.1.4_PUBLIC_20260921.zip','ddac49f903da9d5bac833e5cc79395098b9c33cfd3279be5f31bd00387d2d4db','ghidra_12.1.4_PUBLIC'),
 ('OpenJDK21U-jdk_x64_linux_hotspot_21.0.12.1_1.tar.gz','https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.12.1%2B1/OpenJDK21U-jdk_x64_linux_hotspot_21.0.12.1_1.tar.gz','ce79869e1307ed8ee1e2baa86a412b1eb5b75d10a01006d788a6f968bcfaee94','jdk-21.0.12.1+1'),
 ('upx-5.2.1-amd64_linux.tar.xz','https://github.com/upx/upx/releases/download/v5.2.1/upx-5.2.1-amd64_linux.tar.xz','402162aad30af47e60dbd767fb2e64ca394ace9727ba1f40283641f1d1b91657','upx-5.2.1-amd64_linux')]
for filename,url,expected,directory in assets:
 path=root/'downloads'/filename
 if not path.exists():subprocess.run(['curl','--fail','--silent','--show-error','--location','--max-time','240',url,'-o',str(path)],check=True)
 with path.open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==expected,filename+' checksum mismatch'
 if not (root/directory).is_dir():
  if filename.endswith('.zip'):
   with zipfile.ZipFile(path) as z:
    assert all(not Path(n).is_absolute() and '..' not in Path(n).parts for n in z.namelist())
    z.extractall(root)
  else:
   with tarfile.open(path) as a:a.extractall(root,filter='data')
for relative in ['ghidra_12.1.4_PUBLIC/support/analyzeHeadless','ghidra_12.1.4_PUBLIC/support/launch.sh','ghidra_12.1.4_PUBLIC/Ghidra/Features/Decompiler/os/linux_x86_64/decompile']:
 p=root/relative;p.chmod(p.stat().st_mode|0o111)
activation='''export GHIDRA_INSTALL_DIR=/workspace/rea-tools/ghidra_12.1.4_PUBLIC
export JAVA_HOME=/workspace/rea-tools/jdk-21.0.12.1+1
export REA_ANALYSIS_PROVIDER=ghidra
export REA_GHIDRA_STARTUP_TIMEOUT_MS=330000
export TMPDIR=/workspace/rea-tools/tmp
export PATH=/workspace/rea-tools/runtime/node_modules/.bin:$PATH
'''
(root/'activate.sh').write_text(activation)
PY
source "$rea_tools_root/activate.sh"
test "$(rea --version)" = 6.4.0
rea doctor --provider ghidra --format json > "$rea_tools_root/doctor.json"
python - <<'PY'
from pathlib import Path
import json
v=json.loads(Path('/workspace/rea-tools/doctor.json').read_text());assert v['healthy'],v['scope_checks']
print('REA/Ghidra scope ready; full-host support and optional integrations are separate diagnostics.')
PY
