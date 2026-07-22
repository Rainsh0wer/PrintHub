# -*- coding: utf-8 -*-
"""Convert erd.mmd (generated from the live DB) -> erd.dbml for dbdiagram.io.
Default style (no custom colors). Run: py docs/diagrams/gen_dbml.py
"""
import os
import re

HERE = os.path.dirname(os.path.abspath(__file__))
MMD = os.path.join(HERE, "erd.mmd")
OUT = os.path.join(HERE, "erd.dbml")


def parse(path):
    tables = {}       # name -> [(type, field, keys)]
    order = []        # preserve table order
    refs = []         # (child, col, parent)
    cur = None
    with open(path, encoding="utf-8") as f:
        for line in f:
            s = line.strip()
            if not s or s.startswith("%%") or s == "erDiagram":
                continue
            m = re.match(r"^(\w+)\s*\{$", s)
            if m:
                cur = m.group(1)
                tables[cur] = []
                order.append(cur)
                continue
            if s == "}":
                cur = None
                continue
            if cur:
                parts = s.split()
                if len(parts) >= 2:
                    typ, name = parts[0], parts[1]
                    keys = " ".join(parts[2:])
                    tables[cur].append((typ, name, keys))
                continue
            # relationship line, e.g.  A ||--o{ B : ColName
            if ":" in s and "--" in s:
                left, label = s.split(":", 1)
                toks = left.split()
                if len(toks) >= 3:
                    parent, child = toks[0], toks[-1]
                    refs.append((child, label.strip(), parent))
    return order, tables, refs


def dbml(order, tables, refs):
    out = ["// PrintHub ERD - generated from the live PrintHubDb schema.",
           "// Paste into https://dbdiagram.io (default style).",
           ""]
    for tbl in order:
        out.append(f"Table {tbl} {{")
        for typ, name, keys in tables[tbl]:
            attrs = []
            if "PK" in keys:
                attrs.append("pk")
            if "UK" in keys:
                attrs.append("unique")
            settings = f" [{', '.join(attrs)}]" if attrs else ""
            out.append(f"  {name} {typ}{settings}")
        out.append("}")
        out.append("")
    out.append("// Relationships (FK -> PK)")
    for child, col, parent in refs:
        out.append(f"Ref: {child}.{col} > {parent}.Id")
    out.append("")
    return "\n".join(out)


def main():
    order, tables, refs = parse(MMD)
    text = dbml(order, tables, refs)
    with open(OUT, "w", encoding="utf-8") as f:
        f.write(text)
    print(text)
    print(f"\n// wrote {OUT}  ({len(order)} tables, {len(refs)} refs)")


if __name__ == "__main__":
    main()
