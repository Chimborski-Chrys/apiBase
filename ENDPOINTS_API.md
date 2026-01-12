# Endpoints da API

Documentação completa de todos os endpoints disponíveis.

---

## 🔓 Endpoints Públicos (Sem Autenticação)

### 1. Listar Usuários Ativos
**GET** `/api/users`

Lista todos os usuários ativos (IsActive = true).

**Resposta:** `200 OK`
```json
[
  {
    "id": "uuid",
    "name": "João Silva",
    "email": "joao@exemplo.com",
    "passwordHash": "hash",
    "role": "User",
    "isActive": true,
    "createdAt": "2026-01-12T10:00:00Z",
    "updatedAt": "2026-01-12T10:00:00Z"
  }
]
```

---

### 2. Buscar Usuário por ID
**GET** `/api/users/{id}`

Busca um usuário ativo específico por ID.

**Parâmetros:**
- `id` (UUID): ID do usuário

**Resposta:** `200 OK` ou `404 Not Found`

---

### 3. Registrar Novo Usuário
**POST** `/api/auth/register`

Cria um novo usuário e retorna token JWT.

**Body:**
```json
{
  "name": "João Silva",
  "email": "joao@exemplo.com",
  "password": "senha123",
  "confirmPassword": "senha123",
  "role": "User"
}
```

**Resposta:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "joao@exemplo.com",
  "name": "João Silva",
  "role": "User",
  "expiresAt": "2026-01-13T10:00:00Z"
}
```

---

### 4. Login
**POST** `/api/auth/login`

Autentica um usuário e retorna token JWT.

**Body:**
```json
{
  "email": "joao@exemplo.com",
  "password": "senha123"
}
```

**Resposta:** `200 OK` ou `401 Unauthorized`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "joao@exemplo.com",
  "name": "João Silva",
  "role": "User",
  "expiresAt": "2026-01-13T10:00:00Z"
}
```

---

## 🔒 Endpoints Protegidos (Requerem Autenticação)

**Como autenticar:**
1. Faça login em `/api/auth/login`
2. Copie o token da resposta
3. Clique em "Authorize" no Swagger
4. Cole o token
5. Use os endpoints abaixo

---

### 5. Listar Usuários Inativos
**GET** `/api/users/inactive`

Lista todos os usuários inativos (IsActive = false).

**Headers:**
```
Authorization: Bearer {seu_token}
```

**Resposta:** `200 OK`
```json
[
  {
    "id": "uuid",
    "name": "Maria Santos",
    "email": "maria@exemplo.com",
    "passwordHash": "hash",
    "role": "User",
    "isActive": false,
    "createdAt": "2026-01-10T10:00:00Z",
    "updatedAt": "2026-01-12T15:00:00Z"
  }
]
```

**Erros:**
- `401 Unauthorized` - Token não fornecido ou inválido

---

### 6. Criar Usuário
**POST** `/api/users`

Cria um novo usuário (requer autenticação).

**Headers:**
```
Authorization: Bearer {seu_token}
```

**Body:**
```json
{
  "name": "Pedro Santos",
  "email": "pedro@exemplo.com",
  "passwordHash": "hash_bcrypt",
  "role": "User",
  "isActive": true
}
```

**Resposta:** `201 Created`

---

### 7. Atualizar Usuário
**PUT** `/api/users/{id}`

Atualiza um usuário existente. **Todos os campos são opcionais** - envie apenas o que deseja atualizar.

**Headers:**
```
Authorization: Bearer {seu_token}
```

**Parâmetros:**
- `id` (UUID): ID do usuário

**Body (todos os campos são opcionais):**

Atualizar apenas email:
```json
{
  "email": "novoemail@exemplo.com"
}
```

Atualizar apenas nome:
```json
{
  "name": "João Silva Atualizado"
}
```

Atualizar email e role:
```json
{
  "email": "joao@exemplo.com",
  "role": "Admin"
}
```

Atualizar múltiplos campos:
```json
{
  "name": "João Silva",
  "email": "joao@exemplo.com",
  "role": "Admin",
  "isActive": false
}
```

Atualizar senha:
```json
{
  "passwordHash": "novo_hash_bcrypt"
}
```

**Resposta:** `200 OK` com dados atualizados

**Erros:**
- `400 Bad Request` - Dados inválidos
- `401 Unauthorized` - Não autenticado
- `404 Not Found` - Usuário não existe

---

### 8. Deletar Usuário (Soft Delete)
**DELETE** `/api/users/{id}`

Desativa um usuário (marca IsActive = false). Não remove do banco.

**Headers:**
```
Authorization: Bearer {seu_token}
```

**Parâmetros:**
- `id` (UUID): ID do usuário

**Resposta:** `204 No Content`

**Erros:**
- `401 Unauthorized` - Não autenticado
- `404 Not Found` - Usuário não existe

---

## 📊 Resumo dos Endpoints

| Método | Endpoint | Auth | Descrição |
|--------|----------|------|-----------|
| GET | `/api/users` | ❌ Não | Lista usuários ativos |
| GET | `/api/users/inactive` | ✅ Sim | Lista usuários inativos |
| GET | `/api/users/{id}` | ❌ Não | Busca usuário por ID |
| POST | `/api/auth/register` | ❌ Não | Registrar novo usuário |
| POST | `/api/auth/login` | ❌ Não | Login |
| POST | `/api/users` | ✅ Sim | Criar usuário |
| PUT | `/api/users/{id}` | ✅ Sim | Atualizar usuário |
| DELETE | `/api/users/{id}` | ✅ Sim | Deletar usuário (soft) |

---

## 🔐 Autenticação JWT

### Token de Acesso

O token JWT contém:
- **nameid**: ID do usuário
- **name**: Nome do usuário
- **email**: Email do usuário
- **role**: Role do usuário (Admin/User)
- **exp**: Data de expiração (24 horas)

### Como Usar o Token

**Via Swagger:**
1. Clique em "Authorize" 🔓
2. Cole o token (sem "Bearer")
3. Clique em "Authorize"

**Via cURL/Postman:**
```bash
curl -H "Authorization: Bearer {seu_token}" https://localhost:7000/api/users/inactive
```

**Via JavaScript:**
```javascript
fetch('https://localhost:7000/api/users/inactive', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
```

### Expiração

Tokens expiram em **24 horas** (1440 minutos).

Se receber `401 Unauthorized`:
1. Faça login novamente
2. Obtenha novo token
3. Atualize o token no "Authorize"

---

## ⚠️ Códigos de Status HTTP

| Código | Significado | Quando Ocorre |
|--------|-------------|---------------|
| 200 | OK | Requisição bem-sucedida |
| 201 | Created | Recurso criado com sucesso |
| 204 | No Content | Operação bem-sucedida sem retorno |
| 400 | Bad Request | Dados inválidos ou faltando |
| 401 | Unauthorized | Token não fornecido ou inválido |
| 403 | Forbidden | Sem permissão para o recurso |
| 404 | Not Found | Recurso não encontrado |
| 500 | Internal Server Error | Erro no servidor |

---

## 🧪 Exemplos de Testes

### 1. Fluxo Completo

```bash
# 1. Registrar
curl -X POST https://localhost:7000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Admin",
    "email": "admin@exemplo.com",
    "password": "senha123",
    "confirmPassword": "senha123",
    "role": "Admin"
  }'

# 2. Login (copie o token)
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@exemplo.com",
    "password": "senha123"
  }'

# 3. Listar usuários inativos (com token)
curl -X GET https://localhost:7000/api/users/inactive \
  -H "Authorization: Bearer {seu_token}"

# 4. Atualizar usuário (com token)
curl -X PUT https://localhost:7000/api/users/{id} \
  -H "Authorization: Bearer {seu_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "novoemail@exemplo.com"
  }'

# 5. Deletar usuário (com token)
curl -X DELETE https://localhost:7000/api/users/{id} \
  -H "Authorization: Bearer {seu_token}"
```

---

## 📝 Notas Importantes

1. **Soft Delete**: O DELETE não remove do banco, apenas marca `isActive = false`
2. **Update Parcial**: PUT aceita campos parciais - envie apenas o que deseja atualizar
3. **Senha**: O campo `passwordHash` aceita qualquer string. Em produção, use BCrypt
4. **CORS**: CORS está habilitado para todas as origens em desenvolvimento
5. **HTTPS**: Use HTTPS em produção

---

**Base URL:** `https://localhost:7000` (desenvolvimento)
