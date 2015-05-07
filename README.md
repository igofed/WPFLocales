Library for localization of WPF applications

#### Cmdlet debug

It's possible in this way:

1. Open a *debug.PS1* and check `$OutputDllPath` and `$TargetProjPath`
2. Set *WPFLocales.Powershell* as StartUp project
3. Set Start Action of *WPFLocales.Powershell* to Start external program with *<**path to powershell.exe*>
4. Set Command line orguments to  *<**path to debug.PS1*>
5. Call *<**target CmdLet*> (for example *Sync-Localization*) at the end of *debug.PS1*
