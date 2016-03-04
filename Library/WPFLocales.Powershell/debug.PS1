$OutputDllPath				= "C:\Users\Boikov\Documents\WPFLocales\Library\WPFLocales.Powershell\bin\Debug\WPFLocales.Powershell.dll"
$TargetProjPath				= "C:\Users\Boikov\Documents\Development\eas-client\src\EmergencyAlertSystem\EmergencyAlertSystem.csproj"
$VS_VERSION                 = "VisualStudio2015"

Import-Module $OutputDllPath
$DTE = Get-DTE -TargetProjPath $TargetProjPath -VsVersion $VS_VERSION
$Env:DTE = $DTE

Sync-Localization