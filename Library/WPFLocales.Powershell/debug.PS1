Import-Module "d:\Development\WPFLocales\Library\WPFLocales.Powershell\bin\Debug\WPFLocales.Powershell.dll"
$DTE = Get-DTE -ProjPath "D:\Development\eas-client\src\EmergencyAlertSystem\EmergencyAlertSystem.csproj"
$Env:DTE = $DTE

Sync-Localization