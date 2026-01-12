# Guia de Autenticação JWT

Guia completo para configurar e testar a autenticação JWT na API.

## 1. Configuração Inicial

### Passo 1: Execute o script de setup completo

```powershell
cd C:\VsoPersonal\baseApi
.\setup-complete.ps1
```

Este script irá configurar:
- ✅ Connection String do PostgreSQL
- ✅ JWT Secret (gerado automaticamente)
- ✅ JWT Issuer
- ✅ JWT Audience
- ✅ JWT Tempo de expiração

**Valores recomendados:**
- Connection String: `Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=postgres`
- JWT Issuer: `BaseApi` (padrão)
- JWT Audience: `BaseApiUsers` (padrão)
- Expiração: `60` minutos (padrão)

---

## 2. Executar a Aplicação

No Visual Studio:
1. Pressione **F5**
2. O Swagger abrirá em `https://localhost:7000`

---

## 3. Testar Autenticação no Swagger

### 🔓 Endpoints Públicos (Sem Autenticação)

Estes endpoints funcionam sem token:

- `GET /api/users` - Listar usuários
- `GET /api/users/{id}` - Buscar usuário por ID
- `POST /api/auth/register` - Registrar novo usuário
- `POST /api/auth/login` - Fazer login

### 🔒 Endpoints Protegidos (Requerem Autenticação)

Estes endpoints precisam de token JWT:

- `POST /api/users` - Criar usuário
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Deletar usuário (soft delete)

---

## 4. Fluxo Completo de Teste

### Passo 1: Registrar um Novo Usuário

No Swagger, expanda `POST /api/auth/register`:

1. Clique em **"Try it out"**
2. Cole este JSON no Request body:

```json
{
  "name": "Admin User",
  "email": "admin@exemplo.com",
  "password": "senha123",
  "confirmPassword": "senha123",
  "role": "Admin"
}
```

3. Clique em **"Execute"**

**Resposta esperada (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "admin@exemplo.com",
  "name": "Admin User",
  "role": "Admin",
  "expiresAt": "2026-01-12T15:30:00Z"
}
```

**📋 Copie o valor do campo `token`!**

---

### Passo 2: Autenticar no Swagger

1. No topo da página do Swagger, clique no botão **"Authorize"** 🔓
2. Cole o token no campo **Value** (sem aspas, apenas o token)
3. Clique em **"Authorize"**
4. Clique em **"Close"**

Agora o cadeado 🔒 deve aparecer como **fechado** nos endpoints protegidos!

---

### Passo 3: Testar Endpoint Protegido - Criar Usuário

Expanda `POST /api/users`:

1. Clique em **"Try it out"**
2. Cole este JSON:

```json
{
  "name": "João Silva",
  "email": "joao@exemplo.com",
  "passwordHash": "hash_temporario",
  "role": "User",
  "isActive": true
}
```

3. Clique em **"Execute"**

**Resposta esperada (201 Created):**
```json
{
  "id": "uuid-gerado",
  "name": "João Silva",
  "email": "joao@exemplo.com",
  "role": "User",
  "isActive": true,
  "createdAt": "2026-01-12T12:00:00Z",
  "updatedAt": "2026-01-12T12:00:00Z"
}
```

---

### Passo 4: Fazer Login (Alternativa ao Register)

Se você já tem um usuário, pode fazer login ao invés de registrar:

Expanda `POST /api/auth/login`:

```json
{
  "email": "admin@exemplo.com",
  "password": "senha123"
}
```

**Resposta:** Mesmo formato do register, com o token JWT.

---

### Passo 5: Testar Atualização de Usuário

Expanda `PUT /api/users/{id}`:

1. Copie o `id` de um usuário existente
2. Cole no campo `id`
3. Clique em **"Try it out"**
4. Atualize os dados:

```json
{
  "id": "cole-o-uuid-aqui",
  "name": "João Silva Atualizado",
  "email": "joao@exemplo.com",
  "passwordHash": "hash_temporario",
  "role": "Admin",
  "isActive": true
}
```

5. Clique em **"Execute"**

**Resposta esperada:** `204 No Content`

---

### Passo 6: Testar Deleção (Soft Delete)

Expanda `DELETE /api/users/{id}`:

1. Cole o `id` do usuário
2. Clique em **"Execute"**

**Resposta esperada:** `204 No Content`

O usuário será marcado como `isActive = false` (soft delete).

---

## 5. Erros Comuns e Soluções

### ❌ Erro 401 Unauthorized

**Causa:** Token não fornecido ou inválido

**Solução:**
1. Verifique se você clicou em **"Authorize"**
2. Certifique-se de que copiou o token completo
3. O token não deve ter aspas ou espaços
4. O token expira em 60 minutos (faça login novamente)

---

### ❌ Erro 403 Forbidden

**Causa:** Você não tem permissão para acessar esse recurso

**Solução:**
- Verifique se seu usuário tem a Role adequada (Admin vs User)

---

### ❌ Erro 400 Bad Request - "Email já está em uso"

**Causa:** Tentativa de registrar com email duplicado

**Solução:**
- Use outro email
- Ou faça login com o email existente

---

### ❌ Erro 400 Bad Request - "Email ou senha inválidos"

**Causa:** Credenciais incorretas no login

**Solução:**
- Verifique o email e senha
- Certifique-se de que o usuário está ativo (`isActive = true`)

---

## 6. Testar com Postman ou cURL

### Registrar usuário (cURL)

```bash
curl -X POST https://localhost:7000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Admin",
    "email": "admin@exemplo.com",
    "password": "senha123",
    "confirmPassword": "senha123",
    "role": "Admin"
  }'
```

### Fazer login (cURL)

```bash
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@exemplo.com",
    "password": "senha123"
  }'
```

### Criar usuário com token (cURL)

```bash
curl -X POST https://localhost:7000/api/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "name": "João",
    "email": "joao@exemplo.com",
    "passwordHash": "hash",
    "role": "User",
    "isActive": true
  }'
```

---

## 7. Verificar Configurações JWT

Para ver as configurações atuais:

```powershell
cd C:\VsoPersonal\baseApi\src\BaseApi.Api
dotnet user-secrets list
```

Deve mostrar:
```
ConnectionStrings:DefaultConnection = Host=localhost;...
Jwt:Secret = (64 caracteres)
Jwt:Issuer = BaseApi
Jwt:Audience = BaseApiUsers
Jwt:ExpireMinutes = 60
```

---

## 8. Estrutura do Token JWT

O token JWT contém as seguintes claims:

```
{
  "nameid": "uuid-do-usuario",
  "name": "Nome do Usuário",
  "email": "email@exemplo.com",
  "role": "Admin ou User",
  "jti": "token-id-unico",
  "exp": timestamp-expiracao,
  "iss": "BaseApi",
  "aud": "BaseApiUsers"
}
```

---

## 9. Segurança

### ⚠️ Importante

- **NUNCA** commite o JWT Secret no Git
- Use secrets complexos em produção (mínimo 64 caracteres)
- Tokens expiram em 60 minutos (configurável)
- Senhas são hash eadas com BCrypt
- Use HTTPS em produção

### Exemplo de JWT Secret forte (Produção)

```
kJ8mN2pQ5rT7uW9yA1cE3gI5kM7oQ9sU1wY3aD5fH7jL9nP1rT3vX5zA7cF9hK1m
```

---

## 10. Próximos Passos

Melhorias futuras sugeridas:

- [ ] Adicionar refresh tokens
- [ ] Implementar revogação de tokens
- [ ] Adicionar rate limiting
- [ ] Implementar 2FA (autenticação de dois fatores)
- [ ] Adicionar políticas de autorização por Role
- [ ] Implementar password reset
- [ ] Adicionar logging de tentativas de login

---

## Fluxo Visual Resumido

```
┌─────────────────────────────────────────────────────────┐
│  1. POST /api/auth/register                             │
│     → Cadastra usuário e retorna token JWT              │
└─────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│  2. Clique em "Authorize" no Swagger                    │
│     → Cole o token JWT                                   │
└─────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│  3. Agora você pode usar endpoints protegidos:          │
│     - POST /api/users (criar)                           │
│     - PUT /api/users/{id} (editar)                      │
│     - DELETE /api/users/{id} (deletar)                  │
└─────────────────────────────────────────────────────────┘
```

---

**Pronto! Sua API está protegida com JWT Authentication! 🔒🚀**
