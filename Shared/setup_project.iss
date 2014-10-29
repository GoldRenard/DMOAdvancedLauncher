[Setup]
AppId={{F95215F2-3DC9-4B42-9DC8-3E46B448B055}
AppName=DMO Advanced Launcher
AppVersion=2.3a
AppPublisher=GoldRenard & DragonVs
DefaultDirName={pf}\GoldRenard\DMOAdvancedLauncher
AppendDefaultDirName=no
DefaultGroupName=Digimon Masters Online
OutputDir=.\Setups\
OutputBaseFilename=DMOLauncher_Setup_2.2.4981.21424
SetupIconFile=..\AdvancedLauncher\app_icon.ico
Compression=lzma
SolidCompression=yes
DirExistsWarning=no
LicenseFile=LICENSE.txt
UninstallFilesDir={app}
WizardImageFile=wizardimage.bmp
WizardSmallImageFile=icon.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\AdvancedLauncher\bin\Release\*"; DestDir: "{app}"; Excludes: "*.pdb,\Databases\*,\Configs\*,*.vshost.*,*.xml"; Flags: ignoreversion recursesubdirs
Source: "Apps\apploc.msi"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall;
Source: "Apps\AppLoc.exe"; DestDir:"{win}\apppatch"; Flags: ignoreversion uninsneveruninstall; BeforeInstall: InstallAL('{tmp}\apploc.msi')
Source: "Apps\vcredist_x86.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall;
Source: "Apps\dotNetFx40_Full_setup.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall; Check: not CheckForNetFx4 

[InstallDelete]
Type: files; Name: "{app}\Databases\*.sqlite"

[Run]
Filename: {tmp}\dotNetFx40_Full_setup.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not CheckForNetFx4; StatusMsg: Microsoft Framework 4.0 is being installed. Please wait... 
Filename: {tmp}\vcredist_x86.exe; Parameters: "/q"; StatusMsg: Microsoft Visual C++ 2010 SP1 is being installed. Please wait...
;Filename: {code:GetNetFx4InstallRoot|Ngen.exe}; Parameters: "install ""{app}\System.Data.SQLite.dll"" /nologo"; Flags: skipifdoesntexist; StatusMsg: Registering SQLite...

[UninstallRun]
;Filename: {code:GetNetFx4InstallRoot|Ngen.exe}; Parameters: "uninstall ""{app}\System.Data.SQLite.dll"" /nologo"; Flags: skipifdoesntexist;

[Icons]
Name: "{group}\Digimon Masters Online"; Filename: "{app}\AdvancedLauncher.exe";
Name: "{commondesktop}\Digimon Masters Online"; Filename: "{app}\AdvancedLauncher.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Digimon Masters Online"; Filename: "{app}\AdvancedLauncher.exe"; Tasks: quicklaunchicon

[Code]
function TrimSlash(const Path: String): String;
var
  LastCharacter: String;
begin
  Result := Path;
  if Result <> '' then
  begin
    LastCharacter := Copy(Result, Length(Result), 1);
    if (LastCharacter = '\') or (LastCharacter = '/') then
      Result := Copy(Result, 1, Length(Result) - 1);
  end;
end;

function CheckForNetFx4(): boolean;
var
    key: string;
    install : cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';
    success := RegQueryDWordValue(HKLM, key, 'Install', install);
    result := success and (install = 1);
end;

function GetNetFx4InstallRoot(const FileName: String): String;
var
  InstallRoot: String;
begin
  Result := '';
  if RegQueryStringValue(HKEY_LOCAL_MACHINE, 'Software\Microsoft\.NETFramework', 'InstallRoot', InstallRoot) then
  begin
    if InstallRoot <> '' then
    begin
      Result := TrimSlash(InstallRoot) + '\v4.0.30319';
      if FileName <> '' then
      begin
        Result := TrimSlash(Result) + '\' + FileName;
      end;
    end;
  end;
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
      WizardForm.FilenameLabel.Caption := 'Microsoft AppLocale is being installed. Please wait...'
      ShellExec('', 'msiexec', ExpandConstant('/I "' + FileName + '" /passive /qn'), '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
    end;
end;