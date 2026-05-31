#!/usr/bin/env python3
import argparse
import io
import json
import os
import shutil
import struct
import subprocess
import sys
import zlib
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List, Optional, Tuple

SCRIPT_DIR = Path(__file__).resolve().parent
sys.path.insert(0, str(SCRIPT_DIR))

try:
    import rl_upk_editor as upk
except Exception as exc:
    print(json.dumps({
        "success": False,
        "message": f"Alphy UPK backend missing: {exc}",
    }))
    sys.exit(2)


PACKAGE_FILE_TAG = 0x9E2A83C1
TFC_BLOCK_SIZE = 0x20000


BODY_TARGETS = {
    23: {
        "label": "Octane",
        "upk": "Startup.upk",
        "textures": {
            "Diffuse": ("Pepe_Body_D", "PF_DXT5"),
            "Skin": ("Pepe_Body_BlankSkin", "PF_DXT5"),
            "Chassis.Diffuse": ("Chasis_Pepe_D", "PF_DXT1"),
        },
    },
    4284: {
        "label": "Fennec",
        "upk": "body_grain_SF.upk",
        "textures": {
            "Diffuse": ("Body_Grain_D", "PF_DXT5"),
            "Skin": ("Body_Grain_BlankSkin", "PF_DXT5"),
            "Chassis.Diffuse": ("Chassis_Grain_D", "PF_DXT1"),
        },
    },
    403: {
        "label": "Dominus",
        "upk": "Body_MuscleCar_SF.upk",
        "textures": {
            "Diffuse": ("MuscleCar_Body_D", "PF_DXT5"),
            "Skin": ("MuscleCar_BlankSkin", "PF_DXT5"),
            "Chassis.Diffuse": ("MuscleCar_Chassis_D", "PF_DXT1"),
        },
    },
    4770: {
        "label": "Dominus",
        "upk": "Body_MuscleCar_SF.upk",
        "textures": {
            "Diffuse": ("MuscleCar_Body_D", "PF_DXT5"),
            "Skin": ("MuscleCar_BlankSkin", "PF_DXT5"),
            "Chassis.Diffuse": ("MuscleCar_Chassis_D", "PF_DXT1"),
        },
    },
}

BALL_TARGETS = [
    ("GameInfo_Soccar_SF.upk", "Ball_Default00_D", "PF_DXT1"),
    ("Mutators_Balls_SF.upk", "Ball_Default00_D", "PF_DXT1"),
]

BOOST_METER_TARGETS = {
    "Background": ("GFX_Hud_SF.upk", "BoostMeter_Background", "PF_DXT5"),
    "Fill": ("GFX_Hud_SF.upk", "BoostMeter_Fill", "PF_DXT5"),
    "Tintable": ("GFX_Hud_SF.upk", "BoostMeter_FillTintablePortion", "PF_DXT5"),
    "Glow": ("GFX_Hud_SF.upk", "BoostMeter_Glow", "PF_DXT5"),
}


@dataclass
class MipInfo:
    flags_offset: int
    flags: int
    element_count: int
    element_count_offset: int
    disk_size: int
    disk_size_offset: int
    offset64: Optional[int]
    offset64_offset: Optional[int]
    data_offset: Optional[int]
    width: int
    height: int
    width_offset: int
    height_offset: int


@dataclass
class TextureInfo:
    export: object
    format_name: str
    tfc_name: str
    mips: List[MipInfo]


def result(success: bool, message: str, **extra) -> int:
    payload = {"success": success, "message": message}
    payload.update(extra)
    print(json.dumps(payload, ensure_ascii=False))
    return 0 if success else 1


def log(message: str) -> None:
    print(message, flush=True)


def expected_dxt_size(width: int, height: int, fmt: str) -> int:
    block_bytes = 8 if fmt == "PF_DXT1" else 16
    return max(1, (width + 3) // 4) * max(1, (height + 3) // 4) * block_bytes


def expected_texture_size(width: int, height: int, fmt: str) -> int:
    if fmt == "PF_A8R8G8B8":
        return width * height * 4
    return expected_dxt_size(width, height, fmt)


def encode_dds_mip_with_pillow(image_path: Path, width: int, height: int, fmt: str) -> bytes:
    from PIL import Image

    pixel_format = "DXT1" if fmt == "PF_DXT1" else "DXT5"
    with Image.open(image_path) as image:
        resampling = getattr(Image, "Resampling", Image).LANCZOS
        mip = image.convert("RGBA").resize((width, height), resampling)
        buf = io.BytesIO()
        mip.save(buf, format="DDS", pixel_format=pixel_format)
    raw = buf.getvalue()
    if len(raw) < 128 or raw[:4] != b"DDS ":
        raise ValueError(f"Pillow did not produce a valid {pixel_format} DDS")
    return raw[128:]


def encode_dds_mip_with_texconv(image_path: Path,
                                width: int,
                                height: int,
                                fmt: str,
                                work_dir: Path,
                                texconv_path: Path) -> bytes:
    if not texconv_path.exists():
        raise FileNotFoundError(f"texconv.exe not found: {texconv_path}")

    out_dir = work_dir / "texconv"
    out_dir.mkdir(parents=True, exist_ok=True)
    dds_fmt = "BC1_UNORM" if fmt == "PF_DXT1" else "BC3_UNORM"
    cmd = [
        str(texconv_path),
        "-nologo",
        "-f", dds_fmt,
        "-m", "1",
        "-w", str(width),
        "-h", str(height),
        "-y",
        "-o", str(out_dir),
        str(image_path),
    ]
    proc = subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    if proc.returncode != 0:
        raise ValueError(f"texconv failed for {image_path.name}: {proc.stdout.strip()}")

    candidates = list(out_dir.glob(f"{image_path.stem}.dds")) + list(out_dir.glob(f"{image_path.stem}.DDS"))
    if not candidates:
        candidates = sorted(out_dir.glob("*.dds")) + sorted(out_dir.glob("*.DDS"))
    if not candidates:
        raise ValueError(f"texconv did not write a DDS for {image_path}")

    raw = candidates[0].read_bytes()
    if len(raw) < 128 or raw[:4] != b"DDS ":
        raise ValueError(f"texconv output is not a valid DDS: {candidates[0]}")
    return raw[128:]


def encode_dds_mip(image_path: Path,
                   width: int,
                   height: int,
                   fmt: str,
                   work_dir: Path,
                   texconv_path: Path) -> bytes:
    try:
        payload = encode_dds_mip_with_pillow(image_path, width, height, fmt)
    except Exception:
        payload = encode_dds_mip_with_texconv(image_path, width, height, fmt, work_dir, texconv_path)

    expected = expected_dxt_size(width, height, fmt)
    if len(payload) != expected:
        pixel_format = "DXT1" if fmt == "PF_DXT1" else "DXT5"
        raise ValueError(
            f"{pixel_format} payload size mismatch for {width}x{height}: "
            f"{len(payload)} != {expected}"
        )
    return payload


def encode_bgra_mip(image_path: Path, width: int, height: int) -> bytes:
    from PIL import Image

    with Image.open(image_path) as image:
        resampling = getattr(Image, "Resampling", Image).LANCZOS
        mip = image.convert("RGBA").resize((width, height), resampling)
        rgba = mip.tobytes()

    out = bytearray(len(rgba))
    out[0::4] = rgba[2::4]
    out[1::4] = rgba[1::4]
    out[2::4] = rgba[0::4]
    out[3::4] = rgba[3::4]
    return bytes(out)


def image_dimensions(image_path: Path) -> Tuple[int, int]:
    from PIL import Image

    with Image.open(image_path) as image:
        return image.size


def encode_texture_mip(image_path: Path,
                       width: int,
                       height: int,
                       fmt: str,
                       work_dir: Path,
                       texconv_path: Path) -> bytes:
    if fmt == "PF_A8R8G8B8":
        payload = encode_bgra_mip(image_path, width, height)
    else:
        payload = encode_dds_mip(image_path, width, height, fmt, work_dir, texconv_path)

    expected = expected_texture_size(width, height, fmt)
    if len(payload) != expected:
        raise ValueError(
            f"{fmt} payload size mismatch for {width}x{height}: "
            f"{len(payload)} != {expected}"
        )
    return payload


def build_tfc_payload(raw_dxt: bytes) -> bytes:
    chunks: List[bytes] = []
    table: List[Tuple[int, int]] = []
    for pos in range(0, len(raw_dxt), TFC_BLOCK_SIZE):
        block = raw_dxt[pos:pos + TFC_BLOCK_SIZE]
        comp = zlib.compress(block, 9)
        chunks.append(comp)
        table.append((len(comp), len(block)))

    out = bytearray()
    out += struct.pack("<IIII", PACKAGE_FILE_TAG, TFC_BLOCK_SIZE,
                       sum(len(c) for c in chunks), len(raw_dxt))
    for comp_size, uncomp_size in table:
        out += struct.pack("<II", comp_size, uncomp_size)
    for comp in chunks:
        out += comp
    return bytes(out)


def clean_name(text: str) -> str:
    return str(text).split("\x00", 1)[0]


def find_export(package, texture_name: str):
    matches = [
        exp for exp in package.exports
        if clean_name(package.resolve_name(exp.object_name)).lower() == texture_name.lower()
    ]
    if not matches:
        raise ValueError(f"Texture2D export not found: {texture_name}")
    exp = matches[0]
    cls = package.export_class_name(exp)
    if clean_name(cls) != "Texture2D":
        raise ValueError(f"Export {texture_name} is {cls}, not Texture2D")
    return exp


def property_map(package, export) -> Tuple[Dict[str, object], int]:
    raw = package.object_data(export)
    start, _props = upk._find_best_property_stream_offset(package, raw, None, None)
    end, props, ended = upk._try_parse_property_stream(package, raw, start)
    if not ended:
        raise ValueError(f"Could not parse Texture2D properties for {package.resolve_name(export.object_name)}")
    return {prop.name: prop for prop in props}, end


def parse_texture(package, texture_name: str) -> TextureInfo:
    export = find_export(package, texture_name)
    props, end = property_map(package, export)
    fmt_prop = props.get("Format")
    tfc_prop = props.get("TextureFileCacheName")
    if not fmt_prop or not tfc_prop:
        raise ValueError(f"Texture2D metadata incomplete: {texture_name}")

    fmt = str(fmt_prop.value)
    if fmt not in ("PF_DXT1", "PF_DXT5", "PF_A8R8G8B8"):
        raise ValueError(f"Unsupported texture format for {texture_name}: {fmt}")

    tfc_name = str(tfc_prop.value).strip()
    raw = package.object_data(export)
    cursor = end + 12
    if cursor + 4 > len(raw):
        raise ValueError(f"Texture2D mip table missing: {texture_name}")

    mip_count = struct.unpack_from("<I", raw, cursor)[0]
    cursor += 4
    if mip_count <= 0 or mip_count > 32:
        raise ValueError(f"Invalid mip count for {texture_name}: {mip_count}")

    mips: List[MipInfo] = []
    for _ in range(mip_count):
        flags_offset = cursor
        flags, element_count, disk_size = struct.unpack_from("<III", raw, cursor)
        cursor += 12
        element_count_offset = flags_offset + 4
        disk_size_offset = flags_offset + 8
        external = (flags & 0x3) == 0x3
        offset64 = None
        offset64_offset = None
        data_offset = None
        if external:
            offset64_offset = cursor
            offset64 = struct.unpack_from("<Q", raw, cursor)[0]
            cursor += 8
        else:
            data_offset = cursor
            cursor += disk_size
        width_offset = cursor
        height_offset = cursor + 4
        width, height = struct.unpack_from("<II", raw, cursor)
        cursor += 8
        expected = expected_texture_size(width, height, fmt)
        if expected != element_count:
            raise ValueError(
                f"Mip size mismatch in {texture_name} {width}x{height}: "
                f"{element_count} != {expected}"
            )
        mips.append(MipInfo(
            flags_offset=flags_offset,
            flags=flags,
            element_count=element_count,
            element_count_offset=element_count_offset,
            disk_size=disk_size,
            disk_size_offset=disk_size_offset,
            offset64=offset64,
            offset64_offset=offset64_offset,
            data_offset=data_offset,
            width=width,
            height=height,
            width_offset=width_offset,
            height_offset=height_offset,
        ))
    return TextureInfo(export=export, format_name=fmt, tfc_name=tfc_name, mips=mips)


def range_record_path(backup_dir: Path, action: str) -> Path:
    return backup_dir / f"{action}_ranges.json"


def restore_range_record(record_path: Path) -> int:
    if not record_path.exists():
        return 0

    record = json.loads(record_path.read_text(encoding="utf-8-sig"))
    restored = 0
    for entry in reversed(record.get("ranges", [])):
        target_path = Path(entry.get("filePath") or entry["tfcPath"])
        backup_file = Path(entry["backupFile"])
        offset = int(entry["offset"])
        expected = int(entry["size"])
        if not target_path.exists() or not backup_file.exists():
            continue
        data = backup_file.read_bytes()
        if len(data) != expected:
            raise ValueError(f"Backup range size mismatch: {backup_file}")
        with target_path.open("r+b") as fh:
            fh.seek(offset)
            fh.write(data)
        backup_file.unlink()
        restored += 1

    for entry in reversed(record.get("truncates", [])):
        target = Path(entry["filePath"])
        size = int(entry["size"])
        if not target.exists():
            continue
        with target.open("r+b") as fh:
            fh.truncate(size)
        restored += 1

    record_path.unlink()
    return restored


def record_file_range(file_path: Path,
                      offset: int,
                      size: int,
                      record: dict,
                      seen_ranges: set) -> None:
    key = (str(file_path).lower(), int(offset), int(size))
    if key in seen_ranges:
        return
    idx = len(record["ranges"])
    backup_file = Path(record["backupDir"]) / f"range_{idx:04d}.bin"
    with file_path.open("rb") as fh:
        fh.seek(offset)
        original = fh.read(size)
    if len(original) != size:
        raise ValueError(f"Could not read original file range: {file_path}")
    backup_file.write_bytes(original)
    record["ranges"].append({
        "filePath": str(file_path),
        "offset": int(offset),
        "size": int(size),
        "backupFile": str(backup_file),
    })
    seen_ranges.add(key)


def record_file_size(file_path: Path, record: dict, seen_truncates: set) -> None:
    key = str(file_path).lower()
    if key in seen_truncates:
        return
    record.setdefault("truncates", []).append({
        "filePath": str(file_path),
        "size": int(file_path.stat().st_size),
    })
    seen_truncates.add(key)


def append_tfc_payload_recorded(tfc_path: Path,
                                payload: bytes,
                                record: dict,
                                seen_truncates: set) -> int:
    tfc_path.parent.mkdir(parents=True, exist_ok=True)
    record_file_size(tfc_path, record, seen_truncates)
    with tfc_path.open("ab") as fh:
        offset = fh.tell()
        fh.write(payload)
    return offset


def write_tfc_payload_in_place(tfc_path: Path,
                               offset: int,
                               disk_size: int,
                               payload: bytes,
                               record: dict,
                               seen_ranges: set) -> None:
    if len(payload) > disk_size:
        raise ValueError(
            f"Encoded texture is too large for in-place TFC patch: "
            f"{len(payload)} > {disk_size}"
        )
    record_file_range(tfc_path, offset, disk_size, record, seen_ranges)
    padded = payload + (b"\x00" * (disk_size - len(payload)))
    with tfc_path.open("r+b") as fh:
        fh.seek(offset)
        fh.write(padded)


def decompress_rl_chunk_payload(payload: bytes) -> bytes:
    bio = io.BytesIO(payload)
    tag, block_size, _total_comp, total_uncomp = struct.unpack("<IIII", bio.read(16))
    if tag != PACKAGE_FILE_TAG:
        raise ValueError("Invalid compressed UPK chunk tag")

    blocks: List[Tuple[int, int]] = []
    remaining = total_uncomp
    while remaining > 0:
        comp_size, uncomp_size = struct.unpack("<II", bio.read(8))
        blocks.append((comp_size, uncomp_size))
        remaining -= uncomp_size

    out = bytearray()
    for comp_size, uncomp_size in blocks:
        block = zlib.decompress(bio.read(comp_size))
        if len(block) != uncomp_size:
            raise ValueError("Compressed UPK chunk block size mismatch")
        out += block
    if len(out) != total_uncomp:
        raise ValueError("Compressed UPK chunk total size mismatch")
    del block_size
    return bytes(out)


def zlib_compress_best(data: bytes) -> bytes:
    best = None
    strategies = (zlib.Z_DEFAULT_STRATEGY, zlib.Z_FILTERED)
    for strategy in strategies:
        for level in (9, 8, 7, 6):
            for mem_level in range(1, 10):
                comp = zlib.compressobj(level, zlib.DEFLATED, zlib.MAX_WBITS, mem_level, strategy)
                out = comp.compress(data) + comp.flush()
                if best is None or len(out) < len(best):
                    best = out
    return best if best is not None else zlib.compress(data, 9)


def compress_upk_chunk_payload_optimized(uncompressed: bytes) -> bytes:
    blocks: List[Tuple[bytes, int]] = []
    total_compressed = 0
    for pos in range(0, len(uncompressed), TFC_BLOCK_SIZE):
        piece = uncompressed[pos:pos + TFC_BLOCK_SIZE]
        comp = zlib_compress_best(piece)
        blocks.append((comp, len(piece)))
        total_compressed += len(comp)

    out = bytearray()
    out += struct.pack("<I", PACKAGE_FILE_TAG)
    out += struct.pack("<i", TFC_BLOCK_SIZE)
    out += struct.pack("<ii", total_compressed, len(uncompressed))
    for comp, uncomp_size in blocks:
        out += struct.pack("<ii", len(comp), uncomp_size)
    for comp, _ in blocks:
        out += comp
    return bytes(out)


def encrypted_chunks_for(upk_path: Path, keys_path: Optional[Path]):
    provider = upk.DecryptionProvider(str(keys_path)) if keys_path and keys_path.exists() else upk.DecryptionProvider()
    _summary, meta, encrypted_data, valid_key = upk.find_valid_key(upk_path, provider)
    plain_header = upk.DecryptionProvider.decrypt_ecb(valid_key, encrypted_data)
    return upk.parse_rl_compressed_chunks(plain_header, meta.compressed_chunks_offset)


def patch_upk_compressed_chunks_in_place(upk_path: Path,
                                         keys_path: Optional[Path],
                                         patches: List[Tuple[int, bytes]],
                                         record: dict,
                                         seen_ranges: set) -> None:
    if not patches:
        return

    chunks = encrypted_chunks_for(upk_path, keys_path)
    grouped: Dict[int, List[Tuple[int, bytes]]] = {}
    for absolute_offset, value in patches:
        owner = None
        for idx, chunk in enumerate(chunks):
            start = chunk.uncompressed_offset
            end = start + chunk.uncompressed_size
            if start <= absolute_offset and absolute_offset + len(value) <= end:
                owner = idx
                break
        if owner is None:
            raise ValueError(f"UPK metadata patch offset is outside compressed chunks: {absolute_offset}")
        grouped.setdefault(owner, []).append((absolute_offset, value))

    for idx, chunk_patches in grouped.items():
        chunk = chunks[idx]
        with upk_path.open("rb") as fh:
            fh.seek(chunk.compressed_offset)
            original_payload = fh.read(chunk.compressed_size)
        if len(original_payload) != chunk.compressed_size:
            raise ValueError(f"Could not read compressed UPK chunk: {upk_path}")

        plain = bytearray(decompress_rl_chunk_payload(original_payload))
        for absolute_offset, value in chunk_patches:
            local = absolute_offset - chunk.uncompressed_offset
            plain[local:local + len(value)] = value

        new_payload = compress_upk_chunk_payload_optimized(bytes(plain))
        if len(new_payload) > chunk.compressed_size:
            raise ValueError(
                f"Patched UPK chunk grew too much: {len(new_payload)} > {chunk.compressed_size}"
            )

        record_file_range(upk_path, chunk.compressed_offset, chunk.compressed_size, record, seen_ranges)
        padded = new_payload + (b"\x00" * (chunk.compressed_size - len(new_payload)))
        with upk_path.open("r+b") as fh:
            fh.seek(chunk.compressed_offset)
            fh.write(padded)


def replace_upk_with_rebuilt_package(upk_path: Path,
                                     source_path: Path,
                                     package,
                                     provider,
                                     patches: List[Tuple[int, bytes]],
                                     record: dict,
                                     seen_ranges: set,
                                     seen_truncates: set,
                                     work_dir: Path) -> None:
    if provider is None:
        raise ValueError("Patched UPK chunk grew too much and this package cannot be rebuilt safely")

    modified = bytearray(package.file_bytes)
    for absolute_offset, value in patches:
        if absolute_offset < 0 or absolute_offset + len(value) > len(modified):
            raise ValueError(f"UPK metadata patch offset is outside package bounds: {absolute_offset}")
        modified[absolute_offset:absolute_offset + len(value)] = value

    rebuilt = upk.build_reencrypted_package(
        source_path,
        bytes(modified),
        provider,
        work_dir / f"{source_path.stem}_alphy_rebuilt.upk",
    )
    record_file_size(upk_path, record, seen_truncates)
    record_file_range(upk_path, 0, upk_path.stat().st_size, record, seen_ranges)
    upk_path.write_bytes(rebuilt.read_bytes())


def patch_upk_compressed_chunks(upk_path: Path,
                                source_path: Path,
                                package,
                                provider,
                                keys_path: Optional[Path],
                                patches: List[Tuple[int, bytes]],
                                record: dict,
                                seen_ranges: set,
                                seen_truncates: set,
                                work_dir: Path) -> None:
    try:
        patch_upk_compressed_chunks_in_place(upk_path, keys_path, patches, record, seen_ranges)
    except ValueError as exc:
        if "Patched UPK chunk grew too much" not in str(exc):
            raise
        replace_upk_with_rebuilt_package(
            upk_path, source_path, package, provider, patches,
            record, seen_ranges, seen_truncates, work_dir
        )


def resolve_with_optional_keys(input_path: Path, work_dir: Path, keys_path: Optional[Path]):
    if not keys_path:
        return upk.resolve_input_package(input_path, work_dir, SCRIPT_DIR)
    old_find = getattr(upk, "find_keys_path", None)
    if old_find is None:
        return upk.resolve_input_package(input_path, work_dir, SCRIPT_DIR)

    def forced(_script_dir, _selected_file):
        return keys_path

    upk.find_keys_path = forced
    try:
        return upk.resolve_input_package(input_path, work_dir, SCRIPT_DIR)
    finally:
        upk.find_keys_path = old_find


def build_targets(config: dict) -> Dict[str, Tuple[Path, str, str]]:
    body_id = int(config.get("bodyId", -1))
    target = BODY_TARGETS.get(body_id)
    if not target:
        raise ValueError(f"Unsupported custom decal body id: {body_id}")

    out: Dict[str, Tuple[Path, str, str]] = {}
    diffuse_path = config.get("diffusePath") or config.get("skinPath") or ""
    skin_path = config.get("maskPath") or config.get("skinPath") or config.get("diffusePath") or ""
    images = {
        "Diffuse": diffuse_path,
        "Skin": skin_path,
        "Chassis.Diffuse": config.get("chassisDiffusePath") or "",
    }
    for role, image_path in images.items():
        if not image_path:
            continue
        texture = target["textures"].get(role)
        if not texture:
            continue
        p = Path(image_path)
        if p.exists() and p.is_file():
            out[role] = (p, texture[0], texture[1])

    if not out and not config.get("skinPackageTexturePath"):
        raise ValueError("The decal pack has no usable texture images")
    return out


def load_item_by_id(items_path: Path, item_id: int) -> Optional[dict]:
    if not items_path.exists():
        return None

    data = json.loads(items_path.read_text(encoding="utf-8-sig"))
    if isinstance(data, dict):
        if isinstance(data.get("Items"), list):
            items = data["Items"]
        elif isinstance(data.get("Products"), list):
            items = data["Products"]
        elif isinstance(data.get("items"), list):
            items = data["items"]
        else:
            items = [v for v in data.values() if isinstance(v, dict)]
    elif isinstance(data, list):
        items = data
    else:
        items = []

    for item in items:
        if isinstance(item, dict) and int(item.get("ID", -1)) == item_id:
            return item
    return None


def texture_export_names(package) -> List[str]:
    names: List[str] = []
    for exp in package.exports:
        if clean_name(package.export_class_name(exp)) == "Texture2D":
            names.append(clean_name(package.resolve_name(exp.object_name)))
    return names


def choose_skin_package_texture_names(package) -> List[str]:
    candidates: List[Tuple[int, int, str]] = []
    bad_terms = ("noise", "grad", "curvature", "chassis", "_n", "normal", "pbr", "mask", "guide", "blackalpha")
    for name in texture_export_names(package):
        low = name.lower()
        if any(term in low for term in bad_terms):
            continue
        score = 0
        if "basecolor" in low:
            score += 100
        if "body_paintable" in low or "paintable" in low:
            score += 50
        if "body" in low:
            score += 20
        if "decal" in low or "skin" in low:
            score += 10
        if "_bg" in low or low.endswith("bg") or "background" in low:
            score += 85
        if low.startswith("t_auto"):
            score += 35
        if score <= 0:
            continue
        try:
            info = parse_texture(package, name)
            top = info.mips[0]
            area = top.width * top.height
            if info.format_name == "PF_A8R8G8B8":
                score += 25
            candidates.append((score, area, name))
        except Exception:
            continue

    candidates.sort(reverse=True)
    return [candidates[0][2]] if candidates else []


def choose_skin_package_trim_texture_names(package) -> List[str]:
    candidates: List[Tuple[int, int, str]] = []
    bad_terms = ("guide", "blackalpha", "normal", "_n", "mask")
    for name in texture_export_names(package):
        low = name.lower()
        if any(term in low for term in bad_terms):
            continue
        score = 0
        if "trim" in low:
            score += 100
        if "logo" in low:
            score += 90
        if score <= 0:
            continue
        try:
            info = parse_texture(package, name)
            top = info.mips[0]
            candidates.append((score, top.width * top.height, name))
        except Exception:
            continue

    candidates.sort(reverse=True)
    return [candidates[0][2]] if candidates else []


def build_skin_package_jobs(config: dict,
                            cooked_dir: Path,
                            work_dir: Path,
                            keys_path: Optional[Path]) -> Tuple[Optional[Path], List[Tuple[Path, str, str]]]:
    image_path = Path(config.get("skinPackageTexturePath") or "")
    if not image_path.exists() or not image_path.is_file():
        return None, []

    skin_id = int(config.get("skinId", 0))
    if skin_id <= 0:
        return None, []

    items_path = Path(config.get("itemsPath") or (SCRIPT_DIR / "items.json"))
    item = load_item_by_id(items_path, skin_id)
    if not item:
        raise ValueError(f"SkinID {skin_id} was not found in items.json")

    package_name = item.get("AssetPackage")
    if not package_name:
        raise ValueError(f"SkinID {skin_id} has no AssetPackage in items.json")

    package_path = cooked_dir / package_name
    if not package_path.exists():
        raise ValueError(f"Skin package not found: {package_path}")

    _decrypted_path, package, _provider, _keys, _was_encrypted = resolve_with_optional_keys(
        package_path, work_dir, keys_path
    )
    texture_names = choose_skin_package_texture_names(package)
    jobs = [(image_path, texture_name, "auto") for texture_name in texture_names]

    trim_path = Path(config.get("trimSheetPath") or "")
    if trim_path.exists() and trim_path.is_file():
        for texture_name in choose_skin_package_trim_texture_names(package):
            jobs.append((trim_path, texture_name, "auto"))

    if not jobs:
        raise ValueError(f"No patchable decal texture found in {package_name}")

    return package_path, jobs


def patch_one_upk(upk_path: Path,
                  source_path: Path,
                  cooked_dir: Path,
                  texture_jobs: List[Tuple[Path, str, str]],
                  work_dir: Path,
                  keys_path: Optional[Path],
                  record: dict,
                  seen_ranges: set,
                  seen_truncates: set,
                  texconv_path: Path,
                  preserve_image_dimensions: bool = False) -> Tuple[int, List[str]]:
    _decrypted_path, package, provider, _keys, _was_encrypted = resolve_with_optional_keys(
        source_path, work_dir, keys_path
    )
    patched_names: List[str] = []
    upk_patches: List[Tuple[int, bytes]] = []

    for image_path, texture_name, expected_fmt in texture_jobs:
        info = parse_texture(package, texture_name)
        if expected_fmt not in ("", "auto") and info.format_name != expected_fmt:
            raise ValueError(
                f"{texture_name} format changed: expected {expected_fmt}, found {info.format_name}"
            )
        can_resize_texture = preserve_image_dimensions and info.mips and info.mips[0].offset64 is not None
        source_size = image_dimensions(image_path) if can_resize_texture else None
        if preserve_image_dimensions and source_size:
            props, _end = property_map(package, info.export)
            source_width, source_height = source_size
            for prop_name in ("SizeX", "OriginalSizeX"):
                prop = props.get(prop_name)
                if prop and prop.size == 4:
                    upk_patches.append((info.export.serial_offset + prop.value_offset, struct.pack("<i", source_width)))
            for prop_name in ("SizeY", "OriginalSizeY"):
                prop = props.get(prop_name)
                if prop and prop.size == 4:
                    upk_patches.append((info.export.serial_offset + prop.value_offset, struct.pack("<i", source_height)))

        for mip_index, mip in enumerate(info.mips):
            width, height = mip.width, mip.height
            if preserve_image_dimensions and source_size and mip_index == 0:
                width, height = source_size

            raw_texture = encode_texture_mip(image_path, width, height, info.format_name, work_dir, texconv_path)
            if len(raw_texture) != mip.element_count:
                if not preserve_image_dimensions:
                    raise ValueError(f"Encoded mip size mismatch for {texture_name}")
                upk_patches.append((info.export.serial_offset + mip.element_count_offset, struct.pack("<I", len(raw_texture))))
                upk_patches.append((info.export.serial_offset + mip.width_offset, struct.pack("<I", width)))
                upk_patches.append((info.export.serial_offset + mip.height_offset, struct.pack("<I", height)))

            if mip.offset64 is None:
                if mip.data_offset is None:
                    raise ValueError(f"Inline mip data is missing for {texture_name}")
                if len(raw_texture) != mip.disk_size:
                    raise ValueError(
                        f"Inline mip size mismatch for {texture_name}: "
                        f"{len(raw_texture)} != {mip.disk_size}"
                    )
                upk_patches.append((info.export.serial_offset + mip.data_offset, raw_texture))
                continue

            tfc_path = cooked_dir / f"{info.tfc_name}.tfc"
            if not tfc_path.exists():
                raise ValueError(f"Texture cache not found: {tfc_path}")

            payload = build_tfc_payload(raw_texture)
            if len(payload) <= mip.disk_size:
                write_tfc_payload_in_place(tfc_path, mip.offset64, mip.disk_size, payload, record, seen_ranges)
            else:
                new_offset = append_tfc_payload_recorded(tfc_path, payload, record, seen_truncates)
                upk_patches.append((info.export.serial_offset + mip.disk_size_offset, struct.pack("<I", len(payload))))
                upk_patches.append((info.export.serial_offset + int(mip.offset64_offset), struct.pack("<Q", int(new_offset))))

        patched_names.append(texture_name)

    patch_upk_compressed_chunks(
        upk_path, source_path, package, provider, keys_path, upk_patches,
        record, seen_ranges, seen_truncates, work_dir
    )
    return len(patched_names), patched_names


def rollback_record(record_path: Path, record: dict) -> None:
    if record.get("ranges") or record.get("truncates"):
        record_path.write_text(json.dumps(record, indent=2), encoding="utf-8")
    restore_range_record(record_path)


def common_paths(config: dict) -> Tuple[Path, Path, Path, Optional[Path], Path]:
    cooked_dir = Path(config["cookedDir"])
    backup_dir = Path(config["backupDir"])
    work_dir = Path(config["workDir"])
    keys_path = Path(config["keysPath"]) if config.get("keysPath") else None
    texconv_path = Path(config.get("texconvPath") or (SCRIPT_DIR / "texconv.exe"))
    backup_dir.mkdir(parents=True, exist_ok=True)
    work_dir.mkdir(parents=True, exist_ok=True)
    return cooked_dir, backup_dir, work_dir, keys_path, texconv_path


def apply_decal(config: dict) -> int:
    cooked_dir, backup_dir, work_dir, keys_path, texconv_path = common_paths(config)
    record_path = range_record_path(backup_dir, "custom_decal")
    restore_range_record(record_path)

    raw_targets = config.get("decalTargets") or config.get("targets")
    if isinstance(raw_targets, list) and raw_targets:
        target_configs = []
        for target in raw_targets:
            if not isinstance(target, dict):
                continue
            merged = dict(config)
            merged.pop("decalTargets", None)
            merged.pop("targets", None)
            merged.update(target)
            target_configs.append(merged)
    else:
        target_configs = [config]

    record = {"backupDir": str(backup_dir), "ranges": [], "truncates": []}
    seen_ranges = set()
    seen_truncates = set()
    count = 0
    names: List[str] = []
    patched_bodies: List[str] = []
    try:
        for target_config in target_configs:
            body_id = int(target_config.get("bodyId", -1))
            target_meta = BODY_TARGETS.get(body_id)
            if not target_meta and body_id != -1:
                raise ValueError(f"Unsupported custom decal body id: {body_id}")

            body_label = target_config.get("carBody") or (target_meta.get("label") if target_meta else "Universal Decal") or str(body_id)
            jobs = list(build_targets(target_config).values()) if target_meta else []
            if not jobs and not target_config.get("skinPackageTexturePath"):
                continue

            if jobs:
                upk_name = target_meta["upk"]
                upk_path = cooked_dir / upk_name
                if not upk_path.exists():
                    return result(False, f"Target UPK not found: {upk_path}")

                body_count, body_names = patch_one_upk(
                    upk_path, upk_path, cooked_dir, jobs, work_dir, keys_path,
                    record, seen_ranges, seen_truncates, texconv_path
                )
                count += body_count
                names.extend(f"{body_label}:{name}" for name in body_names)

            skin_package_path, skin_jobs = build_skin_package_jobs(target_config, cooked_dir, work_dir, keys_path)
            if skin_package_path and skin_jobs:
                skin_count, skin_names = patch_one_upk(
                    skin_package_path, skin_package_path, cooked_dir, skin_jobs,
                    work_dir, keys_path, record, seen_ranges, seen_truncates, texconv_path
                )
                count += skin_count
                names.extend(f"{body_label}:{name}" for name in skin_names)

            if body_label not in patched_bodies:
                patched_bodies.append(body_label)

        if count == 0:
            raise ValueError("The decal pack has no usable texture images")

        record_path.write_text(json.dumps(record, indent=2), encoding="utf-8")
    except Exception as exc:
        try:
            rollback_record(record_path, record)
        except Exception:
            pass
        return result(False, str(exc))

    body = config.get("carBody") or " ".join(patched_bodies) or str(config.get("bodyId"))
    return result(True, f"Custom decal applied to {body} textures ({count} textures).", textures=names)


def restore_decals(config: dict) -> int:
    _cooked_dir, backup_dir, _work_dir, _keys_path, _texconv_path = common_paths(config)
    restored = restore_range_record(range_record_path(backup_dir, "custom_decal"))
    return result(True, f"Custom decals restored ({restored} texture ranges).")


def apply_ball(config: dict) -> int:
    cooked_dir, backup_dir, work_dir, keys_path, texconv_path = common_paths(config)
    diffuse = Path(config.get("diffusePath") or "")
    if not diffuse.exists():
        return result(False, "Ball pack is missing Diffuse texture")

    record_path = range_record_path(backup_dir, "custom_ball")
    restore_range_record(record_path)
    record = {"backupDir": str(backup_dir), "ranges": [], "truncates": []}
    seen_ranges = set()
    seen_truncates = set()
    names: List[str] = []
    try:
        for upk_name, texture_name, fmt in BALL_TARGETS:
            upk_path = cooked_dir / upk_name
            if not upk_path.exists():
                continue
            _count, patched = patch_one_upk(
                upk_path, upk_path, cooked_dir, [(diffuse, texture_name, fmt)],
                work_dir, keys_path, record, seen_ranges, seen_truncates, texconv_path
            )
            names.extend(f"{upk_name}:{name}" for name in patched)
        if not names:
            raise ValueError("No default ball Texture2D targets were found")
        record_path.write_text(json.dumps(record, indent=2), encoding="utf-8")
    except Exception as exc:
        try:
            rollback_record(record_path, record)
        except Exception:
            pass
        return result(False, str(exc))

    return result(True, f"Custom ball applied ({len(names)} targets).", textures=names)


def restore_ball(config: dict) -> int:
    _cooked_dir, backup_dir, _work_dir, _keys_path, _texconv_path = common_paths(config)
    restored = restore_range_record(range_record_path(backup_dir, "custom_ball"))
    return result(True, f"Custom ball restored ({restored} texture ranges).")


def apply_boost_meter(config: dict) -> int:
    cooked_dir, backup_dir, work_dir, keys_path, texconv_path = common_paths(config)
    textures = config.get("textures") or {}
    if not isinstance(textures, dict) or not textures:
        return result(False, "Boost meter pack has no textures")

    jobs_by_upk: Dict[str, List[Tuple[Path, str, str]]] = {}
    for role, meta in BOOST_METER_TARGETS.items():
        image_value = textures.get(role) or ""
        image_path = Path(image_value)
        if image_value and image_path.exists() and image_path.is_file():
            jobs_by_upk.setdefault(meta[0], []).append((image_path, meta[1], meta[2]))

    if not jobs_by_upk:
        return result(False, "Boost meter pack has no usable textures")

    record_path = range_record_path(backup_dir, "boost_meter")
    restore_range_record(record_path)
    record = {"backupDir": str(backup_dir), "ranges": [], "truncates": []}
    seen_ranges = set()
    seen_truncates = set()
    names: List[str] = []
    try:
        for upk_name, jobs in jobs_by_upk.items():
            upk_path = cooked_dir / upk_name
            if not upk_path.exists():
                raise ValueError(f"Target UPK not found: {upk_path}")
            _count, patched = patch_one_upk(
                upk_path, upk_path, cooked_dir, jobs,
                work_dir, keys_path, record, seen_ranges, seen_truncates, texconv_path
            )
            names.extend(f"{upk_name}:{name}" for name in patched)
        if not names:
            raise ValueError("No boost meter Texture2D targets were patched")
        record_path.write_text(json.dumps(record, indent=2), encoding="utf-8")
    except Exception as exc:
        try:
            rollback_record(record_path, record)
        except Exception:
            pass
        return result(False, str(exc))

    return result(True, f"Custom boost meter applied ({len(names)} textures).", textures=names)


def restore_boost_meter(config: dict) -> int:
    _cooked_dir, backup_dir, _work_dir, _keys_path, _texconv_path = common_paths(config)
    restored = restore_range_record(range_record_path(backup_dir, "boost_meter"))
    return result(True, f"Custom boost meter restored ({restored} texture ranges).")


def main() -> int:
    parser = argparse.ArgumentParser(description="Alphy custom texture injector")
    parser.add_argument("--config", required=True)
    args = parser.parse_args()
    try:
        config = json.loads(Path(args.config).read_text(encoding="utf-8-sig"))
    except Exception as exc:
        return result(False, f"Cannot read custom texture config: {exc}")

    action = config.get("action", "apply")
    if action == "apply":
        return apply_decal(config)
    if action == "restore":
        return restore_decals(config)
    if action == "apply_ball":
        return apply_ball(config)
    if action == "restore_ball":
        return restore_ball(config)
    if action == "apply_boost_meter":
        return apply_boost_meter(config)
    if action == "restore_boost_meter":
        return restore_boost_meter(config)
    return result(False, f"Unknown custom texture action: {action}")


if __name__ == "__main__":
    sys.exit(main())
