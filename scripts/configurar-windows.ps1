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

$rootPasswordSecure = Read-Host 'Contraseña actual de root de MySQL' -AsSecureString
$rootPasswordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($rootPasswordSecure)
$rootPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($rootPasswordPointer)
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($rootPasswordPointer)

$sql = @"
CREATE DATABASE IF NOT EXISTS utngolcoin_db
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
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
}

$escapedRootPassword = $rootPassword.Replace("'", "''")
$connectionString = "server=127.0.0.1;port=3306;database=utngolcoin_db;user=root;password='$escapedRootPassword'"
$guacalesUrl = "http://${AndreaIp}:18080/demo/api/v1/"

dotnet user-secrets set 'ConnectionStrings:Default' $connectionString --project $project
dotnet user-secrets set 'ServicioEstadisticas:BaseUrl' $guacalesUrl --project $project
$rootPassword = $null
$escapedRootPassword = $null

Write-Host ''
Write-Host 'Configuración lista. Abre UTNGolCoinApi.slnx y ejecuta con F5.'
Write-Host "Guacales: $guacalesUrl"
Write-Host 'UTNGolCoin escuchará en http://0.0.0.0:5001'
