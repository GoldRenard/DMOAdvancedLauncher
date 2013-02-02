[Setup]
AppId={{F95215F2-3DC9-4B42-9DC8-3E46B448B055}
AppName=GDMO Advanced Launcher
AppVersion=2.0
AppPublisher=GoldRenard & DragonVs
DefaultDirName={pf}\GoldRenard\GDMOAdvancedLauncher
AppendDefaultDirName=no
DefaultGroupName=Digimon Masters Online
OutputDir=.\Setups\
OutputBaseFilename=GDMOLauncher_Setup_2.0.4780
SetupIconFile=..\AdvancedLauncher\app_icon.ico
Compression=lzma
SolidCompression=yes
DirExistsWarning=no
UninstallFilesDir={app}
WizardImageFile=wizardimage.bmp
WizardSmallImageFile=icon.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "Apps\dotNetFx40_Full_setup.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected 
Source: "Apps\apploc.msi"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall;
Source: "Apps\AppLoc.exe"; DestDir:"{win}\apppatch"; Flags: ignoreversion uninsneveruninstall; BeforeInstall: InstallAL('{tmp}\apploc.msi')
Source: "Res\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "..\AdvancedLauncher\bin\Release\AdvancedLauncher.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\AdvancedLauncher\bin\Release\AdvancedLauncher.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\AdvancedLauncher\bin\Release\DMOLibrary.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\AdvancedLauncher\bin\Release\HtmlAgilityPack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\AdvancedLauncher\bin\Release\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\AdvancedLauncher\bin\Release\Ookii.Dialogs.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion

[Run]
Filename: {tmp}\dotNetFx40_Full_setup.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected; StatusMsg: Microsoft Framework 4.0 is beïng installed. Please wait... 

[Icons]
Name: "{group}\Digimon Masters Online"; Filename: "{app}\AdvancedLauncher.exe"
Name: "{commondesktop}\Digimon Masters Online"; Filename: "{app}\AdvancedLauncher.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Digimon Masters Online"; Filename: "{app}\AdvancedLauncher.exe"; Tasks: quicklaunchicon

[Code]
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, release, serviceCount: cardinal;
    check45, success: boolean;
var reqNetVer : string;
begin
    // .NET 4.5 installs as update to .NET 4.0 Full
    if version = 'v4.5' then begin
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 uses additional value Release
    if check45 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= 378389);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

function IsRequiredDotNetDetected(): Boolean;  
begin
    result := IsDotNetDetected('v4\Full', 0);
end;
           
procedure InstallAL(FileName: String);
var
  installed:Boolean;
  ResultCode: Integer;
begin
  installed := False;
  if IsWin64 then
  begin
    if not installed then
      installed := RegKeyExists(HKLM64, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{394BE3D9-7F57-4638-A8D1-1D88671913B7}');
    if not installed then
      installed := RegKeyExists(HKCU64, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{394BE3D9-7F57-4638-A8D1-1D88671913B7}');
    if not installed then
      installed := RegKeyExists(HKLM32, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{394BE3D9-7F57-4638-A8D1-1D88671913B7}');
    if not installed then
      installed := RegKeyExists(HKCU32, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{394BE3D9-7F57-4638-A8D1-1D88671913B7}');
  end;
  if not installed then
    installed := RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{394BE3D9-7F57-4638-A8D1-1D88671913B7}');
  if not installed then
    installed := RegKeyExists(HKEY_CURRENT_USER, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{394BE3D9-7F57-4638-A8D1-1D88671913B7}');

  if not installed then
    begin
      WizardForm.FilenameLabel.Caption := 'Intalling AppLocale...'
      ShellExec('', 'msiexec', ExpandConstant('/I "' + FileName + '" /passive /qn'), '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
    end;
end;