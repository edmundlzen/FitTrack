$env:JAVA_HOME = "C:\Program Files\Android\Android Studio\jbr"
Set-Location "C:\Users\edmund\source\repos\FitTrack\FitTrack.Maui"
dotnet build -f net10.0-android -p:JavaSdkDirectory="$env:JAVA_HOME" 2>&1
