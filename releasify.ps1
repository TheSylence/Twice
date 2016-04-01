$channel = "http://software.btbsoft.org/twice/packages"

$nupkg = Get-ChildItem Twice\bin\Release -Filter "*.nupkg" | Select-Object -First 1
$nupkg = $nupkg.FullName

Invoke-Expression "squirrel --releasify=$nupkg -b=$channel --no-msi --icon=Twice\Resources\TwitterIcon.ico"

Remove-Item $nupkg
Move-Item Releases\Setup.exe Releases\twice-setup.exe