# Render every diagram source in this folder to docs/diagrams/png.
#   Mermaid (.mmd)  -> mermaid-cli   (npm i -g @mermaid-js/mermaid-cli)
#   PlantUML (.puml)-> plantuml.jar  (auto-downloaded here if missing; needs Java)
# PlantUML uses the built-in Smetana layout, so Graphviz is NOT required.
#
# Usage:  powershell -ExecutionPolicy Bypass -File docs/diagrams/render.ps1

$ErrorActionPreference = 'Stop'
$dg  = $PSScriptRoot
$png = Join-Path $dg 'png'
New-Item -ItemType Directory -Force -Path $png | Out-Null

# --- PlantUML jar ---
$jar = Join-Path $dg 'plantuml.jar'
if (-not (Test-Path $jar)) {
    Write-Host 'Downloading plantuml.jar ...'
    Invoke-WebRequest -UseBasicParsing `
        -Uri 'https://github.com/plantuml/plantuml/releases/download/v1.2024.7/plantuml-1.2024.7.jar' `
        -OutFile $jar
}

Write-Host '=== PlantUML (.puml) ==='
Get-ChildItem $dg -Filter *.puml | ForEach-Object {
    & java -jar $jar -tpng -o $png $_.FullName | Out-Null
    Write-Host "  $($_.BaseName).png"
}

Write-Host '=== Mermaid (.mmd) ==='
Get-ChildItem $dg -Filter *.mmd | ForEach-Object {
    $out = Join-Path $png ($_.BaseName + '.png')
    & mmdc -i $_.FullName -o $out -b white -s 2 | Out-Null
    Write-Host "  $($_.BaseName).png"
}

Write-Host "Done -> $png"
