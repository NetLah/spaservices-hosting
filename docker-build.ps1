[CmdletBinding()]
param (
    [Parameter(Mandatory = $false)] [string] $Context = '.',
    [Parameter(Mandatory = $false)] [string] $Dockerfile = '',
    [Parameter(Mandatory = $false)] [string] $Tags,
    [Parameter(Mandatory = $false)] [string] $Labels,
    [Parameter(Mandatory = $false)] [string] $BuildArgs,
    [Parameter(Mandatory = $false)] [switch] $NoPull,
    [Parameter(Mandatory = $false)] [switch] $Squash,
    [Parameter(Mandatory = $false)] [switch] $NoPush
)

Write-Output "Powershell Version: $($PSVersionTable.PSVersion)"
Write-Output "Tags (raw): $Tags"
Write-Output "Labels (raw): $Labels"

$tagStrs = [Array]::Reverse($Tags.Trim() -split '\r|\n' | Where-Object { $_ })
$labelStrs = $Labels.Trim() -split '\r|\n' | Where-Object { $_ }
$buildArgStrs = $BuildArgs.Trim() -split '\r|\n' | Where-Object { $_ }

if (!$tagStrs) {
    throw 'Tags is required'
}

$params = @('build', $Context)

if (!$NoPull) {
    $params += @('--pull')
}

if ($Squash -And !($Env:OS)) {
    $params += @('--squash')
}

if ($Dockerfile) {
    $params += @('--file', $Dockerfile)
}

if ($labelStrs) {
    $params += $labelStrs | ForEach-Object { @('--label', $_) }
}

if ($tagStrs) {
    $params += $tagStrs | ForEach-Object { @('--tag', $_) }
}

if ($buildArgStrs) {
    $params += $buildArgStrs | ForEach-Object { @('--build-arg', $_) }
}

Write-Host "docker $params"
docker @params 
if (!$?) {
    exit $LASTEXITCODE
}

if (!$NoPush -And $tagStrs) {
    Write-Host "Pushing docker images"
    foreach ($dockerImageTag in $tagStrs) {
        docker push $dockerImageTag
        if (!$?) {
            exit $LASTEXITCODE
        }
    }
}
