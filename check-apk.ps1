$apk = "C:\Users\edmund\source\repos\FitTrack\FitTrack.Maui\bin\Debug\net10.0-android\com.companyname.fittrack.maui-Signed.apk"
$bytes = [System.IO.File]::ReadAllBytes($apk)
$text = [System.Text.Encoding]::UTF8.GetString($bytes)
if ($text -match "text-white") { Write-Output "OLD_UI_FOUND" }
if ($text -match "font-size:1.5rem") { Write-Output "NEW_UI_FOUND" }
if ($text -notmatch "text-white" -and $text -notmatch "font-size:1.5rem") { Write-Output "NEITHER_FOUND" }
