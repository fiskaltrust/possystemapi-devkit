# Builds DocFX documentation site

$ErrorActionPreference = "Stop"

Push-Location $PSScriptRoot
try {
  dotnet build ../../libPosSystemAPI/libPOSSystemAPI.csproj -c Release
  dotnet tool restore
  dotnet docfx docfx.json
  Write-Host "DocFX site generated at: $PSScriptRoot\_site\index.html" -ForegroundColor Green
  Write-Host "To run a local server, execute: 'dotnet docfx serve _site'" -ForegroundColor Green
} finally {
  Pop-Location
}
