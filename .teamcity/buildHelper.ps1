function CopyCoverage([string]$source, [string]$coveragePath) {
    $coverlet = (Get-ChildItem -Path $source)
    if ($coverlet -isnot [System.IO.DirectoryInfo]) {
        Write-Error "Cannot file testResult inside $source" -ErrorAction Stop
    }
    
    if (!(Test-Path -Path $coveragePath -Type Container)) {
        $coverage = New-Item -Path $coveragePath -Type Directory
        Write-Verbose $coverage
    }
    
    Copy-Item -Path "$($coverlet.FullName)/*" -Destination $coveragePath  -Force -Confirm:$false -ErrorAction Stop
}

function DropFolderRetry([string] $folder, [int] $Retry) {
    for ($i = $Retry; $i -ge 1; $i--) {
        if ($folder -and (Test-Path $folder -ErrorAction SilentlyContinue)) {
            Write-Verbose "Drop Folder/Remove-Item $folder / $i"
            if ($i -gt 1) {
                Remove-Item -Path $folder -Recurse -Force -Confirm:$false -ErrorAction Ignore
            }
            else {
                Remove-Item -Path $folder -Recurse -Force -Confirm:$false -ErrorAction Stop
            }
        }        
        if (!$folder -or !(Test-Path $folder -ErrorAction SilentlyContinue)) {
            break            
        }
        Write-Verbose 'Remove-Item failed, sleep for 1 second'
        Start-Sleep -Seconds 1
    }
}

function TouchFile {
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )
  
    if (Test-Path -LiteralPath $Path) {
        (Get-Item -Path $Path).LastWriteTime = Get-Date
    }
    else {
        New-Item -Type File -Path $Path | Out-Null
    }
}

function blockOpened($block, $description) {
    Write-Host "##teamcity[blockOpened name='$($block)' description='$($description)']"
    progressMessage $description
}

function blockClosed($block) {
    Write-Host "##teamcity[blockClosed name='$($block)']"
}

function progressStart($message) {
    Write-Host "##teamcity[progressStart '$($message)']"
}

function progressMessage($message) {
    Write-Host "##teamcity[progressMessage '$($message)']"
}

function progressFinish($message) {
    Write-Host "##teamcity[progressFinish '$($message)']"
}

function FailBuild($subStep) {
    Write-Host "##teamcity[buildProblem description='$($subStep) failed.']"
    RestoreLocation
    EndBuild
    Write-Host "Build failed, elapsed $($_buildElapsedTime.TotalSeconds) seconds"
    exit 1
}

function StartBuildSaveLocation {
    StartBuild
    SaveLocation
}

function FinishBuildRestoreLocation {
    RestoreLocation
    FinishBuild
}

function StartBuild {
    $Script:_buildStartTime = Get-Date
    Write-Output "Build start $_buildStartTime"
}

function EndBuild {
    $Script:_buildEndTime = Get-Date
    $Script:_buildElapsedTime = New-TimeSpan $_buildStartTime $_buildEndTime
}

function FinishBuild {
    EndBuild
    progressMessage 'Build is done!'
    Write-Host "Build elapsed $($_buildElapsedTime.TotalSeconds) seconds"
}

function SaveLocation {
    $Script:_buildSaveOriginDirectory = Get-Location
}

function RestoreLocation {
    if ($Script:_buildSaveOriginDirectory) {
        Set-Location $Script:_buildSaveOriginDirectory
    }
}
