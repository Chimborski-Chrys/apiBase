# Setup completo da Base API
# Configura Connection String e JWT em uma unica etapa

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Base API - Setup Completo" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se o dotnet esta instalado
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "ERRO: .NET SDK nao encontrado!" -ForegroundColor Red
    Write-Host "Instale o .NET 8 SDK: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Navegar para o diretorio da API
$apiProjectPath = Join-Path $PSScriptRoot "src\BaseApi.Api"

if (-not (Test-Path $apiProjectPath)) {
    Write-Host "ERRO: Projeto da API nao encontrado em: $apiProjectPath" -ForegroundColor Red
    exit 1
}

Set-Location $apiProjectPath

Write-Host "=======================================" -ForegroundColor Cyan
Write-Host " PASSO 1: Connection String PostgreSQL" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Exemplo:" -ForegroundColor Yellow
Write-Host "Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=postgres" -ForegroundColor Gray
Write-Host ""
$connectionString = Read-Host "Digite a Connection String"

if ([string]::IsNullOrWhiteSpace($connectionString)) {
    Write-Host ""
    Write-Host "ERRO: Connection String nao pode estar vazia!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host " PASSO 2: Configuracoes JWT" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

# Gerar JWT Secret automaticamente
$jwtSecret = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
Write-Host "[OK] JWT Secret gerado automaticamente (64 caracteres)" -ForegroundColor Green

Write-Host ""
Write-Host "JWT Issuer (pressione Enter para usar 'BaseApi'): " -NoNewline -ForegroundColor Yellow
$jwtIssuer = Read-Host
if ([string]::IsNullOrWhiteSpace($jwtIssuer)) {
    $jwtIssuer = "BaseApi"
}

Write-Host "JWT Audience (pressione Enter para usar 'BaseApiUsers'): " -NoNewline -ForegroundColor Yellow
$jwtAudience = Read-Host
if ([string]::IsNullOrWhiteSpace($jwtAudience)) {
    $jwtAudience = "BaseApiUsers"
}

Write-Host "JWT Expiracao em minutos (pressione Enter para usar '1440' = 24h): " -NoNewline -ForegroundColor Yellow
$jwtExpireMinutes = Read-Host
if ([string]::IsNullOrWhiteSpace($jwtExpireMinutes)) {
    $jwtExpireMinutes = "1440"
}

Write-Host ""
Write-Host "Aplicando todas as configuracoes..." -ForegroundColor Yellow

try {
    # Configurar Connection String
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString | Out-Null

    # Configurar JWT
    dotnet user-secrets set "Jwt:Secret" $jwtSecret | Out-Null
    dotnet user-secrets set "Jwt:Issuer" $jwtIssuer | Out-Null
    dotnet user-secrets set "Jwt:Audience" $jwtAudience | Out-Null
    dotnet user-secrets set "Jwt:ExpireMinutes" $jwtExpireMinutes | Out-Null

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  [OK] Configuracao concluida!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Configuracoes aplicadas:" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "PostgreSQL:" -ForegroundColor Yellow
        Write-Host "  [OK] Connection String configurada" -ForegroundColor White
        Write-Host ""
        Write-Host "JWT Authentication:" -ForegroundColor Yellow
        Write-Host "  [OK] Secret: ******* (64 caracteres)" -ForegroundColor White
        Write-Host "  [OK] Issuer: $jwtIssuer" -ForegroundColor White
        Write-Host "  [OK] Audience: $jwtAudience" -ForegroundColor White
        Write-Host "  [OK] Expiracao: $jwtExpireMinutes minutos" -ForegroundColor White
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "Proximos passos:" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "  1. Execute a aplicacao no Visual Studio (F5)" -ForegroundColor White
        Write-Host "  2. Abra o Swagger em https://localhost:7000" -ForegroundColor White
        Write-Host "  3. Use /api/auth/register para criar um usuario" -ForegroundColor White
        Write-Host "  4. Use /api/auth/login para obter o token JWT" -ForegroundColor White
        Write-Host "  5. Clique em 'Authorize' e cole o token" -ForegroundColor White
        Write-Host "  6. Agora voce pode editar/deletar usuarios!" -ForegroundColor White
        Write-Host ""
        Write-Host "Para verificar todos os secrets:" -ForegroundColor Yellow
        Write-Host "  dotnet user-secrets list" -ForegroundColor Gray
        Write-Host ""
    }
    else {
        Write-Host ""
        Write-Host "ERRO: Falha ao configurar secrets!" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host ""
    Write-Host "ERRO: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    Set-Location $PSScriptRoot
}
