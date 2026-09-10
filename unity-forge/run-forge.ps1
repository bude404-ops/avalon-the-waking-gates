# AVALON FORGE — one-shot runner for Big's seat (Windows PowerShell).
# Usage:
#   .\run-forge.ps1 -Mode probes                # first run: 404-GEN verification probes
#   .\run-forge.ps1 -Mode pipeline               # run the character pipeline (forge-config.json)
#   .\run-forge.ps1 -Mode pipeline -Class Sovereign
# Optional: -Schedule "22:00" registers a nightly Task Scheduler job that runs the
# pipeline while the Editor is closed. Batch mode is NOT license-free: the machine
# must hold an activated, signed-in Unity license (your Personal seat covers this).

param(
    [ValidateSet("probes", "pipeline")] [string]$Mode = "probes",
    [string]$Class = "",
    [string]$ProjectPath = "C:\Projects\avalon-mythos",
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe",
    [string]$Schedule = ""
)

$ErrorActionPreference = "Stop"
$LogFile = Join-Path $ProjectPath "forge-logs\forge-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"
New-Item -ItemType Directory -Force -Path (Split-Path $LogFile) | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $ProjectPath "forge-artifacts") | Out-Null

# pull the latest pipeline code + config
git -C $ProjectPath pull origin main 2>$null

$method = if ($Mode -eq "probes") { "AvalonForge.Editor.Gen404Probes.Run" } else { "AvalonForge.RunClass" }
if ($Mode -eq "pipeline" -and $Class -eq "") { $Class = "Sovereign-M" }
$env:AVALON_CLASS = $Class
$extra = if ($Class -ne "") { @("-class", $Class) } else { @() }

Write-Host "== AVALON FORGE: $Mode $Class =="
& $UnityExe -batchmode -quit -projectPath $ProjectPath -executeMethod $method @extra -logFile $LogFile
$code = $LASTEXITCODE

# surface the artifacts
$art = Join-Path $ProjectPath "forge-artifacts"
if (Test-Path "$art\probe-report.json") { Get-Content "$art\probe-report.json" | Select-Object -First 40 }
if (Test-Path "$art\error.txt") { Write-Host "FORGE ERROR:"; Get-Content "$art\error.txt" }

# push results back so BIGagent404 can inspect and iterate
git -C $ProjectPath add forge-artifacts 2>$null
git -C $ProjectPath commit -m "forge artifacts: $Mode $Class $(Get-Date -Format 'yyyy-MM-dd HH:mm')" 2>$null
git -C $ProjectPath push origin main 2>$null

Write-Host "== FORGE EXIT CODE: $code (0=ok 1=pipeline-fail 2=crash) =="

if ($Schedule -ne "") {
    $action = New-ScheduledTaskAction -Execute "powershell.exe" -Argument "-File `$PSCommandPath -Mode pipeline"
    $trigger = New-ScheduledTaskTrigger -Daily -At $Schedule
    Register-ScheduledTask -TaskName "AvalonForge" -Action $action -Trigger $trigger -Force | Out-Null
    Write-Host "Scheduled: AvalonForge runs nightly at $Schedule (Editor stays closed; license must remain signed in)."
}
