#!/usr/bin/env pwsh

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Base API - Configuração JWT" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se o dotnet está instalado
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "ERRO: .NET SDK não encontrado!" -ForegroundColor Red
    Write-Host "Instale o .NET 8 SDK: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Navegar para o diretório da API
$apiProjectPath = Join-Path $PSScriptRoot "src\BaseApi.Api"

if (-not (Test-Path $apiProjectPath)) {
    Write-Host "ERRO: Projeto da API não encontrado em: $apiProjectPath" -ForegroundColor Red
    exit 1
}

Set-Location $apiProjectPath

Write-Host "Configurando JWT Secrets..." -ForegroundColor Yellow
Write-Host ""

# Gerar um secret aleatório seguro
$jwtSecret = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})

Write-Host "1. JWT Secret: [GERADO AUTOMATICAMENTE - 64 caracteres]" -ForegroundColor Green
Write-Host "2. JWT Issuer (padrão: BaseApi): " -NoNewline -ForegroundColor Yellow
$jwtIssuer = Read-Host
if ([string]::IsNullOrWhiteSpace($jwtIssuer)) {
    $jwtIssuer = "BaseApi"
}

Write-Host "3. JWT Audience (padrão: BaseApiUsers): " -NoNewline -ForegroundColor Yellow
$jwtAudience = Read-Host
if ([string]::IsNullOrWhiteSpace($jwtAudience)) {
    $jwtAudience = "BaseApiUsers"
}

Write-Host "4. JWT Expiração em minutos (padrão: 60): " -NoNewline -ForegroundColor Yellow
$jwtExpireMinutes = Read-Host
if ([string]::IsNullOrWhiteSpace($jwtExpireMinutes)) {
    $jwtExpireMinutes = "60"
}

Write-Host ""
Write-Host "Aplicando configurações..." -ForegroundColor Yellow

try {
    # Configurar JWT Secrets
    dotnet user-secrets set "Jwt:Secret" $jwtSecret
    dotnet user-secrets set "Jwt:Issuer" $jwtIssuer
    dotnet user-secrets set "Jwt:Audience" $jwtAudience
    dotnet user-secrets set "Jwt:ExpireMinutes" $jwtExpireMinutes

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  JWT configurado com sucesso!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Configurações aplicadas:" -ForegroundColor Cyan
        Write-Host "  - JWT Secret: ******* (64 caracteres)" -ForegroundColor White
        Write-Host "  - JWT Issuer: $jwtIssuer" -ForegroundColor White
        Write-Host "  - JWT Audience: $jwtAudience" -ForegroundColor White
        Write-Host "  - JWT Expiração: $jwtExpireMinutes minutos" -ForegroundColor White
        Write-Host ""
        Write-Host "Para verificar todos os secrets:" -ForegroundColor Yellow
        Write-Host "  dotnet user-secrets list" -ForegroundColor Gray
    }
    else {
        Write-Host ""
        Write-Host "ERRO: Falha ao configurar JWT Secrets!" -ForegroundColor Red
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
