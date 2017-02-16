$OutputDllPath				= "C:\Users\Boikov\Documents\Development\WPFLocales\Library\WPFLocales.Powershell\bin\Debug\WPFLocales.Powershell.dll"
$TargetProjPath				= "C:\Users\Boikov\Documents\Development\eas-client\src\EmergencyAlertSystem\EmergencyAlertSystem.csproj"

Import-Module $OutputDllPath
$DTE = Get-DTE -TargetProjPath $TargetProjPath
$Env:DTE = $DTE

Sync-Localization