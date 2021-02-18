@echo off

SET DOCFX=%USERPROFILE%\.nuget\packages\docfx.console\2.56.6\tools\docfx.exe DocFx.json

if %1.==. (
    SET DOCFX=%DOCFX% --serve
) else (
    SET DOCFX=%DOCFX% %1 %2 %3
)

dotnet restore
echo %DOCFX%
