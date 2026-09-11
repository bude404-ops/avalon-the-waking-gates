#!/usr/bin/env python3
"""Minimal stdlib-only binary AXML (AndroidManifest) parser.
Reconstructs element/attribute structure so launcher verification never
depends on androguard/pip being present in the CI image."""
import struct

RES_STRING_POOL_TYPE = 0x0001
RES_XML_START_TYPE = 0x0100
RES_XML_START_NAMESPACE_TYPE = 0x0100
RES_XML_START_ELEMENT_TYPE = 0x0102
RES_XML_END_ELEMENT_TYPE = 0x0103
RES_XML_RESOURCE_MAP_TYPE = 0x0180

class Node:
    def __init__(self, name):
        self.name = name
        self.attrs = {}
        self.children = []

def parse_axml(data):
    # ---- string pool ----
    # header: type(2) size(2) size(4) then chunk
    strings = []
    pos = 8  # skip file header
    def u16(o): return struct.unpack_from('<H', data, o)[0]
    def u32(o): return struct.unpack_from('<I', data, o)[0]
    while pos < len(data) - 8:
        ctype = u16(pos); csize = u32(pos+4)
        if ctype == RES_STRING_POOL_TYPE:
            sc = u32(pos+12); stc = u32(pos+8)
            flags = u32(pos+16)
            str_start = u32(pos+20)
            is_utf8 = (flags & (1 << 8)) != 0
            for i in range(stc):
                if is_utf8:
                    # u16 len, u8(byte-len), u8(char-len), bytes, 0
                    off = pos + str_start + u32(pos + 28 + i*4)
                    _decl16 = struct.unpack_from('<H', data, off)[0]
                    o = off + 2
                    blen = data[o]; o += 1
                    if blen & 0x80:
                        blen = ((blen & 0x7F) << 8) | data[o]; o += 1
                    s = data[o:o+blen].decode('utf-8', 'replace')
                else:
                    off = pos + str_start + u32(pos + 28 + i*4)
                    n = struct.unpack_from('<H', data, off)[0]
                    o = off + 2
                    if n & 0x8000:
                        n = ((n & 0x7FFF) << 16) | struct.unpack_from('<H', data, o)[0]
                        o += 2
                    s = data[o:o+n*2].decode('utf-16-le', 'replace')
                strings.append(s)
        pos += csize
    # ---- walk chunks, build tree ----
    root = Node('root'); stack = [root]
    pos = 8
    while pos < len(data) - 8:
        ctype = u16(pos); csize = u32(pos+4)
        if csize < 8 or pos + csize > len(data) + 8: break
        if ctype == RES_XML_START_ELEMENT_TYPE:
            lineno = u32(pos+16)
            name_idx = u32(pos+20)
            attr_start = u16(pos+24); attr_size = u16(pos+26)
            attr_count = u16(pos+28)
            node = Node(strings[name_idx] if name_idx < len(strings) else '?')
            base = pos + attr_start + attr_start  # attrStart is relative to chunk start? no:
            # attribute offset = chunkStart + headerSize(0x10 for start-elem with lineno) + attrStart - 20?? use standard: attrs begin at pos + 16 + attrStart
            abase = pos + 16 + attr_start
            for i in range(attr_count):
                a = abase + i * attr_size
                ns_idx = u32(a); name_idx2 = u32(a+4); raw_idx = u32(a+8)
                vtype = data[a+15]; vdata = u32(a+16)
                aname = strings[name_idx2] if name_idx2 < len(strings) else '?'
                if raw_idx != 0xFFFFFFFF and raw_idx < len(strings):
                    aval = strings[raw_idx]
                elif vtype == 0x03 and vdata < len(strings):
                    aval = strings[vdata]
                elif vtype == 0x10:
                    aval = str(vdata)
                elif vtype == 0x12:
                    aval = 'true' if vdata else 'false'
                else:
                    aval = str(vdata)
                if not aname.startswith('android:') and ns_idx != 0xFFFFFFFF:
                    aname = 'android:' + aname.split(':')[-1]
                node.attrs[aname] = aval
            stack[-1].children.append(node)
            stack.append(node)
        elif ctype == RES_XML_END_ELEMENT_TYPE:
            if len(stack) > 1: stack.pop()
        pos += csize
    return root

def findall(node, name):
    out = []
    if node.name == name: out.append(node)
    for c in node.children: out.extend(findall(c, name))
    return out

def get_manifest_tree(apk_path):
    import zipfile
    z = zipfile.ZipFile(apk_path)
    return parse_axml(z.read('AndroidManifest.xml'))
