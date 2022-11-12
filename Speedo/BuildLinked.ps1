# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/Speedo/*" -Force -Recurse
dotnet publish "./Speedo.csproj" -c Release -o "$env:RELOADEDIIMODS/Speedo" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location