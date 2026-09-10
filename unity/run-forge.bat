@echo off
REM AVALON FORGE launcher — double-click-friendly.
REM If run with no args, runs the pipeline for Sovereign. Edit CLASS below or:
REM   run-forge.bat Ravager "C:\Projects\avalon-mythos"
set CLASS=%1
if "%CLASS%"=="" set CLASS=Sovereign
set PROJ=%2
if "%PROJ%"=="" set PROJ=C:\Projects\avalon-mythos
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$u = Get-ChildItem 'C:\Program Files\Unity\Hub\Editor\*' -Recurse -Filter Unity.exe -ErrorAction SilentlyContinue | Select-Object -First 1; & $u.FullName -batchmode -quit -projectPath '%PROJ%' -executeMethod AvalonForge.Forge.Run -character %CLASS%"
echo.
echo FORGE EXIT CODE: %ERRORLEVEL%   (0 = prefab forged, 1 = check forge-artifacts + Console)
pause
