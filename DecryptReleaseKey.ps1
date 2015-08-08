[Reflection.Assembly]::LoadWithPartialName("System.Security");

Get-Location;

function Encrypt-File($File, $EncryptedFile, $Salt, $Passphrase, $Init) {
Write-Host  test $Passphrase;
	$pass = [Text.Encoding]::UTF8.GetBytes($Passphrase);
	$salt = [Text.Encoding]::UTF8.GetBytes($Salt);
	$init = [Text.Encoding]::UTF8.GetBytes($Init);

	$r     = new-Object  System.Security.Cryptography.RijndaelManaged;
	$r.Key = (new-Object Security.Cryptography.PasswordDeriveBytes $pass, $salt, "SHA1", 5).GetBytes(32);
	$r.IV  = (new-Object Security.Cryptography.SHA1Managed).ComputeHash($init)[0..15];

	$c         = $r.CreateEncryptor();
    $inStream  = new-Object IO.FileStream($File, "Open");
    $outStream = new-Object IO.FileStream($EncryptedFile, "Create");
    $cs        = new-Object Security.Cryptography.CryptoStream $outStream, $c, "Write";

    $inStream.CopyTo($cs);

    $cs.Close();
    $inStream.Close();
    $outStream.Close();
    $r.Clear();
}

function Decrypt-File($EncryptedFile, $File, $Salt, $Passphrase, $Init) {
	$pass = [Text.Encoding]::UTF8.GetBytes($Passphrase);
	$salt = [Text.Encoding]::UTF8.GetBytes($Salt);
	$init = [Text.Encoding]::UTF8.GetBytes($Init);

	$r     = new-Object  System.Security.Cryptography.RijndaelManaged;
	$r.Key = (new-Object Security.Cryptography.PasswordDeriveBytes $pass, $salt, "SHA1", 5).GetBytes(32);
	$r.IV  = (new-Object Security.Cryptography.SHA1Managed).ComputeHash($init)[0..15];

	$c         = $r.CreateDecryptor();
    $inStream  = new-Object IO.FileStream($EncryptedFile, "Open");
    $outStream = new-Object IO.FileStream($File, "Create");
    $cs        = new-Object Security.Cryptography.CryptoStream $inStream, $c, "Read";

    $cs.CopyTo($outStream);

    $cs.Close();
    $inStream.Close();
    $outStream.Close();
    $r.Clear();
}

$inSalt       = $env:SNK_SEC_SALT;
$inPassphrase = $env:SNK_SEC_PASSPHRASE;
$inInit       = $env:SNK_SEC_INIT;

#$inSalt       = "mysalt";
#$inPassphrase = "mypassphrase";
#$inInit       = "mysecinit";

if ([string]::IsNullOrEmpty($inSalt) -or [string]::IsNullOrEmpty($inSalt) -or [string]::IsNullOrEmpty($inSalt)) {
    Write-Host "Unknown encryption data, skipping release key decryption...";
    exit;
}

Decrypt-File "C:\Projects\DMOAdvancedLauncher\signkey.release.enc" "C:\Projects\DMOAdvancedLauncher\signkey.snk" $inSalt $inPassphrase $inInit;

$md5 = New-Object -TypeName System.Security.Cryptography.MD5CryptoServiceProvider;
$hash = [System.BitConverter]::ToString($md5.ComputeHash([System.IO.File]::ReadAllBytes("C:\Projects\DMOAdvancedLauncher\signkey.snk")));
Write-Host Decrypted MD5: $hash;