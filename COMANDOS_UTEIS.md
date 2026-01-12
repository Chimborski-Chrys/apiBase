# Comandos Úteis - Base API

## Desenvolvimento Local

### Iniciar o PostgreSQL via Docker
```bash
docker-compose up -d
```

### Parar o PostgreSQL
```bash
docker-compose down
```

### Verificar logs do PostgreSQL
```bash
docker-compose logs -f postgres
```

## User Secrets

### Configurar Connection String
```bash
cd src/BaseApi.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=postgres"
```

### Listar todos os secrets
```bash
dotnet user-secrets list
```

### Remover um secret
```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

### Limpar todos os secrets
```bash
dotnet user-secrets clear
```

## Entity Framework Migrations

### Criar uma nova migration
```bash
cd src/BaseApi.Api
dotnet ef migrations add NomeDaMigration --project ../BaseApi.Infra
```

### Aplicar migrations ao banco
```bash
dotnet ef database update --project ../BaseApi.Infra
```

### Reverter a última migration
```bash
dotnet ef migrations remove --project ../BaseApi.Infra
```

### Listar migrations
```bash
dotnet ef migrations list --project ../BaseApi.Infra
```

### Gerar script SQL da migration
```bash
dotnet ef migrations script --project ../BaseApi.Infra --output migration.sql
```

### Reverter para uma migration específica
```bash
dotnet ef database update NomeDaMigration --project ../BaseApi.Infra
```

### Remover banco de dados
```bash
dotnet ef database drop --project ../BaseApi.Infra
```

## Build e Execução

### Restaurar dependências
```bash
dotnet restore
```

### Compilar a solução
```bash
dotnet build
```

### Executar a API
```bash
cd src/BaseApi.Api
dotnet run
```

### Executar em modo watch (hot reload)
```bash
cd src/BaseApi.Api
dotnet watch run
```

### Publicar para produção
```bash
dotnet publish -c Release -o ./publish
```

## Testes

### Executar todos os testes
```bash
dotnet test
```

### Executar testes com cobertura
```bash
dotnet test /p:CollectCoverage=true
```

## NuGet

### Adicionar pacote
```bash
dotnet add package NomeDoPacote
```

### Remover pacote
```bash
dotnet remove package NomeDoPacote
```

### Atualizar pacote
```bash
dotnet add package NomeDoPacote --version x.x.x
```

### Listar pacotes outdated
```bash
dotnet list package --outdated
```

## Git

### Inicializar repositório
```bash
git init
git add .
git commit -m "Initial commit"
```

### Verificar status
```bash
git status
```

### Criar branch
```bash
git checkout -b feature/nova-funcionalidade
```

## Docker

### Build da imagem
```bash
docker build -t baseapi:latest .
```

### Executar container
```bash
docker run -p 8080:80 baseapi:latest
```

## Limpeza

### Limpar binários
```bash
dotnet clean
```

### Remover pastas bin e obj
```bash
# PowerShell
Get-ChildItem -Include bin,obj -Recurse | Remove-Item -Recurse -Force

# Bash
find . -name "bin" -o -name "obj" | xargs rm -rf
```

## Verificação de Código

### Formatar código
```bash
dotnet format
```

### Analisar código
```bash
dotnet build /p:TreatWarningsAsErrors=true
```

## Banco de Dados (PostgreSQL)

### Conectar ao banco via psql
```bash
docker exec -it baseapi-postgres psql -U postgres -d baseapi_dev
```

### Comandos úteis do psql
```sql
-- Listar bancos de dados
\l

-- Conectar a um banco
\c baseapi_dev

-- Listar tabelas
\dt

-- Descrever tabela
\d "Users"

-- Listar usuários
SELECT * FROM "Users";

-- Sair
\q
```

### Backup do banco
```bash
docker exec baseapi-postgres pg_dump -U postgres baseapi_dev > backup.sql
```

### Restaurar backup
```bash
docker exec -i baseapi-postgres psql -U postgres baseapi_dev < backup.sql
```

## Performance

### Executar com profile
```bash
dotnet run --configuration Release --launch-profile https
```

### Analisar bundle size
```bash
dotnet publish -c Release --self-contained false /p:PublishTrimmed=true
```

## Logs

### Visualizar logs da aplicação
```bash
dotnet run --verbosity detailed
```

### Logs do Docker
```bash
docker logs baseapi-postgres -f
```

## Versionamento

### Atualizar versão do projeto
Edite o arquivo `.csproj`:
```xml
<PropertyGroup>
  <Version>1.0.0</Version>
</PropertyGroup>
```

## Scripts Personalizados

### Executar setup
```bash
# PowerShell
.\setup-dev.ps1

# Bash
chmod +x setup-dev.sh
./setup-dev.sh
```

---

**Dica:** Adicione esses comandos como aliases no seu terminal para agilizar o desenvolvimento!
