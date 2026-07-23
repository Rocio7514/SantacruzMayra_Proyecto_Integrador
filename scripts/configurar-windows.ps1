param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z0-9.-]+$')]
    [string]$AndreaIp
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\UTNGolCoinApi.csproj'

if (-not (Get-Command mysql -ErrorAction SilentlyContinue)) {
    throw 'No se encontró mysql.exe en PATH. Agrega la carpeta bin de MySQL y vuelve a ejecutar.'
}

$appPasswordSecure = Read-Host 'Nueva contraseña del usuario utngolcoin' -AsSecureString
$appPasswordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($appPasswordSecure)
$appPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($appPasswordPointer)
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($appPasswordPointer)

if ($appPassword -notmatch '^[A-Za-z0-9._-]{8,64}$') {
    throw 'La contraseña debe tener 8-64 caracteres: letras, números, punto, guion o guion bajo.'
}

$rootPasswordSecure = Read-Host 'Contraseña actual de root de MySQL' -AsSecureString
$rootPasswordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($rootPasswordSecure)
$rootPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($rootPasswordPointer)
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($rootPasswordPointer)

$sql = @"
CREATE DATABASE IF NOT EXISTS utngolcoin_db
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER IF NOT EXISTS 'utngolcoin'@'127.0.0.1' IDENTIFIED BY '$appPassword';
ALTER USER 'utngolcoin'@'127.0.0.1' IDENTIFIED BY '$appPassword';
GRANT ALL PRIVILEGES ON utngolcoin_db.* TO 'utngolcoin'@'127.0.0.1';
FLUSH PRIVILEGES;
"@

try {
    $env:MYSQL_PWD = $rootPassword
    $sql | mysql --host=127.0.0.1 --user=root
    if ($LASTEXITCODE -ne 0) {
        throw "MySQL terminó con código $LASTEXITCODE."
    }
}
finally {
    Remove-Item Env:MYSQL_PWD -ErrorAction SilentlyContinue
    $rootPassword = $null
}

$connectionString = "server=127.0.0.1;port=3306;database=utngolcoin_db;user=utngolcoin;password=$appPassword"
$guacalesUrl = "http://${AndreaIp}:18080/demo/api/v1/"

dotnet user-secrets set 'ConnectionStrings:Default' $connectionString --project $project
dotnet user-secrets set 'ServicioEstadisticas:BaseUrl' $guacalesUrl --project $project

$appPassword = $null

Write-Host ''
Write-Host 'Configuración lista. Abre UTNGolCoinApi.slnx y ejecuta con F5.'
Write-Host "Guacales: $guacalesUrl"
Write-Host 'UTNGolCoin escuchará en http://0.0.0.0:5001'
