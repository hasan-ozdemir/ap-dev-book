Param(
    [switch]$SkipLinks
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "==> Building documentation with MkDocs (strict mode)..." -ForegroundColor Cyan
try {
    mkdocs build --strict --clean | Write-Host
}
catch {
    Write-Error "MkDocs build failed. Ensure mkdocs-material is installed (pip install mkdocs-material)."
    throw
}

if ($SkipLinks) {
    Write-Host "Skipping external link validation (SkipLinks requested)." -ForegroundColor Yellow
    return
}

Write-Host "==> Checking external links in Markdown files..." -ForegroundColor Cyan
if (-not (Get-Command python -ErrorAction SilentlyContinue)) {
    Write-Error "Python is required for link validation. Install Python 3.11+ or run with -SkipLinks."
    throw
}

$scriptPath = Join-Path -Path (Split-Path -Parent $MyInvocation.MyCommand.Path) -ChildPath "check_links.py"
python $scriptPath

Write-Host "Documentation validation completed successfully." -ForegroundColor Green
