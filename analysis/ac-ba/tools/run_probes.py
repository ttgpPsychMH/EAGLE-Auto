"""Compile selected managed methods with fakes; never execute the application/native DLL."""
from pathlib import Path
import hashlib
import json
import subprocess
import sys

repo = Path(__file__).resolve().parents[3]
inventory = json.loads((repo / "analysis/ac-ba/evidence/source-inventory.json").read_text())
for relative, expected in inventory["input_sha256"].items():
    assert hashlib.sha256((repo / relative).read_bytes()).hexdigest() == expected, "Changed analysis baseline: " + relative
sys.path.insert(0, str(repo / "tools"))
from thuy_lao_review import apply_review as thuy
from ky_cuoc_review import apply_review as ky
from trung_ac_review import apply_review as trung
from ac_tac_review import apply_review as ac_tac

source = (repo / "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs").read_text()
path = repo / ".build/ac-ba/probe"
path.mkdir(parents=True, exist_ok=True)
(path.parent / "ReviewedGame.cs").write_text(ac_tac(repo, trung(repo, ky(repo, thuy(repo, source)))))
(path / "Probe.csproj").write_text('''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework></PropertyGroup>
  <ItemGroup>
    <Compile Include="../../../analysis/ac-ba/tools/Probe.cs" Link="Program.cs" />
    <Reference Include="Microsoft.CodeAnalysis" HintPath="$(MSBuildSDKsPath)/../Roslyn/bincore/Microsoft.CodeAnalysis.dll" />
    <Reference Include="Microsoft.CodeAnalysis.CSharp" HintPath="$(MSBuildSDKsPath)/../Roslyn/bincore/Microsoft.CodeAnalysis.CSharp.dll" />
  </ItemGroup>
</Project>
''')
subprocess.run(["dotnet", "run", "--project", str(path / "Probe.csproj"), "-c", "Release", "--", str(repo)],
               cwd=repo, check=True)
