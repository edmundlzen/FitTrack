$apk = "C:\Users\edmund\source\repos\FitTrack\FitTrack.Maui\bin\Debug\net10.0-android\com.companyname.fittrack.maui-Signed.apk"
$bytes = [System.IO.File]::ReadAllBytes($apk)
$text = [System.Text.Encoding]::UTF8.GetString($bytes)
if ($text -match "background removed to let Bootstrap") { Write-Output "CSS_FIX_FOUND" }
else { Write-Output "CSS_FIX_NOT_FOUND" }

# Also check for the old rule
if ($text -match "background: var\(--ft-card\)") { Write-Output "OLD_RULE_STILL_THERE" }
