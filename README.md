# Base API

Template base para desenvolvimento de APIs utilizando .NET 8, PostgreSQL e Clean Architecture.

## Arquitetura

Este projeto segue os princípios de **Clean Architecture**, organizando o código em camadas bem definidas:

```
src/
├── BaseApi.Domain/          # Entidades, Enums e Regras de Negócio
├── BaseApi.Application/     # Casos de Uso e Interfaces
├── BaseApi.Infra/          # Implementações de Infraestrutura (DbContext, Repositories)
└── BaseApi.Api/            # Controllers, Endpoints e Configurações
```

## Stack Tecnológica

- **.NET 8** (C#)
- **PostgreSQL** (via Entity Framework Core)
- **Npgsql** (Driver PostgreSQL para .NET)
- **Swagger/OpenAPI** (Documentação automática)
- **Clean Architecture** (Separação de responsabilidades)

## Segurança

A Connection String do banco de dados **NÃO está armazenada no código** ou no `appsettings.json` por questões de segurança.

Utilizamos o recurso **User Secrets** do .NET, que armazena informações sensíveis de forma segura fora do repositório Git.

## Como Rodar pela Primeira Vez

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (versão 12 ou superior)
- Um banco de dados PostgreSQL criado

### 1. Configure a Connection String

Execute o script de setup de acordo com seu sistema operacional:

**Windows (PowerShell):**
```powershell
.\setup-dev.ps1
```

**Linux/macOS (Bash):**
```bash
chmod +x setup-dev.sh
./setup-dev.sh
```

O script irá solicitar a Connection String do PostgreSQL. Exemplo:

```
Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=sua_senha
```

### 2. Restaure as Dependências

```bash
dotnet restore
```

### 3. Crie e Aplique as Migrations

Navegue até o projeto da API:

```bash
cd src/BaseApi.Api
```

Crie a migration inicial:

```bash
dotnet ef migrations add InitialCreate --project ../BaseApi.Infra
```

Aplique a migration ao banco de dados:

```bash
dotnet ef database update --project ../BaseApi.Infra
```

### 4. Execute a API

```bash
dotnet run
```

A API estará disponível em:
- **Swagger UI:** https://localhost:7000
- **HTTP:** http://localhost:5000

## Estrutura do Banco de Dados

### Tabela: Users

| Coluna       | Tipo      | Descrição                    |
|--------------|-----------|------------------------------|
| Id           | UUID      | Identificador único          |
| Name         | string    | Nome do usuário              |
| Email        | string    | Email (único)                |
| PasswordHash | string    | Hash da senha                |
| Role         | string    | Admin ou User                |
| IsActive     | bool      | Status de ativação           |
| CreatedAt    | datetime  | Data de criação              |
| UpdatedAt    | datetime  | Data de atualização          |

## Endpoints Disponíveis

### Users

- `GET /api/users` - Lista todos os usuários ativos
- `GET /api/users/{id}` - Busca um usuário por ID
- `POST /api/users` - Cria um novo usuário
- `PUT /api/users/{id}` - Atualiza um usuário
- `DELETE /api/users/{id}` - Desativa um usuário (soft delete)

## Configuração Manual de User Secrets

Se preferir configurar manualmente (sem o script):

```bash
cd src/BaseApi.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=sua_senha"
```

Para listar os secrets configurados:

```bash
dotnet user-secrets list
```

Para remover um secret:

```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

## Exemplo de Connection String PostgreSQL

```
Host=localhost;Port=5432;Database=nome_do_banco;Username=seu_usuario;Password=sua_senha
```

### Variações Comuns:

**PostgreSQL Local:**
```
Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=postgres
```

**PostgreSQL com SSL:**
```
Host=seu-servidor.com;Port=5432;Database=baseapi_prod;Username=user;Password=pass;SSL Mode=Require
```

**PostgreSQL em Container Docker:**
```
Host=localhost;Port=5433;Database=baseapi_dev;Username=postgres;Password=postgres
```

## Migrations do Entity Framework

### Criar uma nova migration:
```bash
dotnet ef migrations add NomeDaMigration --project src/BaseApi.Infra --startup-project src/BaseApi.Api
```

### Aplicar migrations pendentes:
```bash
dotnet ef database update --project src/BaseApi.Infra --startup-project src/BaseApi.Api
```

### Reverter a última migration:
```bash
dotnet ef migrations remove --project src/BaseApi.Infra --startup-project src/BaseApi.Api
```

### Listar todas as migrations:
```bash
dotnet ef migrations list --project src/BaseApi.Infra --startup-project src/BaseApi.Api
```

## Boas Práticas de Segurança

1. **NUNCA** commite a Connection String no Git
2. **NUNCA** coloque senhas no `appsettings.json`
3. Use **User Secrets** para desenvolvimento local
4. Use **Azure Key Vault**, **AWS Secrets Manager** ou equivalente em produção
5. Sempre use variáveis de ambiente em ambientes de CI/CD
6. Mantenha o `.gitignore` atualizado para evitar commits acidentais

## Estrutura de Arquivos

```
baseApi/
├── src/
│   ├── BaseApi.Domain/
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   └── User.cs
│   │   └── Enums/
│   │       └── UserRole.cs
│   ├── BaseApi.Application/
│   │   ├── Interfaces/
│   │   └── Services/
│   ├── BaseApi.Infra/
│   │   ├── Data/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Repositories/
│   │   └── DependencyInjection.cs
│   └── BaseApi.Api/
│       ├── Controllers/
│       │   └── UsersController.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
├── setup-dev.ps1
├── setup-dev.sh
├── .gitignore
└── README.md
```

## Próximos Passos Sugeridos

- Implementar autenticação JWT
- Adicionar validação de dados (FluentValidation)
- Implementar padrão Repository
- Adicionar testes unitários e de integração
- Configurar logging (Serilog)
- Implementar rate limiting
- AdicionarHealthChecks
- Configurar Docker/Docker Compose

## Contribuindo

Este é um template base. Sinta-se livre para adaptar conforme as necessidades do seu projeto.

## Licença

MIT License - Use como quiser!

---

**Desenvolvido com .NET 8 e Clean Architecture**
