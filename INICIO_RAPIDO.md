# Início Rápido - Base API

Guia rápido para colocar a API funcionando em 5 minutos.

## Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- [ ] [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [ ] [Docker Desktop](https://www.docker.com/products/docker-desktop/) (ou PostgreSQL instalado)
- [ ] Git

## Opção 1: Setup Completo com Docker (Recomendado)

### Passo 1: Subir o PostgreSQL

```bash
docker-compose up -d
```

Este comando irá:
- Baixar a imagem do PostgreSQL 16 Alpine
- Criar um container chamado `baseapi-postgres`
- Expor a porta 5432
- Criar o banco `baseapi_dev`
- Configurar usuário `postgres` e senha `postgres`

### Passo 2: Configurar User Secrets

**Windows (PowerShell):**
```powershell
.\setup-dev.ps1
```

**Linux/Mac (Bash):**
```bash
chmod +x setup-dev.sh
./setup-dev.sh
```

Quando solicitado, use esta connection string:
```
Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=postgres
```

### Passo 3: Aplicar Migrations

```bash
cd src/BaseApi.Api
dotnet ef migrations add InitialCreate --project ../BaseApi.Infra
dotnet ef database update --project ../BaseApi.Infra
```

### Passo 4: Executar a API

```bash
dotnet run
```

Acesse: **https://localhost:7000**

## Opção 2: Setup com PostgreSQL Existente

### Passo 1: Ter um PostgreSQL rodando

Certifique-se de que você tem um PostgreSQL em execução e crie um banco:

```sql
CREATE DATABASE baseapi_dev;
```

### Passo 2: Configurar User Secrets Manualmente

```bash
cd src/BaseApi.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=SEU_HOST;Port=PORTA;Database=NOME_DB;Username=USUARIO;Password=SENHA"
```

Exemplo:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=minhasenha"
```

### Passo 3: Aplicar Migrations

```bash
dotnet ef migrations add InitialCreate --project ../BaseApi.Infra
dotnet ef database update --project ../BaseApi.Infra
```

### Passo 4: Executar a API

```bash
dotnet run
```

## Verificando se Funcionou

### 1. Swagger UI

Abra o navegador em: **https://localhost:7000**

Você deverá ver a interface do Swagger com os endpoints disponíveis.

### 2. Testando o Endpoint

**GET /api/users** - Lista usuários

```bash
curl https://localhost:7000/api/users
```

Resposta esperada:
```json
[]
```

**POST /api/users** - Criar usuário

```bash
curl -X POST https://localhost:7000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@example.com",
    "passwordHash": "hash_temporario",
    "role": "Admin",
    "isActive": true
  }'
```

### 3. Verificando o Banco de Dados

```bash
docker exec -it baseapi-postgres psql -U postgres -d baseapi_dev
```

Dentro do psql:
```sql
-- Listar tabelas
\dt

-- Ver usuários
SELECT * FROM "Users";

-- Sair
\q
```

## Comandos Úteis do Dia a Dia

### Parar o PostgreSQL
```bash
docker-compose down
```

### Reiniciar o PostgreSQL
```bash
docker-compose restart
```

### Ver logs do PostgreSQL
```bash
docker-compose logs -f postgres
```

### Executar com Hot Reload
```bash
cd src/BaseApi.Api
dotnet watch run
```

### Limpar e Recompilar
```bash
dotnet clean
dotnet build
```

### Listar User Secrets Configurados
```bash
cd src/BaseApi.Api
dotnet user-secrets list
```

## Troubleshooting

### Erro: "Connection string 'DefaultConnection' not found"

**Solução:** Execute o script de setup ou configure manualmente:
```bash
cd src/BaseApi.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "sua-connection-string"
```

### Erro: "Cannot connect to PostgreSQL"

**Verificações:**
1. PostgreSQL está rodando?
   ```bash
   docker ps | grep postgres
   ```

2. Porta 5432 está acessível?
   ```bash
   telnet localhost 5432
   ```

3. Connection string está correta?
   ```bash
   dotnet user-secrets list
   ```

### Erro ao aplicar migrations

**Solução:** Certifique-se de estar no diretório correto:
```bash
cd src/BaseApi.Api
dotnet ef database update --project ../BaseApi.Infra
```

### Porta 7000 já em uso

**Solução:** Edite `src/BaseApi.Api/Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:OUTRA_PORTA;http://localhost:5000"
```

## Estrutura Criada

```
✅ Clean Architecture (Domain, Application, Infra, API)
✅ PostgreSQL configurado com EF Core
✅ User Secrets para segurança
✅ CRUD completo de Users
✅ Swagger UI
✅ Docker Compose
✅ Repository Pattern
✅ Soft Delete
✅ Timestamps automáticos
```

## Próximos Passos

Agora que a API está funcionando, você pode:

1. **Adicionar autenticação JWT**
   - Ver `COMANDOS_UTEIS.md` para exemplos

2. **Implementar validação**
   - Adicionar FluentValidation
   - Criar DTOs

3. **Adicionar testes**
   - Criar projeto de testes
   - Implementar testes unitários

4. **Melhorar segurança**
   - Hash de senhas com BCrypt
   - Rate limiting

5. **Expandir funcionalidades**
   - Novos endpoints
   - Novas entidades

## Recursos Adicionais

- 📖 **README.md** - Documentação completa
- 📋 **COMANDOS_UTEIS.md** - Comandos do dia a dia
- 🏗️ **ESTRUTURA_PROJETO.md** - Detalhes da arquitetura
- 🐳 **docker-compose.yml** - Setup do PostgreSQL
- 🔧 **setup-dev.ps1/sh** - Automação de setup

## Suporte

Para questões sobre:
- **.NET**: https://docs.microsoft.com/dotnet
- **Entity Framework**: https://docs.microsoft.com/ef/core
- **PostgreSQL**: https://www.postgresql.org/docs

---

**Pronto para desenvolver! 🚀**
