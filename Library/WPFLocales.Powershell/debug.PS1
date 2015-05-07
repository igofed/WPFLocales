 
  
# $devenv = [System.Diagnostics.Process]::GetProcessesByName("devenv") | Where-Object {$_.Id -eq $Env:devenvPID}
# if ($devenv)

Import-Module "d:\Development\WPFLocales\Library\WPFLocales.Powershell\bin\Debug\WPFLocales.Powershell.dll"
#$type = [Type]::GetTypeFromProgID("VisualStudio.DTE.12.0");
#$DTE = [System.Activator]::CreateInstance($type)
$DTE = Get-DTE -VSPath "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" -ProjPath "D:\Development\eas-client\src\EmergencyAlertSystem\EmergencyAlertSystem.csproj"
[Environment]::SetEnvironmentVariable("DTE", $DTE, "User")
Sync-Localization