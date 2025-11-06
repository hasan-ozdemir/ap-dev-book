#!/usr/bin/env python3
"""
Lightweight Markdown link checker for the devyum portal.

Scans all Markdown files under ./docs, extracts http(s) links, and issues HEAD
requests (falling back to GET when necessary). Fails with a non-zero exit code
if any URLs consistently return 4xx/5xx responses or cannot be reached.
"""

from __future__ import annotations

import argparse
import concurrent.futures
import re
import ssl
import sys
import time
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable
from urllib.error import HTTPError, URLError
from urllib.request import Request, urlopen

URL_PATTERN = re.compile(r"\[(?:[^\]]+)\]\((https?://[^)\s]+)\)")
DEFAULT_TIMEOUT = 10


@dataclass(frozen=True)
class LinkResult:
    url: str
    status: str
    ok: bool
    source_files: tuple[str, ...]


def extract_links(paths: Iterable[Path]) -> dict[str, set[str]]:
    url_sources: dict[str, set[str]] = {}
    repo_root = Path.cwd().resolve()
    for path in paths:
        text = path.read_text(encoding="utf-8")
        resolved = path.resolve()
        for match in URL_PATTERN.finditer(text):
            url = match.group(1).rstrip(")")
            try:
                relative = resolved.relative_to(repo_root)
            except ValueError:
                relative = path
            url_sources.setdefault(url, set()).add(str(relative).replace("\\", "/"))
    return url_sources


def probe_url(url: str, timeout: int) -> LinkResult:
    ctx = ssl.create_default_context()
    headers = {"User-Agent": "devyum-link-check/1.0"}
    req = Request(url, headers=headers, method="HEAD")
    try:
        with urlopen(req, timeout=timeout, context=ctx) as resp:  # nosec: B310
            status = f"{resp.status} {resp.reason}"
            return LinkResult(url, status, 200 <= resp.status < 400, tuple())
    except HTTPError as exc:
        if exc.code in {405, 501}:
            # Retry with GET for servers that dislike HEAD.
            return probe_get(url, timeout, ctx, headers)
        return LinkResult(url, f"{exc.code} {exc.reason}", False, tuple())
    except URLError as exc:
        return LinkResult(url, f"Connection error: {exc.reason}", False, tuple())


def probe_get(url: str, timeout: int, ctx: ssl.SSLContext, headers: dict[str, str]) -> LinkResult:
    req = Request(url, headers=headers, method="GET")
    try:
        with urlopen(req, timeout=timeout, context=ctx) as resp:  # nosec: B310
            status = f"{resp.status} {resp.reason}"
            return LinkResult(url, status, 200 <= resp.status < 400, tuple())
    except HTTPError as exc:
        return LinkResult(url, f"{exc.code} {exc.reason}", False, tuple())
    except URLError as exc:
        return LinkResult(url, f"Connection error: {exc.reason}", False, tuple())


def run_checks(timeout: int, max_workers: int) -> list[LinkResult]:
    doc_root = Path("docs")
    md_files = sorted(doc_root.rglob("*.md"))
    if not md_files:
        print("No Markdown files found under ./docs", file=sys.stderr)
        return []

    link_map = extract_links(md_files)
    if not link_map:
        print("No external links discovered in Markdown content.")
        return []

    results: list[LinkResult] = []
    print(f"Checking {len(link_map)} unique links (timeout {timeout}s)...")
    start = time.perf_counter()

    with concurrent.futures.ThreadPoolExecutor(max_workers=max_workers) as executor:
        future_map = {
            executor.submit(probe_url, url, timeout): (url, tuple(sorted(sources)))
            for url, sources in link_map.items()
        }
        for future in concurrent.futures.as_completed(future_map):
            url, sources = future_map[future]
            result = future.result()
            results.append(LinkResult(url, result.status, result.ok, sources))

    elapsed = time.perf_counter() - start
    failures = [r for r in results if not r.ok]

    print(f"Link check completed in {elapsed:.2f}s.")
    if failures:
        print("\nBroken links detected:")
        for item in sorted(failures, key=lambda r: r.url):
            files = ", ".join(item.source_files)
            print(f"  - {item.url} -> {item.status} (referenced in {files})")
    else:
        print("All external links responded successfully.")

    return results


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Check external links in Markdown files.")
    parser.add_argument(
        "--timeout",
        type=int,
        default=DEFAULT_TIMEOUT,
        help=f"HTTP timeout per request in seconds (default: {DEFAULT_TIMEOUT})",
    )
    parser.add_argument(
        "--workers",
        type=int,
        default=16,
        help="Number of concurrent workers (default: 16)",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    results = run_checks(timeout=args.timeout, max_workers=args.workers)
    if not results:
        return 0
    failures = [r for r in results if not r.ok]
    return 0 if not failures else 1


if __name__ == "__main__":
    sys.exit(main())
