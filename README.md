# Base API - .NET 8 + PostgreSQL

API completa com autenticação JWT, RBAC, sistema White-Label e hierarquia de administradores usando Clean Architecture.

## Stack Tecnológica

- **.NET 8** (C# 12)
- **PostgreSQL** (Supabase)
- **Entity Framework Core** (ORM + Migrations)
- **Npgsql** (PostgreSQL Driver)
- **BCrypt.Net** (Hash de senhas)
- **JWT Bearer** (Autenticação)
- **Swagger/OpenAPI** (Documentação automática)
- **Clean Architecture** (4 camadas)
- **DotNetEnv** (Variáveis de ambiente)

## Arquitetura Clean Architecture

```
src/
├── BaseApi.Domain/          # Entidades, Enums, Regras de Negócio
│   ├── Entities/
│   │   ├── BaseEntity.cs           # Id, CreatedAt, UpdatedAt
│   │   ├── User.cs                 # Usuários + Hierarquia (CreatedById)
│   │   └── AppSettings.cs          # Configurações White-Label
│   └── Enums/
│       └── UserRole.cs             # User, Admin, Moderator
│
├── BaseApi.Application/     # DTOs, Services, Interfaces
│   ├── DTOs/
│   │   ├── AuthResponse.cs         # Login response (token, role, createdById)
│   │   ├── CreateUserRequest.cs    # DTO para criação com validações
│   │   ├── UpdateUserRequest.cs    # DTO para atualização
│   │   └── AppSettingsResponse.cs  # DTO para configurações white-label
│   ├── Services/
│   │   ├── AuthService.cs          # Login/Register + JWT generation
│   │   ├── TokenService.cs         # JWT token creation
│   │   └── AppSettingsService.cs   # Gerenciamento de configurações
│   └── Interfaces/
│
├── BaseApi.Infra/          # DbContext, Repositories, Migrations
│   ├── Data/
│   │   ├── ApplicationDbContext.cs # EF Core DbContext
│   │   └── DatabaseSeeder.cs       # Seed inicial (admin + settings)
│   ├── Repositories/               # User + AppSettings Repositories
│   └── DependencyInjection.cs      # Registro de serviços
│
└── BaseApi.Api/            # Controllers, Program.cs, Configurações
    ├── Controllers/
    │   ├── AuthController.cs       # POST /login, /register
    │   ├── UsersController.cs      # CRUD com hierarquia
    │   └── AppSettingsController.cs # GET/PUT configurações (admin raiz)
    └── Program.cs                  # JWT, CORS, Swagger, Enum serialization
```

## Features Principais

### 🔐 Autenticação JWT + RBAC
- **JWT tokens** com expiração configurável (8 horas)
- **BCrypt** para hash de senhas
- **Role-Based Access Control** (Admin, User, Moderator)
- **Bearer token** em todos os endpoints protegidos
- **Claims-based authentication** (NameIdentifier, Role, Email)

### 👥 Hierarquia de Administradores
- **Admin Raiz** - Primeiro admin criado (sem `CreatedById`)
  - Vê todos os usuários
  - Cria novos admins/usuários
  - Acessa configurações do sistema
- **Admins Secundários** - Criados por outros admins
  - Veem apenas usuários que criaram
  - Criam usuários vinculados a eles
  - Não podem editar/excluir quem os criou
  - Sem acesso às configurações do sistema

### 🎨 Sistema White-Label
- **Configurações dinâmicas** via banco de dados
- **BrandName** - Nome da marca
- **LogoUrl** - URL da logo
- **PrimaryColor, SecondaryColor, AccentColor** - Cores HEX
- **Endpoint público** - GET /api/AppSettings (sem auth)
- **Endpoint restrito** - PUT /api/AppSettings (apenas admin raiz)

### 🗄️ Banco de Dados
- **PostgreSQL** via Supabase
- **Migrations automáticas** na inicialização
- **Seed automático** - Cria admin padrão e configurações iniciais
- **Soft delete** - Usuários marcados como `IsActive = false`
- **Timestamps** - CreatedAt e UpdatedAt em todas entidades

## Estrutura do Banco de Dados

### Users
| Campo        | Tipo      | Descrição                              |
|--------------|-----------|----------------------------------------|
| Id           | Guid      | PK, gerado automaticamente             |
| Name         | string    | Nome do usuário                        |
| Email        | string    | Email único                            |
| PasswordHash | string    | Hash BCrypt da senha                   |
| Role         | enum      | User, Admin ou Moderator               |
| IsActive     | bool      | Status ativo/inativo                   |
| CreatedById  | Guid?     | Referência para quem criou (hierarquia)|
| CreatedAt    | DateTime  | Data de criação                        |
| UpdatedAt    | DateTime  | Data de última atualização             |

### AppSettings
| Campo          | Tipo      | Descrição                    |
|----------------|-----------|------------------------------|
| Id             | Guid      | PK, único registro           |
| BrandName      | string    | Nome da marca/empresa        |
| LogoUrl        | string?   | URL da logo (opcional)       |
| PrimaryColor   | string    | Cor primária (HEX)           |
| SecondaryColor | string    | Cor secundária (HEX)         |
| AccentColor    | string    | Cor de destaque (HEX)        |
| CreatedAt      | DateTime  | Data de criação              |
| UpdatedAt      | DateTime  | Data de atualização          |

## Instalação e Configuração

### 1. Clonar e instalar dependências
```bash
cd src/BaseApi.Api
dotnet restore
```

### 2. Configurar variáveis de ambiente
Crie um arquivo `.env` na raiz do projeto (pasta `baseApi/`):

```env
# PostgreSQL Connection (Supabase)
CONNECTIONSTRINGS__DEFAULTCONNECTION=Host=aws-1-sa-east-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.xyz;Password=suasenha;SSL Mode=Prefer;Trust Server Certificate=true;Timeout=60;Connection Idle Lifetime=300;Keepalive=30;Multiplexing=false

# JWT Configuration
JWT__SECRET=sua_chave_secreta_minimo_32_caracteres
JWT__ISSUER=BaseApi
JWT__AUDIENCE=BaseApiUsers
JWT__EXPIREMINUTES=480

# Admin Padrão (criado automaticamente)
ADMIN__NAME=Administrador
ADMIN__EMAIL=admin@admin.com
ADMIN__PASSWORD=mudar123
```

### 3. Rodar a API
```bash
dotnet run
```

A API estará disponível em:
- **Swagger UI**: https://localhost:7000
- **HTTP**: http://localhost:5000

**Migrations e seed são executados automaticamente** na inicialização.

## Endpoints da API

### Autenticação
```http
POST /api/auth/login
POST /api/auth/register
```

### Usuários (com hierarquia)
```http
GET    /api/users              # Lista usuários (filtrado por hierarquia)
GET    /api/users/{id}         # Busca usuário por ID
POST   /api/users              # Cria usuário (salva CreatedById)
PUT    /api/users/{id}         # Atualiza usuário (valida hierarquia)
DELETE /api/users/{id}         # Soft delete (valida hierarquia)
GET    /api/users/inactive     # Lista usuários inativos [Authorize]
```

### Configurações White-Label
```http
GET /api/AppSettings           # Público - retorna configurações
PUT /api/AppSettings           # Admin raiz - atualiza configurações
```

## Autenticação JWT

### Login Request
```json
POST /api/auth/login
{
  "email": "email@example.com",
  "password": "pwdexample"
}
```

### Login Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "DEFINA_EMAIL_ROOT",
  "name": "Administrador",
  "role": "Admin",
  "expiresAt": "2026-01-22T08:00:00Z",
  "createdById": null  // null = admin raiz
}
```

### Usando o Token
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Hierarquia de Administradores

### Criar Usuário (Admin raiz)
```json
POST /api/users
Authorization: Bearer {token_admin_raiz}

{
  "name": "Novo Admin",
  "email": "email2@example.com",
  "password": "pwdexample",
  "role": "Admin"
}
```

O campo `CreatedById` é preenchido automaticamente com o ID do usuário autenticado.

### Validações de Hierarquia
- **GET /api/users**: Retorna apenas usuários criados pelo admin logado (ou todos se for admin raiz)
- **PUT /api/users/{id}**: Bloqueia edição de quem criou você (403 Forbidden)
- **DELETE /api/users/{id}**: Bloqueia exclusão de quem criou você (403 Forbidden)

## Sistema White-Label

### Obter Configurações (Público)
```http
GET /api/AppSettings
```

```json
{
  "brandName": "Minha Empresa",
  "logoUrl": "https://exemplo.com/logo.png",
  "primaryColor": "#3B82F6",
  "secondaryColor": "#8B5CF6",
  "accentColor": "#22C55E"
}
```

### Atualizar Configurações (Admin Raiz)
```http
PUT /api/AppSettings
Authorization: Bearer {token_admin_raiz}

{
  "brandName": "Nova Marca",
  "logoUrl": "https://exemplo.com/nova-logo.png",
  "primaryColor": "#FF5733",
  "secondaryColor": "#C70039",
  "accentColor": "#900C3F"
}
```

**Importante**: Apenas o admin raiz (sem `CreatedById`) pode alterar as configurações. Admins secundários recebem 403 Forbidden.

## Migrations do Entity Framework

### Criar nova migration
```bash
cd src/BaseApi.Infra
dotnet ef migrations add NomeDaMigration --startup-project ../BaseApi.Api
```

### Aplicar migrations manualmente (não necessário, feito automaticamente)
```bash
dotnet ef database update --startup-project ../BaseApi.Api
```

### Listar migrations
```bash
dotnet ef migrations list --startup-project ../BaseApi.Api
```

### Remover última migration
```bash
dotnet ef migrations remove --startup-project ../BaseApi.Api
```

## Validações e Segurança

### Validações de Senha
- Mínimo 6 caracteres
- Hash BCrypt automático
- Senha nunca retornada nas respostas

### Validações de Email
- Formato válido
- Unicidade garantida
- Case-insensitive

### Proteções RBAC
- `[Authorize]` - Requer autenticação
- `[Authorize(Roles = "Admin")]` - Requer role Admin
- `[AllowAnonymous]` - Endpoint público

### Enum Serialization
Enums são serializados como strings no JSON:
```json
{
  "role": "Admin"  // ✅ string, não número
}
```

Configurado em `Program.cs`:
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
```

## Configuração do PostgreSQL (Supabase)

O projeto está configurado para usar Supabase com pooling:

```csharp
// DependencyInjection.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(60);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    }));
```

## Seed Automático

Na primeira execução, o sistema cria automaticamente:

### Admin Padrão configure no arquivo .env
- Email: `admin@example.com`
- Senha: `pwdexample`
- Role: Admin
- CreatedById: null (admin raiz)

### Configurações Padrão
- BrandName: "Base API"
- PrimaryColor: "#3B82F6" (azul)
- SecondaryColor: "#8B5CF6" (roxo)
- AccentColor: "#22C55E" (verde)

## CORS

CORS está configurado para aceitar todas as origens em desenvolvimento:

```csharp
app.UseCors("AllowAll");
```

Para produção, configure origens específicas em `Program.cs`.

## Swagger/OpenAPI

Swagger está sempre ativo e disponível na raiz:
- https://localhost:7000

Inclui autenticação JWT:
1. Clique em "Authorize" no topo
2. Digite: `Bearer {seu_token}`
3. Teste endpoints protegidos

## Licença

MIT
