#!/usr/bin/env pwsh

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Base API - Setup de Desenvolvimento" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se o dotnet está instalado
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "ERRO: .NET SDK não encontrado!" -ForegroundColor Red
    Write-Host "Instale o .NET 8 SDK: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

Write-Host "Configuração de User Secrets para a Connection String do PostgreSQL" -ForegroundColor Green
Write-Host ""
Write-Host "Exemplo de Connection String:" -ForegroundColor Yellow
Write-Host "Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=sua_senha" -ForegroundColor Gray
Write-Host ""

# Solicitar a connection string
$connectionString = Read-Host "Digite a Connection String do PostgreSQL"

if ([string]::IsNullOrWhiteSpace($connectionString)) {
    Write-Host ""
    Write-Host "ERRO: Connection String não pode estar vazia!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Configurando User Secrets..." -ForegroundColor Yellow

# Navegar para o diretório da API
$apiProjectPath = Join-Path $PSScriptRoot "src\BaseApi.Api"

if (-not (Test-Path $apiProjectPath)) {
    Write-Host "ERRO: Projeto da API não encontrado em: $apiProjectPath" -ForegroundColor Red
    exit 1
}

# Configurar o User Secret
try {
    Set-Location $apiProjectPath
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  Configuração concluída com sucesso!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Próximos passos:" -ForegroundColor Cyan
        Write-Host "  1. dotnet restore (Restaurar dependências)" -ForegroundColor White
        Write-Host "  2. dotnet ef migrations add InitialCreate --project ../BaseApi.Infra (Criar migration)" -ForegroundColor White
        Write-Host "  3. dotnet ef database update --project ../BaseApi.Infra (Aplicar migration)" -ForegroundColor White
        Write-Host "  4. dotnet run (Executar a API)" -ForegroundColor White
        Write-Host ""
        Write-Host "A API estará disponível em: https://localhost:7000 (Swagger na raiz)" -ForegroundColor Yellow
    }
    else {
        Write-Host ""
        Write-Host "ERRO: Falha ao configurar User Secrets!" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host ""
    Write-Host "ERRO: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    # Voltar ao diretório original
    Set-Location $PSScriptRoot
}
