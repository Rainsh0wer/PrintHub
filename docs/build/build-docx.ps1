<#
    Build .docx deliverables from the Markdown sources.

    Run this again any time you edit the .md files — it is idempotent and
    overwrites the output. Editing the .md and rebuilding is much less painful
    than editing Word directly.

    Usage:  powershell -ExecutionPolicy Bypass -File build-docx.ps1
#>

$ErrorActionPreference = 'Stop'

$Pandoc    = "$env:LOCALAPPDATA\Pandoc\pandoc.exe"
$DocsDir   = Split-Path -Parent $PSScriptRoot
$BuildDir  = $PSScriptRoot
$OutDir    = Join-Path $DocsDir 'docx'
$Filter    = Join-Path $BuildDir 'linebreak.lua'
$RefDoc    = Join-Path (Split-Path -Parent $DocsDir) '1. Template Intro .docx'

if (-not (Test-Path $Pandoc)) { throw "pandoc not found at $Pandoc" }
if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Path $OutDir | Out-Null }

# ---------------------------------------------------------------------------
# Strip the hand-written "Table of Contents" block.
# Pandoc generates a real, updatable Word TOC field via --toc, so keeping the
# markdown one would produce two tables of contents.
# ---------------------------------------------------------------------------
function Remove-ManualToc {
    param([string[]]$Lines)

    $out      = New-Object System.Collections.Generic.List[string]
    $skipping = $false

    foreach ($line in $Lines) {
        if ($line -match '^##\s+Table of Contents\s*$') { $skipping = $true; continue }
        if ($skipping) {
            # Resume emitting once the first real chapter heading appears.
            if ($line -match '^#\s+(I\.|Report)') { $skipping = $false } else { continue }
        }
        $out.Add($line)
    }
    return $out
}

function Convert-ToDocx {
    param(
        [string[]]$SourceFiles,
        [string]  $OutputName,
        [string]  $Title
    )

    $merged = @()
    foreach ($f in $SourceFiles) {
        $path = Join-Path $DocsDir $f
        if (-not (Test-Path $path)) { throw "source not found: $path" }
        $merged += Get-Content -Path $path -Encoding UTF8
        $merged += ''
        $merged += ''
    }

    $merged = Remove-ManualToc -Lines $merged

    $tmp = Join-Path $env:TEMP "printhub_$([guid]::NewGuid().ToString('N')).md"
    Set-Content -Path $tmp -Value $merged -Encoding UTF8

    $outPath = Join-Path $OutDir $OutputName

    $args = @(
        $tmp
        '--from=markdown+pipe_tables+raw_html'
        '--to=docx'
        "--output=$outPath"
        "--lua-filter=$Filter"
        '--toc'
        '--toc-depth=3'
        '--metadata'
        "title=$Title"
    )
    if (Test-Path $RefDoc) { $args += "--reference-doc=$RefDoc" }

    & $Pandoc @args
    Remove-Item $tmp -Force

    $size = [math]::Round((Get-Item $outPath).Length / 1KB, 1)
    Write-Output "  OK  $OutputName  ($size KB)"
}

Write-Output "Building .docx into $OutDir"

Convert-ToDocx `
    -SourceFiles @('1_Project_Introduction.md') `
    -OutputName  '1_Project_Introduction.docx' `
    -Title       'PrintHub - Report 1: Project Introduction'

Convert-ToDocx `
    -SourceFiles @('2_SRS_Part1_Overview.md','2_SRS_Part2_UseCases.md','2_SRS_Part3_Requirements.md') `
    -OutputName  '2_SRS.docx' `
    -Title       'PrintHub - Report 2: Software Requirement Specification'

Write-Output ""
Write-Output "Done. Open the files and press Ctrl+A then F9 to refresh the table of contents."
