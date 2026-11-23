"""Package an already-published Windows build, its notices and SHA-256 checksum.

Usage: python scripts/package_release.py PUBLISH_DIR PROJECT_ASSETS_JSON OUTPUT_DIR
Uses only the Python standard library; run dotnet publish before this script.
"""
from pathlib import Path
import hashlib
import json
import shutil
import sys
import urllib.request
import xml.etree.ElementTree as ET
import zipfile

root = Path(__file__).resolve().parents[1]
publish, assets_path, output = map(Path, sys.argv[1:4])
version = "0.7.0"
assets = json.loads(assets_path.read_text(encoding="utf-8-sig"))
package_roots = [Path(p) for p in assets["packageFolders"]]
licenses = publish / "licenses"
licenses.mkdir(parents=True, exist_ok=True)
mit = (root / "LICENSE").read_text(encoding="utf-8").split("Permission is hereby granted", 1)[1]
index = ["# Bundled dependency notices", "", "| Package | License |", "|---|---|"]

for identity, item in sorted(assets["libraries"].items()):
    if item.get("type") != "package":
        continue
    # Native packages for other operating systems are not in the Windows bundle.
    if any(p in identity for p in (".Linux/", ".macOS/", ".WebAssembly/")):
        continue
    package = next((r / item["path"] for r in package_roots if (r / item["path"]).exists()), None)
    if package is None:
        raise RuntimeError("Missing NuGet package: " + identity)
    metadata = ET.parse(next(package.glob("*.nuspec"))).getroot()
    fields = {e.tag.split("}")[-1]: e.text or "" for e in metadata.iter()}
    license_name = fields.get("license", fields.get("licenseUrl", "See package metadata"))
    index.append(f"| {identity} | {license_name} |")
    destination = licenses / identity.replace("/", "-")
    destination.mkdir(exist_ok=True)
    notices = [p for p in item.get("files", []) if any(term in Path(p).name.lower() for term in ("license", "notice", "copying"))]
    for name in notices:
        shutil.copy2(package / name, destination / Path(name).name)
    copyright_text = fields.get("copyright", "")
    (destination / "package-attribution.txt").write_text(
        identity + "\nAuthors: " + fields.get("authors", "") + "\n" + copyright_text + "\nLicense: " + license_name + "\n",
        encoding="utf-8",
    )
    if not notices and license_name == "MIT":
        (destination / "LICENSE.txt").write_text("MIT License\n\n" + copyright_text + "\n\nPermission is hereby granted" + mit, encoding="utf-8")
    if license_name == "Apache-2.0":
        with urllib.request.urlopen("https://www.apache.org/licenses/LICENSE-2.0.txt", timeout=30) as response:
            (destination / "LICENSE.txt").write_bytes(response.read())
        if identity.lower().startswith("log4net/"):
            (destination / "NOTICE.txt").write_text("Apache log4net\n" + copyright_text + "\nThis product includes software developed at The Apache Software Foundation (https://www.apache.org/).\n", encoding="utf-8")

config = json.loads((publish / "ModuleXmlReader.Avalonia.runtimeconfig.json").read_text(encoding="utf-8-sig"))
for framework in config["runtimeOptions"].get("includedFrameworks", []):
    name, runtime_version = framework["name"], framework["version"]
    package_name = name.lower() + ".runtime.win-x64"
    runtime = next((r / package_name / runtime_version for r in package_roots if (r / package_name / runtime_version).exists()), None)
    if runtime is None:
        raise RuntimeError("Missing runtime notices: " + name)
    destination = licenses / (name + "-" + runtime_version)
    destination.mkdir(exist_ok=True)
    for filename in ("LICENSE.TXT", "THIRD-PARTY-NOTICES.TXT"):
        shutil.copy2(runtime / filename, destination / filename)
    index.append(f"| {name}/{runtime_version} | See bundled runtime license and notices |")

(licenses / "README.md").write_text("\n".join(index) + "\n", encoding="utf-8")
for filename in ("LICENSE", "README.md", "CHANGELOG.md", "THIRD-PARTY-NOTICES.md"):
    shutil.copy2(root / filename, publish / filename)
shutil.copytree(root / "docs", publish / "docs", dirs_exist_ok=True)
samples = publish / "Samples"
samples.mkdir(exist_ok=True)
for sample in (root / "ModuleXmlReader.CharacterizationTests" / "Fixtures").glob("*.module"):
    shutil.copy2(sample, samples / sample.name)
output.mkdir(parents=True, exist_ok=True)
archive = output / f"ModuleXmlReader-{version}-win-x64.zip"
with zipfile.ZipFile(archive, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=9) as bundle:
    for path in sorted(publish.rglob("*")):
        if path.is_file():
            if path.suffix in (".log", ".pdb"):
                continue
            bundle.write(path, path.relative_to(publish))
digest = hashlib.file_digest(archive.open("rb"), "sha256").hexdigest()
(output / "SHA256SUMS.txt").write_text(f"{digest}  {archive.name}\n", encoding="ascii")
print(f"Packaged {archive.name}: {archive.stat().st_size} bytes; SHA-256 {digest}")
