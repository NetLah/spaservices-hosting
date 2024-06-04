[CmdletBinding()]
param (
    [Parameter(Mandatory = $false, Position = 0)]
    [string] $PublishRoot,
    [string[]] $NugetSources,
    [string] $Artifacts,
    [string] $Coverage
)

$root = Split-Path $MyInvocation.MyCommand.Path -Parent -Resolve
. $root/buildHelper.ps1

if (!$Artifacts) {
    $Artifacts = 'artifacts'
}
if (!$Coverage) {
    $Coverage = "$Artifacts/coverage"   # %cus.P_COVERAGE%
}

StartBuildSaveLocation

# blockOpened 'git-clean' 'git clean'
# git clean -xdf -e .vs -e NuGet.Config
# if (!$?) { FailBuild 'git clean' }
# blockClosed 'git-clean'

blockOpened 'dotnet-tool-restore' 'dotnet tool restore'
if ($NugetSources) {
    $params = @()
    foreach ($NugetSource in $NugetSources) {
        $params += @('--add-source', $NugetSource)
    }
    dotnet tool restore @params || FailBuild 'dotnet tool restore'
}
else {
    dotnet tool restore
    if (!$?) { FailBuild 'dotnet tool restore' }
}
blockClosed 'dotnet-tool-restore'

blockOpened 'dotnet-restore' 'dotnet restore'
if ($NugetSources) {
    $params = @()
    foreach ($NugetSource in $NugetSources) {
        $params += @('--source', $NugetSource)
    }
    dotnet restore @params || FailBuild 'dotnet restore'
}
else {
    dotnet restore || FailBuild 'dotnet restore'
}
blockClosed 'dotnet-restore'

blockOpened 'dotnet-build' 'dotnet build'
dotnet build -c Release --no-restore -nodereuse:false || FailBuild 'dotnet build'
blockClosed 'dotnet-build'

if (!$PublishRoot) {
    $PublishRoot = $Artifacts
    Write-Host "Set publishRoot: $PublishRoot"
}

foreach ($targetVersionNum in @("6.0", "7.0", "8.0", "9.0")) {
    $path = "$publishRoot/$targetVersionNum"
    $targetVersion = if ($targetVersionNum -eq '3.1') { 'netcoreapp3.1' } else { "net$targetVersionNum" }

    DropFolderRetry $path -Retry 3

    blockOpened ".NET-$targetVersionNum" ".NET $targetVersionNum"

    blockOpened 'dotnet-publish-src\WebApp' 'dotnet publish src\WebApp'
    dotnet publish src\WebApp -f $targetVersion -c Release --no-build -o $path/spa-host
    if (!$?) { FailBuild 'dotnet publish src\WebApp' }
    blockClosed 'dotnet-publish-src\WebApp'

    Write-Host "##teamcity[setParameter name='cus.tfmNet$($targetVersionNum)' value='true']"
    blockClosed ".NET-$targetVersionNum"
}

FinishBuild
