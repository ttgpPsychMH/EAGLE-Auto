"""Read the single product version source used by MSBuild and packaging."""
import argparse
from pathlib import Path
import re
import xml.etree.ElementTree as ET


def product_version(repo, override=None):
    version = override if override is not None else ET.parse(repo / "EagleAuto.Version.props").findtext("./PropertyGroup/EagleAutoVersion")
    if not version or not re.fullmatch(r"(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)", version) or any(int(part) > 65534 for part in version.split(".")):
        raise ValueError("Product version must be major.minor, each 0..65534, without leading zeroes")
    tag = "v" + version
    executable = "EAGLE-Auto-" + tag + ".exe"
    return {"version": version, "display_version": tag, "file_version": version + ".0.0",
            "executable": executable, "config": executable + ".config",
            "zip_name": "EAGLE-Auto-" + tag + "-net48.zip",
            "artifact_name": "EAGLE-Auto-" + tag + "-net48-review"}


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--github-output", type=Path)
    args = parser.parse_args()
    values = product_version(args.repo.resolve())
    lines = "".join(key + "=" + value + "\n" for key, value in values.items())
    if args.github_output:
        with args.github_output.open("a", encoding="utf-8") as output:
            output.write(lines)
    print(lines, end="")
