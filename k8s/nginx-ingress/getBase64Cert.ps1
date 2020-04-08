Param(
    [parameter(Mandatory=$false)][string]$crtPath="ssl-cert\localhost.crt",
    [parameter(Mandatory=$false)][string]$keyPath="ssl-cert\localhost.key"
)


function base64Encode($text) {
    $bytes = [System.Text.Encoding]::Unicode.GetBytes($text);
    $encodedText =[Convert]::ToBase64String($bytes);

    $encodedText;
}

$certText = cat $crtPath;
$keyText = cat $keyPath;

Write-Host "Base64 EnCoded Cert:"
base64Encode($certText);

Write-Host "Base64 Encoded Key:"
base64Encode($keyText);