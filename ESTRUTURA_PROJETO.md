# Estrutura do Projeto - Base API

## Visão Geral

Template base para API seguindo Clean Architecture com .NET 8 e PostgreSQL.

## Estrutura de Diretórios

```
baseApi/
│
├── src/
│   │
│   ├── BaseApi.Domain/                    # Camada de Domínio
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs             # Classe base para entidades
│   │   │   └── User.cs                    # Entidade de usuário
│   │   ├── Enums/
│   │   │   └── UserRole.cs               # Enum de perfis (Admin/User)
│   │   └── BaseApi.Domain.csproj
│   │
│   ├── BaseApi.Application/               # Camada de Aplicação
│   │   ├── Interfaces/
│   │   │   └── IUserRepository.cs        # Interface do repositório de usuários
│   │   ├── Services/                      # (Para implementações futuras)
│   │   └── BaseApi.Application.csproj
│   │
│   ├── BaseApi.Infra/                     # Camada de Infraestrutura
│   │   ├── Data/
│   │   │   └── ApplicationDbContext.cs   # Contexto do EF Core
│   │   ├── Repositories/
│   │   │   └── UserRepository.cs         # Implementação do repositório
│   │   ├── DependencyInjection.cs        # Configuração de DI
│   │   └── BaseApi.Infra.csproj
│   │
│   └── BaseApi.Api/                       # Camada de Apresentação
│       ├── Controllers/
│       │   └── UsersController.cs        # Controller de usuários (CRUD)
│       ├── Properties/
│       │   └── launchSettings.json       # Configurações de porta
│       ├── Program.cs                     # Ponto de entrada da aplicação
│       ├── appsettings.json              # Configurações gerais
│       ├── appsettings.Development.json  # Configurações de dev
│       └── BaseApi.Api.csproj            # Projeto principal com UserSecretsId
│
├── setup-dev.ps1                          # Script de configuração (Windows)
├── setup-dev.sh                           # Script de configuração (Linux/Mac)
├── docker-compose.yml                     # PostgreSQL via Docker
├── Dockerfile                             # Build da aplicação
├── .dockerignore                          # Ignorar arquivos no build Docker
├── .gitignore                             # Arquivos ignorados pelo Git
├── .env.example                           # Exemplo de variáveis de ambiente
├── BaseApi.sln                            # Arquivo de solução
├── README.md                              # Documentação principal
├── COMANDOS_UTEIS.md                      # Comandos úteis do dia a dia
└── ESTRUTURA_PROJETO.md                   # Este arquivo
```

## Camadas e Responsabilidades

### 1. Domain (BaseApi.Domain)
**Responsabilidade:** Regras de negócio e entidades do domínio.

**Características:**
- Não depende de nenhuma outra camada
- Contém as entidades do negócio
- Define enums e value objects
- Sem dependências externas

**Arquivos:**
- `BaseEntity.cs`: Classe base com Id, CreatedAt, UpdatedAt
- `User.cs`: Entidade de usuário com Name, Email, PasswordHash, Role, IsActive
- `UserRole.cs`: Enum com perfis Admin e User

### 2. Application (BaseApi.Application)
**Responsabilidade:** Lógica de aplicação e casos de uso.

**Características:**
- Depende apenas do Domain
- Define interfaces (contratos)
- Implementa regras de negócio de aplicação
- Sem dependências de infraestrutura

**Arquivos:**
- `IUserRepository.cs`: Interface do repositório de usuários

### 3. Infrastructure (BaseApi.Infra)
**Responsabilidade:** Implementações de infraestrutura.

**Características:**
- Depende do Domain e Application
- Implementa acesso a dados (EF Core)
- Implementa repositórios
- Configura DbContext
- Registra dependências

**Arquivos:**
- `ApplicationDbContext.cs`: Contexto do EF Core com mapeamento de User
- `UserRepository.cs`: Implementação do IUserRepository
- `DependencyInjection.cs`: Configuração de injeção de dependências

### 4. API (BaseApi.Api)
**Responsabilidade:** Exposição de endpoints HTTP.

**Características:**
- Depende de todas as outras camadas
- Define controllers e endpoints
- Configura middleware
- Ponto de entrada da aplicação

**Arquivos:**
- `Program.cs`: Configuração da aplicação, DI, Swagger
- `UsersController.cs`: CRUD completo de usuários
- `appsettings.json`: Configurações (sem secrets)
- `launchSettings.json`: Portas (https://localhost:7000)

## Fluxo de Dependências

```
API → Infra → Application → Domain
    ↘      ↘             ↗
      Infrastructure ────┘
```

- Domain não conhece ninguém (núcleo)
- Application conhece apenas Domain
- Infra conhece Domain e Application
- API conhece todos (orquestra)

## Segurança Implementada

### User Secrets
- Connection String armazenada fora do código
- Configurada via `dotnet user-secrets`
- UserSecretsId: `baseapi-secrets`
- Scripts automatizados para setup

### .gitignore
- Ignora bin/, obj/, .vs/
- Ignora *.user, *.suo
- **CRÍTICO:** Ignora appsettings.*.json (exceto o exemplo)
- Ignora connection strings
- Ignora .env

## Recursos Implementados

### Entidades
- [x] BaseEntity com timestamps
- [x] User com soft delete (IsActive)
- [x] UserRole enum (Admin/User)

### Banco de Dados
- [x] ApplicationDbContext configurado
- [x] Mapeamento fluente de User
- [x] Índice único em Email
- [x] Auto-update de UpdatedAt
- [x] Conversão de Role para string

### Repository Pattern
- [x] IUserRepository interface
- [x] UserRepository implementação
- [x] Registrado no DI

### API
- [x] CRUD completo de Users
- [x] Soft delete implementado
- [x] Swagger configurado na raiz
- [x] CORS habilitado para dev

### DevOps
- [x] Docker Compose para PostgreSQL
- [x] Dockerfile para build da API
- [x] Scripts de setup (PS1 e SH)
- [x] .env.example

## Próximos Passos Recomendados

### Autenticação e Autorização
- [ ] Implementar JWT
- [ ] Adicionar BCrypt para hash de senhas
- [ ] Middleware de autenticação
- [ ] Políticas de autorização por Role

### Validação
- [ ] FluentValidation
- [ ] DTOs (Data Transfer Objects)
- [ ] AutoMapper

### Testes
- [ ] xUnit
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Mock de repositórios

### Logging e Monitoramento
- [ ] Serilog
- [ ] Application Insights / ELK
- [ ] Health Checks

### Performance
- [ ] Cache (Redis)
- [ ] Paginação
- [ ] Rate Limiting

### Documentação
- [ ] XML Comments
- [ ] Swagger melhorado
- [ ] Postman Collection

## Tecnologias e Versões

| Tecnologia | Versão |
|-----------|--------|
| .NET | 8.0 |
| Entity Framework Core | 8.0.0 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.0 |
| Swashbuckle.AspNetCore | 6.5.0 |
| PostgreSQL | 16 (Alpine) |

## Comandos Rápidos

### Setup Inicial
```bash
# 1. Configurar secrets
.\setup-dev.ps1

# 2. Subir PostgreSQL
docker-compose up -d

# 3. Aplicar migrations
cd src/BaseApi.Api
dotnet ef migrations add InitialCreate --project ../BaseApi.Infra
dotnet ef database update --project ../BaseApi.Infra

# 4. Executar
dotnet run
```

### Desenvolvimento
```bash
# Executar com hot reload
dotnet watch run

# Compilar
dotnet build

# Limpar
dotnet clean
```

## Portas Configuradas

| Serviço | Porta |
|---------|-------|
| API (HTTPS) | 7000 |
| API (HTTP) | 5000 |
| PostgreSQL | 5432 |
| Swagger UI | https://localhost:7000 |

## Conexão com Banco

### Local (Docker)
```
Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=postgres
```

### Produção (Exemplo)
```
Host=seu-servidor.com;Port=5432;Database=baseapi_prod;Username=user;Password=pass;SSL Mode=Require
```

## Princípios Aplicados

- **SOLID**
- **Clean Architecture**
- **Repository Pattern**
- **Dependency Injection**
- **Separation of Concerns**
- **Security by Design**

---

**Criado com .NET 8 e boas práticas de arquitetura**
