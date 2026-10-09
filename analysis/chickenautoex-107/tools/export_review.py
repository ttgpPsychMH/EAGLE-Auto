"""Export a textual, redacted review snapshot, not a repaired application."""
import argparse
import hashlib
import json
from pathlib import Path
import re


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--input", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    source, dest = args.input.resolve(), args.output.resolve()
    if dest == source or source in dest.parents:
        raise ValueError("Keep the raw analysis and review snapshot separate")
    if dest.exists() and any(dest.iterdir()):
        raise ValueError("Refusing to overwrite an existing snapshot")
    dest.mkdir(parents=True, exist_ok=True)
    literal = r'"(?:\\.|[^"\\])*"'
    patterns = {
        "TinhKiemAuto/Debug.cs": (r'(dictionary\.Add\("key", )(' + literal + r')', 1),
        "TinhKiemAuto.Models/LoadFile.cs": (r'(string password = )(' + literal + r')', 2),
        "TinhKiemAuto.Models/TienIch.cs": (r'(return GetMD5\()(' + literal + r')', 1),
        "TinhKiemAuto/Game.cs": (r'(TINHKIEM\.Hasher\.Decrypt\()(' + literal + r', ' + literal + r')', 1),
    }
    redactions, omitted, files, sensitive = [], [], [], []
    # Collect values first: an encryption key may also appear in a text resource.
    for relative, (pattern, expected) in patterns.items():
        text = (source / relative).read_text(encoding="utf-8-sig")
        matches = list(re.finditer(pattern, text))
        if len(matches) != expected:
            raise ValueError("Unexpected sensitive-literal layout: " + relative)
        for match in matches:
            sensitive.extend(value[1:-1] for value in re.findall(literal, match.group(2)))
    for path in sorted(source.rglob("*")):
        if not path.is_file():
            continue
        relative = path.relative_to(source).as_posix()
        if path.suffix not in (".cs", ".csproj", ".resx", ".json", ".xml", ".txt", ".manifest"):
            omitted.append({"path": relative, "size": path.stat().st_size,
                            "sha256": hashlib.sha256(path.read_bytes()).hexdigest()})
            continue
        text = path.read_text(encoding="utf-8-sig")
        if relative in patterns:
            pattern, expected = patterns[relative]
            matches = list(re.finditer(pattern, text))
            if len(matches) != expected:
                raise ValueError("Unexpected sensitive-literal layout: " + relative)
            for match in matches:
                redactions.append({"path": relative, "line": text[:match.start()].count("\n") + 1,
                                   "reason": "credential/key material or encrypted embedded payload omitted"})
            def replace(match):
                count = len(re.findall(literal, match.group(2)))
                return match.group(1) + ', '.join(['"[REDACTED]"'] * count) + " /* analysis redaction */"
            text = re.sub(pattern, replace, text)
        for value in set(sensitive):
            while value in text:
                offset = text.index(value)
                redactions.append({"path": relative, "line": text[:offset].count("\n") + 1,
                                   "reason": "additional occurrence of omitted key material in text"})
                text = text[:offset] + "[REDACTED]" + text[offset + len(value):]
        output = dest / relative
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(text, encoding="utf-8")
        files.append({"path": relative, "raw_source_sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
                      "review_sha256": hashlib.sha256(output.read_bytes()).hexdigest()})
    for path in dest.rglob("*"):
        if path.is_file():
            text = path.read_text()
            if any(value in text for value in sensitive):
                raise ValueError("Sensitive literal remains in " + str(path))
    manifest = {"redactions": redactions, "omitted_binary_resources": omitted,
                "files": files, "csharp_file_count": sum(x["path"].endswith(".cs") for x in files)}
    (dest.parent / "recovery-manifest.json").write_text(json.dumps(manifest, indent=2) + "\n")
    print("Exported", manifest["csharp_file_count"], "C# files;", len(redactions), "redacted sites.")


if __name__ == "__main__":
    main()
