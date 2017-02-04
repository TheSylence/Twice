$nupkg = Get-ChildItem Twice\bin\Release -Filter "*.nupkg" | Select-Object -First 1
$nupkg = $nupkg.FullName

$newname = [string]($nupkg).substring(0, $nupkg.length -8) + ".nupkg"

Rename-Item $nupkg $newname
$nupkg = $newname
Remove-Item Releases\Setup.exe

Invoke-Expression "squirrel --releasify=$nupkg --no-msi --icon=Twice\Resources\TwitterIcon.ico"

Remove-Item $nupkg