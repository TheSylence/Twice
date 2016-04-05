$channel = "http://software.btbsoft.org/twice/packages"

$nupkg = Get-ChildItem Twice\bin\Release -Filter "*.nupkg" | Select-Object -First 1
$nupkg = $nupkg.FullName

#Invoke-Expression "squirrel --releasify=$nupkg -b=$channel --no-msi --icon=Twice\Resources\TwitterIcon.ico"

#Remove-Item $nupkg
#Move-Item Releases\Setup.exe Releases\twice-setup.exe

$version = ($nupkg -split "(\d+\.\d+\.\d+(\.\d+)?)")[1]
$baseDir = Split-Path $MyInvocation.MyCommand.Path
$projectFile = Join-Path $baseDir "Twice.smproj"
$sourceDir = $baseDir

Copy-Item smcmd.xml smcmd.tmp.xml
(Get-Content smcmd.tmp.xml).replace('@CHECKPOINT_NAME@', "test") | Set-Content smcmd.tmp.xml
(Get-Content smcmd.tmp.xml).replace('@PROJECT_FILE@', $projectFile) | Set-Content smcmd.tmp.xml
(Get-Content smcmd.tmp.xml).replace('@SOURCE_DIR@', $sourceDir) | Set-Content smcmd.tmp.xml

$sm = "C:\Program Files (x86)\SourceMonitor\SourceMonitor.exe"
Start-Process $sm -argumentlist "/C smcmd.tmp.xml" -wait

#Remove-Item smcmd.tmp.xml