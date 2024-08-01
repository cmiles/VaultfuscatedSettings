$ErrorActionPreference = "Stop"

$PublishVersion = get-date -f yyyy-MM-dd-HH-mm

$GitCommit = & git rev-parse --short HEAD

dotnet clean .\VaultfuscatedSettings.sln -property:Configuration=Release -property:Platform=x64 -verbosity:minimal

dotnet restore .\VaultfuscatedSettings.sln -r win-x64 -verbosity:minimal

$vsWhere = "{0}\Microsoft Visual Studio\Installer\vswhere.exe" -f ${env:ProgramFiles(x86)}

$msBuild = & $vsWhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

& $msBuild .\VaultfuscatedSettings.sln -property:Configuration=Release -property:Platform=x64 -verbosity:minimal

if ($lastexitcode -ne 0) { throw ("Exec: " + $errorMessage) }

$publishPath = "M:\PointlessWaymarksPublications\PointlessWaymarks.VaultfuscatedTextEditor"
if(!(test-path -PathType container $publishPath)) { New-Item -ItemType Directory -Path $publishPath }

Remove-Item -Path $publishPath\* -Recurse

& $msBuild .\PointlessWaymarks.VaultfuscatedTextEditor\PointlessWaymarks.VaultfuscatedTextEditor.csproj -t:publish -p:PublishProfile=.\PointlessWaymarks.VaultfuscatedTextEditor\Properties\PublishProfile\FolderProfile.pubxml -verbosity:minimal

if ($lastexitcode -ne 0) { throw ("Exec: " + $errorMessage) }

& 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe' .\Publish-InnoSetupInstaller-VaultfuscatedTextEditor.iss /DVersion=$PublishVersion /DGitCommit=$GitCommit

if ($lastexitcode -ne 0) { throw ("Exec: " + $errorMessage) }


