#!/bin/bash

echo "========================================"
echo "  Base API - Setup de Desenvolvimento"
echo "========================================"
echo ""

# Verificar se o dotnet está instalado
if ! command -v dotnet &> /dev/null; then
    echo "ERRO: .NET SDK não encontrado!"
    echo "Instale o .NET 8 SDK: https://dotnet.microsoft.com/download"
    exit 1
fi

echo "Configuração de User Secrets para a Connection String do PostgreSQL"
echo ""
echo "Exemplo de Connection String:"
echo "Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=sua_senha"
echo ""

# Solicitar a connection string
read -p "Digite a Connection String do PostgreSQL: " connectionString

if [ -z "$connectionString" ]; then
    echo ""
    echo "ERRO: Connection String não pode estar vazia!"
    exit 1
fi

echo ""
echo "Configurando User Secrets..."

# Navegar para o diretório da API
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
API_PROJECT_PATH="$SCRIPT_DIR/src/BaseApi.Api"

if [ ! -d "$API_PROJECT_PATH" ]; then
    echo "ERRO: Projeto da API não encontrado em: $API_PROJECT_PATH"
    exit 1
fi

# Configurar o User Secret
cd "$API_PROJECT_PATH"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$connectionString"

if [ $? -eq 0 ]; then
    echo ""
    echo "========================================"
    echo "  Configuração concluída com sucesso!"
    echo "========================================"
    echo ""
    echo "Próximos passos:"
    echo "  1. dotnet restore (Restaurar dependências)"
    echo "  2. dotnet ef migrations add InitialCreate --project ../BaseApi.Infra (Criar migration)"
    echo "  3. dotnet ef database update --project ../BaseApi.Infra (Aplicar migration)"
    echo "  4. dotnet run (Executar a API)"
    echo ""
    echo "A API estará disponível em: https://localhost:7000 (Swagger na raiz)"
else
    echo ""
    echo "ERRO: Falha ao configurar User Secrets!"
    exit 1
fi

cd "$SCRIPT_DIR"
