#!/usr/bin/env pwsh

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Diagnóstico da API" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000"

# Teste 1: Swagger UI
Write-Host "1. Testando Swagger UI..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method Get -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "   ✓ Swagger UI está acessível" -ForegroundColor Green
    }
} catch {
    Write-Host "   ✗ Swagger UI não está acessível" -ForegroundColor Red
    Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Teste 2: Swagger JSON
Write-Host "2. Testando Swagger JSON..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/swagger/v1/swagger.json" -Method Get -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "   ✓ Swagger JSON está acessível" -ForegroundColor Green
    }
} catch {
    Write-Host "   ✗ Swagger JSON não está acessível" -ForegroundColor Red
    Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Teste 3: Endpoint de API
Write-Host "3. Testando endpoint /api/users..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/users" -Method Get -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "   ✓ Endpoint /api/users está funcionando" -ForegroundColor Green
        Write-Host "   Resposta: $($response.Content)" -ForegroundColor Gray
    }
} catch {
    Write-Host "   ✗ Endpoint /api/users não está acessível" -ForegroundColor Red
    Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Diagnóstico concluído!" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
